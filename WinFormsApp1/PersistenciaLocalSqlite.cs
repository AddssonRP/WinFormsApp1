using System.Text.Json;
using Microsoft.Data.Sqlite;

namespace WinFormsApp1
{
    internal sealed class PersistenciaLocalSqlite
    {
        private readonly string caminhoBanco;
        private readonly string caminhoJsonLegado;

        public PersistenciaLocalSqlite(string caminhoBanco, string caminhoJsonLegado)
        {
            this.caminhoBanco = caminhoBanco;
            this.caminhoJsonLegado = caminhoJsonLegado;
        }

        public EstadoLoja Carregar()
        {
            string? diretorio = Path.GetDirectoryName(caminhoBanco);
            if (string.IsNullOrWhiteSpace(diretorio))
            {
                throw new InvalidOperationException("O caminho do banco SQLite não contém um diretório válido.");
            }

            Directory.CreateDirectory(diretorio);

            using var conexao = new SqliteConnection(new SqliteConnectionStringBuilder
            {
                DataSource = caminhoBanco,
                Mode = SqliteOpenMode.ReadWriteCreate
            }.ToString());
            conexao.Open();
            CriarEstrutura(conexao);

            using (var comando = conexao.CreateCommand())
            {
                comando.CommandText = "SELECT ImportacaoLegadaConcluida FROM EstadoAplicacao WHERE Id = 1;";
                if (Convert.ToInt32(comando.ExecuteScalar()) == 0)
                {
                    EstadoLoja estado = File.Exists(caminhoJsonLegado)
                        ? JsonSerializer.Deserialize<EstadoLoja>(File.ReadAllText(caminhoJsonLegado))
                            ?? throw new InvalidDataException("O arquivo JSON legado não contém dados válidos.")
                        : new EstadoLoja();
                    ValidarEstado(estado);
                    foreach (ClienteFidelidade cliente in estado.Clientes.Where(cliente => cliente.Pontos > 0 && cliente.UltimaPontuacaoEm is null))
                    {
                        cliente.UltimaPontuacaoEm = DateTime.Now;
                    }
                    Salvar(conexao, estado);
                    return estado;
                }
            }

            return LerEstado(conexao);
        }

        public void Salvar(EstadoLoja estado)
        {
            using var conexao = AbrirConexao();
            Salvar(conexao, estado);
        }

        private SqliteConnection AbrirConexao()
        {
            var conexao = new SqliteConnection(new SqliteConnectionStringBuilder
            {
                DataSource = caminhoBanco,
                Mode = SqliteOpenMode.ReadWriteCreate
            }.ToString());
            conexao.Open();
            return conexao;
        }

        private static void CriarEstrutura(SqliteConnection conexao)
        {
            using var comando = conexao.CreateCommand();
            comando.CommandText = """
                CREATE TABLE IF NOT EXISTS EstadoAplicacao (
                    Id INTEGER PRIMARY KEY CHECK (Id = 1),
                    ImportacaoLegadaConcluida INTEGER NOT NULL DEFAULT 0 CHECK (ImportacaoLegadaConcluida IN (0, 1)),
                    UltimoLembreteData TEXT NULL
                );
                INSERT OR IGNORE INTO EstadoAplicacao (Id, ImportacaoLegadaConcluida)
                    VALUES (1, 0);
                CREATE TABLE IF NOT EXISTS Clientes (
                    Id INTEGER PRIMARY KEY,
                    Nome TEXT NOT NULL,
                    Contato TEXT NOT NULL,
                    Pontos INTEGER NOT NULL CHECK (Pontos >= 0),
                    UltimaPontuacaoEm TEXT NULL
                );
                CREATE TABLE IF NOT EXISTS Agendamentos (
                    Id INTEGER PRIMARY KEY,
                    Nome TEXT NOT NULL,
                    Contato TEXT NOT NULL,
                    Data TEXT NOT NULL,
                    Hora TEXT NOT NULL,
                    TipoAtendimento TEXT NOT NULL
                );
                """;
            comando.ExecuteNonQuery();

            bool possuiDataPontuacao = false;
            using (var consultaColunas = conexao.CreateCommand())
            {
                consultaColunas.CommandText = "PRAGMA table_info(Clientes);";
                using SqliteDataReader leitor = consultaColunas.ExecuteReader();
                while (leitor.Read())
                {
                    if (leitor.GetString(1) == "UltimaPontuacaoEm")
                    {
                        possuiDataPontuacao = true;
                        break;
                    }
                }
            }

            if (!possuiDataPontuacao)
            {
                using var migracao = conexao.CreateCommand();
                migracao.CommandText = "ALTER TABLE Clientes ADD COLUMN UltimaPontuacaoEm TEXT NULL; UPDATE Clientes SET UltimaPontuacaoEm = $data WHERE Pontos > 0;";
                migracao.Parameters.AddWithValue("$data", DateTime.Now.ToString("O", System.Globalization.CultureInfo.InvariantCulture));
                migracao.ExecuteNonQuery();
            }

            bool possuiDataLembrete = false;
            using (var consultaLembrete = conexao.CreateCommand())
            {
                consultaLembrete.CommandText = "PRAGMA table_info(EstadoAplicacao);";
                using SqliteDataReader leitor = consultaLembrete.ExecuteReader();
                while (leitor.Read())
                {
                    if (leitor.GetString(1) == "UltimoLembreteData")
                    {
                        possuiDataLembrete = true;
                        break;
                    }
                }
            }

            if (!possuiDataLembrete)
            {
                using var migracaoLembrete = conexao.CreateCommand();
                migracaoLembrete.CommandText = "ALTER TABLE EstadoAplicacao ADD COLUMN UltimoLembreteData TEXT NULL;";
                migracaoLembrete.ExecuteNonQuery();
            }
        }

        public bool RegistrarLembreteDiario(DateTime data)
        {
            using SqliteConnection conexao = AbrirConexao();
            CriarEstrutura(conexao);
            string dataFormatada = data.ToString("yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
            using SqliteTransaction transacao = conexao.BeginTransaction();

            using (var consulta = conexao.CreateCommand())
            {
                consulta.Transaction = transacao;
                consulta.CommandText = "SELECT UltimoLembreteData FROM EstadoAplicacao WHERE Id = 1;";
                if (string.Equals(consulta.ExecuteScalar() as string, dataFormatada, StringComparison.Ordinal))
                {
                    transacao.Commit();
                    return false;
                }
            }

            using (var atualizacao = conexao.CreateCommand())
            {
                atualizacao.Transaction = transacao;
                atualizacao.CommandText = "UPDATE EstadoAplicacao SET UltimoLembreteData = $data WHERE Id = 1;";
                atualizacao.Parameters.AddWithValue("$data", dataFormatada);
                atualizacao.ExecuteNonQuery();
            }

            transacao.Commit();
            return true;
        }

        private static EstadoLoja LerEstado(SqliteConnection conexao)
        {
            var estado = new EstadoLoja();
            using (var comando = conexao.CreateCommand())
            {
                comando.CommandText = "SELECT Nome, Contato, Pontos, UltimaPontuacaoEm FROM Clientes ORDER BY Id;";
                using SqliteDataReader leitor = comando.ExecuteReader();
                while (leitor.Read())
                {
                    estado.Clientes.Add(new ClienteFidelidade
                    {
                        Nome = leitor.GetString(0),
                        Contato = leitor.GetString(1),
                        Pontos = leitor.GetInt32(2),
                        UltimaPontuacaoEm = leitor.IsDBNull(3)
                            ? null
                            : DateTime.Parse(leitor.GetString(3), System.Globalization.CultureInfo.InvariantCulture,
                                System.Globalization.DateTimeStyles.RoundtripKind)
                    });
                }
            }

            using (var comando = conexao.CreateCommand())
            {
                comando.CommandText = "SELECT Nome, Contato, Data, Hora, TipoAtendimento FROM Agendamentos ORDER BY Id;";
                using SqliteDataReader leitor = comando.ExecuteReader();
                while (leitor.Read())
                {
                    estado.Agendamentos.Add(new AgendamentoVip
                    {
                        Nome = leitor.GetString(0),
                        Contato = leitor.GetString(1),
                        Data = leitor.GetString(2),
                        Hora = leitor.GetString(3),
                        TipoAtendimento = leitor.GetString(4)
                    });
                }
            }

            return estado;
        }

        private static void Salvar(SqliteConnection conexao, EstadoLoja estado)
        {
            CriarEstrutura(conexao);
            using SqliteTransaction transacao = conexao.BeginTransaction();

            using (var comando = conexao.CreateCommand())
            {
                comando.Transaction = transacao;
                comando.CommandText = "UPDATE EstadoAplicacao SET ImportacaoLegadaConcluida = 1 WHERE Id = 1;";
                comando.ExecuteNonQuery();
            }

            using (var comando = conexao.CreateCommand())
            {
                comando.Transaction = transacao;
                comando.CommandText = "DELETE FROM Clientes; DELETE FROM Agendamentos;";
                comando.ExecuteNonQuery();
            }

            foreach (ClienteFidelidade cliente in estado.Clientes)
            {
                using var comando = conexao.CreateCommand();
                comando.Transaction = transacao;
                comando.CommandText = "INSERT INTO Clientes (Nome, Contato, Pontos, UltimaPontuacaoEm) VALUES ($nome, $contato, $pontos, $data);";
                comando.Parameters.AddWithValue("$nome", cliente.Nome);
                comando.Parameters.AddWithValue("$contato", cliente.Contato);
                comando.Parameters.AddWithValue("$pontos", cliente.Pontos);
                comando.Parameters.AddWithValue("$data", cliente.UltimaPontuacaoEm is DateTime data
                    ? data.ToString("O", System.Globalization.CultureInfo.InvariantCulture)
                    : DBNull.Value);
                comando.ExecuteNonQuery();
            }

            foreach (AgendamentoVip agendamento in estado.Agendamentos)
            {
                using var comando = conexao.CreateCommand();
                comando.Transaction = transacao;
                comando.CommandText = "INSERT INTO Agendamentos (Nome, Contato, Data, Hora, TipoAtendimento) VALUES ($nome, $contato, $data, $hora, $tipo);";
                comando.Parameters.AddWithValue("$nome", agendamento.Nome);
                comando.Parameters.AddWithValue("$contato", agendamento.Contato);
                comando.Parameters.AddWithValue("$data", agendamento.Data);
                comando.Parameters.AddWithValue("$hora", agendamento.Hora);
                comando.Parameters.AddWithValue("$tipo", agendamento.TipoAtendimento);
                comando.ExecuteNonQuery();
            }

            transacao.Commit();
        }

        private static void ValidarEstado(EstadoLoja estado)
        {
            if (estado.Agendamentos is null || estado.Clientes is null ||
                estado.Clientes.Any(cliente => cliente is null || cliente.Pontos < 0 || string.IsNullOrWhiteSpace(cliente.Contato)) ||
                estado.Agendamentos.Any(agendamento => agendamento is null || agendamento.Nome is null ||
                    agendamento.Contato is null || agendamento.Data is null || agendamento.Hora is null ||
                    agendamento.TipoAtendimento is null))
            {
                throw new InvalidDataException("O arquivo JSON legado não contém um cadastro válido.");
            }
        }
    }
}
