using System.ComponentModel;
using System.Text.Json;
using Microsoft.Data.Sqlite;

namespace WinFormsApp1
{
    public partial class Form1 : Form
    {
        private const int LimiteAgendamentosPorData = 4;

        private readonly string caminhoArquivo = Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory,
            "dados_desejos_offline.json");
        private readonly PersistenciaLocalSqlite persistencia;
        private readonly EstadoLoja estado = new();
        private readonly BindingList<AgendamentoVip> listaBinding = new();
        private ClienteFidelidade? clienteSelecionado;
        private AgendamentoVip? agendamentoEmEdicao;
        private Button btnCadastrarCliente = null!;
        private Button btnConsultarPontos = null!;
        private Button btnApagarAgendamento = null!;
        private Button btnCorrigirAgendamento = null!;
        private bool persistenciaDisponivel;

        public Form1()
        {
            InitializeComponent();
            persistencia = new PersistenciaLocalSqlite(
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                    "WinFormsApp1", "dados_desejos.sqlite"),
                caminhoArquivo);
            ConfigurarControles();
            AplicarTema();
            persistenciaDisponivel = CarregarDadosOffline();
            btnAgendar.Enabled = persistenciaDisponivel;
            btnPontos.Enabled = persistenciaDisponivel;
            btnCadastrarCliente.Enabled = persistenciaDisponivel;
            btnConsultarPontos.Enabled = persistenciaDisponivel;
            btnApagarAgendamento.Enabled = persistenciaDisponivel;
            btnCorrigirAgendamento.Enabled = persistenciaDisponivel;
            AtualizarInterface();
            Shown += Form1_Shown;
        }

        private void ConfigurarControles()
        {
            cmbHora.Items.Clear();
            cmbHora.Items.AddRange(new object[] { "18:30", "19:30", "20:30", "21:30" });
            cmbHora.SelectedIndex = 0;

            cmbTipo.Items.Clear();
            cmbTipo.Items.AddRange(new object[]
            {
                "Consultoria discreta na loja",
                "Retirada embalada com sigilo",
                "selecao de presentes e kits"
            });
            cmbTipo.SelectedIndex = 0;

            dgvAgendamentos.AutoGenerateColumns = false;
            dgvAgendamentos.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Nome",
                DataPropertyName = nameof(AgendamentoVip.Nome),
                FillWeight = 20
            });
            dgvAgendamentos.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Contato",
                DataPropertyName = nameof(AgendamentoVip.Contato),
                FillWeight = 20
            });
            dgvAgendamentos.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Data",
                DataPropertyName = nameof(AgendamentoVip.Data),
                FillWeight = 12
            });
            dgvAgendamentos.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Horario",
                DataPropertyName = nameof(AgendamentoVip.Hora),
                FillWeight = 10
            });
            dgvAgendamentos.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Atendimento",
                DataPropertyName = nameof(AgendamentoVip.TipoAtendimento),
                FillWeight = 30
            });
            dgvAgendamentos.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colPontuacaoAtual",
                HeaderText = "Pontuação atual",
                FillWeight = 12,
                ReadOnly = true
            });
            dgvAgendamentos.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colCategoria",
                HeaderText = "Categoria",
                FillWeight = 12,
                ReadOnly = true
            });
            dgvAgendamentos.DataSource = listaBinding;
            dgvAgendamentos.CellFormatting += dgvAgendamentos_CellFormatting;
            dgvAgendamentos.SelectionChanged += dgvAgendamentos_SelectionChanged;
            dgvAgendamentos.Dock = DockStyle.None;
            dgvAgendamentos.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            dgvAgendamentos.Location = new Point(4, 24);
            dgvAgendamentos.Size = new Size(grpLista.ClientSize.Width - 8, 156);

            btnCadastrarCliente = new Button
            {
                Font = new Font("Arial", 8.5F, FontStyle.Bold),
                Location = new Point(21, 282),
                Name = "btnCadastrarCliente",
                Size = new Size(196, 32),
                Text = "Cadastrar novo cliente",
                UseVisualStyleBackColor = true
            };
            btnCadastrarCliente.Click += btnCadastrarCliente_Click;
            grpFidelidade.Controls.Add(btnCadastrarCliente);

            btnConsultarPontos = new Button
            {
                Font = new Font("Arial", 8.5F, FontStyle.Bold),
                Location = new Point(232, 282),
                Name = "btnConsultarPontos",
                Size = new Size(196, 32),
                Text = "Consultar pontuação",
                UseVisualStyleBackColor = true
            };
            btnConsultarPontos.Click += btnConsultarPontos_Click;
            grpFidelidade.Controls.Add(btnConsultarPontos);

            btnCorrigirAgendamento = new Button
            {
                Anchor = AnchorStyles.Bottom | AnchorStyles.Left,
                Font = new Font("Arial", 9.5F, FontStyle.Bold),
                Location = new Point(12, grpLista.ClientSize.Height - 42),
                Name = "btnCorrigirAgendamento",
                Size = new Size(250, 32),
                Text = "Corrigir cadastro selecionado",
                UseVisualStyleBackColor = true
            };
            btnCorrigirAgendamento.Click += btnCorrigirAgendamento_Click;
            grpLista.Controls.Add(btnCorrigirAgendamento);

            btnApagarAgendamento = new Button
            {
                Anchor = AnchorStyles.Bottom | AnchorStyles.Right,
                Font = new Font("Arial", 9.5F, FontStyle.Bold),
                Location = new Point(grpLista.ClientSize.Width - 242, grpLista.ClientSize.Height - 42),
                Name = "btnApagarAgendamento",
                Size = new Size(230, 32),
                Text = "Apagar agendamento selecionado",
                UseVisualStyleBackColor = true
            };
            btnApagarAgendamento.Click += btnApagarAgendamento_Click;
            grpLista.Controls.Add(btnApagarAgendamento);
        }

        private void AplicarTema()
        {
            var preto = Color.FromArgb(18, 18, 18);
            var cinzaEscuro = Color.FromArgb(38, 38, 38);
            var vermelho = Color.FromArgb(170, 24, 40);
            var dourado = Color.FromArgb(212, 175, 55);
            var branco = Color.White;

            BackColor = preto;
            ForeColor = branco;
            foreach (Control controle in Controls)
            {
                AplicarTemaRecursivo(controle, preto, cinzaEscuro, dourado, branco);
            }

            lblTitulo.ForeColor = dourado;
            lblSubtitulo.ForeColor = branco;
            lblSaldoPontos.ForeColor = dourado;
            AtualizarFidelidadeSelecionada();

            dgvAgendamentos.BackgroundColor = preto;
            dgvAgendamentos.GridColor = dourado;
            dgvAgendamentos.ColumnHeadersDefaultCellStyle.BackColor = vermelho;
            dgvAgendamentos.ColumnHeadersDefaultCellStyle.ForeColor = branco;
            dgvAgendamentos.ColumnHeadersDefaultCellStyle.Font = new Font("Arial", 10F, FontStyle.Bold);
            dgvAgendamentos.DefaultCellStyle.BackColor = cinzaEscuro;
            dgvAgendamentos.DefaultCellStyle.ForeColor = branco;
            dgvAgendamentos.DefaultCellStyle.SelectionBackColor = vermelho;
            dgvAgendamentos.DefaultCellStyle.SelectionForeColor = branco;
            dgvAgendamentos.DefaultCellStyle.Font = new Font("Arial", 9.5F);
            dgvAgendamentos.EnableHeadersVisualStyles = false;
            dgvAgendamentos.RowTemplate.Height = 30;

            ConfigurarBotao(btnAgendar, vermelho, branco, dourado);
            ConfigurarBotao(btnPontos, dourado, preto, vermelho);
            ConfigurarBotao(btnCadastrarCliente, cinzaEscuro, branco, dourado);
            ConfigurarBotao(btnConsultarPontos, cinzaEscuro, branco, dourado);
            ConfigurarBotao(btnCorrigirAgendamento, dourado, preto, vermelho);
            ConfigurarBotao(btnApagarAgendamento, vermelho, branco, dourado);
        }

        private void AplicarTemaRecursivo(Control controle, Color preto, Color cinzaEscuro,
            Color dourado, Color branco)
        {
            switch (controle)
            {
                case GroupBox:
                    controle.BackColor = cinzaEscuro;
                    controle.ForeColor = dourado;
                    break;
                case TextBoxBase:
                    controle.BackColor = branco;
                    controle.ForeColor = preto;
                    break;
                case ComboBox:
                case NumericUpDown:
                    controle.BackColor = branco;
                    controle.ForeColor = preto;
                    break;
                case DataGridView:
                    controle.BackColor = preto;
                    break;
                case Label:
                    controle.ForeColor = controle == lblSaldoPontos || controle == lblNivel ? dourado : branco;
                    break;
                case Button:
                    break;
                default:
                    controle.BackColor = cinzaEscuro;
                    controle.ForeColor = branco;
                    break;
            }

            foreach (Control filho in controle.Controls)
            {
                AplicarTemaRecursivo(filho, preto, cinzaEscuro, dourado, branco);
            }
        }

        private static void ConfigurarBotao(Button botao, Color fundo, Color texto, Color borda)
        {
            botao.UseVisualStyleBackColor = false;
            botao.FlatStyle = FlatStyle.Flat;
            botao.BackColor = fundo;
            botao.ForeColor = texto;
            botao.FlatAppearance.BorderColor = borda;
            botao.FlatAppearance.BorderSize = 2;
        }
        private void ConfigurarTela()
        {
            Text = "Desejos - Atendimento VIP e Fidelidade";
            StartPosition = FormStartPosition.CenterScreen;
            MinimumSize = new Size(850, 600);
            Size = new Size(1100, 760);

            var layout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(16),
                ColumnCount = 1,
                RowCount = 4
            };
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 48));
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 190));
            layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 112));
            Controls.Add(layout);

            var titulo = new Label
            {
                Text = "Atendimento VIP",
                Dock = DockStyle.Fill,
                Font = new Font(Font.FontFamily, 18, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleLeft
            };
            layout.Controls.Add(titulo, 0, 0);

            var grupoAgendamento = new GroupBox
            {
                Text = "Novo agendamento",
                Dock = DockStyle.Fill,
                Padding = new Padding(12)
            };
            layout.Controls.Add(grupoAgendamento, 0, 1);

            var campos = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 5,
                RowCount = 3,
                Padding = new Padding(4)
            };
            for (int coluna = 0; coluna < 5; coluna++)
            {
                campos.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20));
            }
            campos.RowStyles.Add(new RowStyle(SizeType.Absolute, 32));
            campos.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            campos.RowStyles.Add(new RowStyle(SizeType.Absolute, 44));
            grupoAgendamento.Controls.Add(campos);

            txtNome = new TextBox { Dock = DockStyle.Fill, MaxLength = 100 };
            txtContato = new MaskedTextBox
            {
                Dock = DockStyle.Fill,
                Mask = "(00)00000-0000",
                PromptChar = '_',
                TextMaskFormat = MaskFormat.IncludeLiterals
            };
            dtpData = new DateTimePicker
            {
                Dock = DockStyle.Fill,
                Format = DateTimePickerFormat.Custom,
                CustomFormat = "dd/MM/yyyy"
            };
            cmbHora = new ComboBox { Dock = DockStyle.Fill, DropDownStyle = ComboBoxStyle.DropDownList };
            cmbHora.Items.AddRange(new object[] { "18:30", "19:30", "20:30", "21:30" });
            cmbHora.SelectedIndex = 0;
            cmbTipo = new ComboBox { Dock = DockStyle.Fill, DropDownStyle = ComboBoxStyle.DropDownList };
            cmbTipo.Items.AddRange(new object[]
            {
                "Consultoria discreta na loja",
                "Retirada embalada com sigilo",
                "selecao de presentes e kits"
            });
            cmbTipo.SelectedIndex = 0;

            AdicionarCampo(campos, "Nome ou apelido", txtNome, 0);
            AdicionarCampo(campos, "WhatsApp ou telefone", txtContato, 1);
            AdicionarCampo(campos, "Data", dtpData, 2);
            AdicionarCampo(campos, "Horario", cmbHora, 3);
            AdicionarCampo(campos, "Tipo de atendimento", cmbTipo, 4);

            btnAgendar = new Button
            {
                Text = "Salvar agendamento",
                AutoSize = true,
                Padding = new Padding(12, 6, 12, 6),
                Anchor = AnchorStyles.Right | AnchorStyles.Bottom
            };
            btnAgendar.Click += btnAgendar_Click;
            campos.Controls.Add(btnAgendar, 4, 2);

            var grupoLista = new GroupBox
            {
                Text = "Agendamentos salvos neste computador",
                Dock = DockStyle.Fill,
                Padding = new Padding(10)
            };
            layout.Controls.Add(grupoLista, 0, 2);

            dgvAgendamentos = new DataGridView
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                AutoGenerateColumns = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                RowHeadersVisible = false
            };
            dgvAgendamentos.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Nome",
                DataPropertyName = nameof(AgendamentoVip.Nome),
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill,
                FillWeight = 20
            });
            dgvAgendamentos.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Contato",
                DataPropertyName = nameof(AgendamentoVip.Contato),
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill,
                FillWeight = 20
            });
            dgvAgendamentos.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Data",
                DataPropertyName = nameof(AgendamentoVip.Data),
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill,
                FillWeight = 12
            });
            dgvAgendamentos.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Horario",
                DataPropertyName = nameof(AgendamentoVip.Hora),
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill,
                FillWeight = 10
            });
            dgvAgendamentos.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Atendimento",
                DataPropertyName = nameof(AgendamentoVip.TipoAtendimento),
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill,
                FillWeight = 30
            });
            dgvAgendamentos.DataSource = listaBinding;
            grupoLista.Controls.Add(dgvAgendamentos);

            var grupoFidelidade = new GroupBox
            {
                Text = "Fidelidade no caixa/balcÃ£o",
                Dock = DockStyle.Fill,
                Padding = new Padding(10)
            };
            layout.Controls.Add(grupoFidelidade, 0, 3);

            var fidelidade = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = false,
                Padding = new Padding(4),
                Anchor = AnchorStyles.Left
            };
            grupoFidelidade.Controls.Add(fidelidade);

            fidelidade.Controls.Add(new Label
            {
                Text = "Valor da compra (R$):",
                AutoSize = true,
                Margin = new Padding(4, 10, 4, 0)
            });
            numValorCompra = new NumericUpDown
            {
                Minimum = 0,
                Maximum = 1000000,
                DecimalPlaces = 2,
                ThousandsSeparator = true,
                Width = 130,
                Margin = new Padding(4, 4, 12, 4)
            };
            fidelidade.Controls.Add(numValorCompra);

            btnPontos = new Button
            {
                Text = "Adicionar pontos",
                AutoSize = true,
                Margin = new Padding(4)
            };
            btnPontos.Click += btnPontos_Click;
            fidelidade.Controls.Add(btnPontos);

            lblSaldoPontos = new Label
            {
                AutoSize = true,
                Font = new Font(Font, FontStyle.Bold),
                Margin = new Padding(20, 10, 4, 0)
            };
            fidelidade.Controls.Add(lblSaldoPontos);
            lblNivel = new Label
            {
                AutoSize = true,
                Margin = new Padding(12, 10, 4, 0)
            };
            fidelidade.Controls.Add(lblNivel);
        }

        private static void AdicionarCampo(TableLayoutPanel layout, string titulo, Control controle, int coluna)
        {
            layout.Controls.Add(new Label
            {
                Text = titulo,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.BottomLeft,
                AutoEllipsis = true
            }, coluna, 0);
            layout.Controls.Add(controle, coluna, 1);
        }

        private bool CarregarDadosOffline()
        {
            try
            {
                EstadoLoja carregado = persistencia.Carregar();
                estado.Agendamentos = carregado.Agendamentos;
                estado.Clientes = carregado.Clientes;

                bool clientesAtualizados = false;
                foreach (AgendamentoVip agendamento in estado.Agendamentos)
                {
                    if (ObterCliente(agendamento.Contato) is null)
                    {
                        estado.Clientes.Add(new ClienteFidelidade
                        {
                            Nome = agendamento.Nome,
                            Contato = agendamento.Contato,
                            Pontos = 0
                        });
                        clientesAtualizados = true;
                    }
                }

                foreach (ClienteFidelidade cliente in estado.Clientes)
                {
                    if (cliente.Pontos > 0 && cliente.UltimaPontuacaoEm is null)
                    {
                        cliente.UltimaPontuacaoEm = DateTime.Now;
                        clientesAtualizados = true;
                    }
                }

                if (ExpirarPontuacoesVencidas())
                {
                    clientesAtualizados = true;
                }

                if (clientesAtualizados)
                {
                    SalvarDadosOffline();
                }

                return true;
            }
            catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or JsonException or InvalidDataException or SqliteException)
            {
                MessageBox.Show(
                    $"NÃ£o foi possÃ­vel carregar ou preparar os dados locais. O salvamento foi desativado para evitar perda de dados.\n\nDetalhes: {ex.Message}",
                    "Erro nos dados locais",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return false;
            }
        }

        private void SalvarDadosOffline()
        {
            persistencia.Salvar(estado);
        }

        private bool ExpirarPontuacoesVencidas()
        {
            bool expirou = false;
            DateTime limite = DateTime.Now.AddDays(-30);
            foreach (ClienteFidelidade cliente in estado.Clientes)
            {
                if (cliente.Pontos > 0 && cliente.UltimaPontuacaoEm is DateTime ultimaPontuacao &&
                    ultimaPontuacao <= limite)
                {
                    cliente.Pontos = 0;
                    cliente.UltimaPontuacaoEm = null;
                    expirou = true;
                }
            }

            return expirou;
        }

        private bool GarantirPontuacoesVigentes()
        {
            ClienteFidelidade[] vencidos = estado.Clientes
                .Where(cliente => cliente.Pontos > 0 && cliente.UltimaPontuacaoEm is DateTime data &&
                    data <= DateTime.Now.AddDays(-30))
                .ToArray();
            if (vencidos.Length == 0)
            {
                return true;
            }

            var valoresAnteriores = vencidos.Select(cliente =>
                (Cliente: cliente, cliente.Pontos, cliente.UltimaPontuacaoEm)).ToArray();
            foreach (ClienteFidelidade cliente in vencidos)
            {
                cliente.Pontos = 0;
                cliente.UltimaPontuacaoEm = null;
            }

            try
            {
                SalvarDadosOffline();
                return true;
            }
            catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or SqliteException)
            {
                foreach (var anterior in valoresAnteriores)
                {
                    anterior.Cliente.Pontos = anterior.Pontos;
                    anterior.Cliente.UltimaPontuacaoEm = anterior.UltimaPontuacaoEm;
                }
                MostrarErroAoSalvar(ex);
                return false;
            }
        }

        private void AtualizarInterface()
        {
            listaBinding.Clear();
            foreach (AgendamentoVip agendamento in estado.Agendamentos)
            {
                listaBinding.Add(agendamento);
            }

            dgvAgendamentos.ClearSelection();
            dgvAgendamentos.CurrentCell = null;
            AtualizarFidelidadeSelecionada();
        }

        private static string NormalizarContato(string contato)
        {
            string digitos = new string(contato.Where(char.IsDigit).ToArray());
            return digitos.Length > 0 ? digitos : contato.Trim().ToUpperInvariant();
        }

        private ClienteFidelidade? ObterCliente(string contato)
        {
            string contatoNormalizado = NormalizarContato(contato);
            return estado.Clientes.FirstOrDefault(cliente =>
                NormalizarContato(cliente.Contato) == contatoNormalizado);
        }

        private void AtualizarFidelidadeSelecionada()
        {
            if (persistenciaDisponivel && !GarantirPontuacoesVigentes())
            {
                return;
            }

            if (dgvAgendamentos.CurrentRow?.DataBoundItem is not AgendamentoVip agendamento)
            {
                clienteSelecionado = null;
                lblSaldoPontos.Text = "Selecione um cadastro";
                lblNivel.Text = "-";
                return;
            }

            clienteSelecionado = ObterCliente(agendamento.Contato);
            if (clienteSelecionado is null)
            {
                lblSaldoPontos.Text = "Cadastro indisponÃ­vel";
                lblNivel.Text = "-";
                return;
            }

            lblSaldoPontos.Text = $"{clienteSelecionado.Pontos} pts";

            lblNivel.Text = ObterCategoria(clienteSelecionado.Pontos);
            lblNivel.ForeColor = clienteSelecionado.Pontos >= 200
                ? Color.FromArgb(212, 175, 55)
                : clienteSelecionado.Pontos >= 100 ? Color.Silver : Color.Brown;
        }

        private static string ObterCategoria(int pontos)
        {
            return pontos >= 200 ? "VIP Ouro" : pontos >= 100 ? "VIP Prata" : "Bronze";
        }

        private void dgvAgendamentos_SelectionChanged(object? sender, EventArgs e)
        {
            AtualizarFidelidadeSelecionada();
        }

        private void dgvAgendamentos_CellFormatting(object? sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0 || dgvAgendamentos.Rows[e.RowIndex].DataBoundItem is not AgendamentoVip agendamento)
            {
                return;
            }

            ClienteFidelidade? cliente = ObterCliente(agendamento.Contato);
            if (cliente is null)
            {
                return;
            }

            if (dgvAgendamentos.Columns[e.ColumnIndex].Name == "colPontuacaoAtual")
            {
                e.Value = $"{cliente.Pontos} pts";
                e.FormattingApplied = true;
            }
            else if (dgvAgendamentos.Columns[e.ColumnIndex].Name == "colCategoria")
            {
                e.Value = ObterCategoria(cliente.Pontos);
                e.FormattingApplied = true;
            }
        }

        private void btnCadastrarCliente_Click(object? sender, EventArgs e)
        {
            using var dialogo = new Form
            {
                Text = "Cadastrar novo cliente",
                FormBorderStyle = FormBorderStyle.FixedDialog,
                StartPosition = FormStartPosition.CenterParent,
                ClientSize = new Size(420, 225),
                MinimizeBox = false,
                MaximizeBox = false,
                ShowInTaskbar = false
            };
            var rotuloNome = new Label { AutoSize = true, Location = new Point(12, 12), Text = "Nome ou apelido:" };
            var entradaNome = new TextBox { Location = new Point(12, 36), Size = new Size(394, 25), MaxLength = 100 };
            var rotuloTelefone = new Label { AutoSize = true, Location = new Point(12, 70), Text = "Nº de telefone:" };
            var entradaTelefone = new MaskedTextBox
            {
                Location = new Point(12, 94),
                Size = new Size(394, 25),
                Mask = "(00)00000-0000",
                PromptChar = '_',
                TextMaskFormat = MaskFormat.IncludeLiterals
            };
            var rotuloPontos = new Label { AutoSize = true, Location = new Point(12, 128), Text = "Pontuação atual:" };
            var entradaPontos = new NumericUpDown
            {
                Location = new Point(130, 126),
                Maximum = int.MaxValue,
                ThousandsSeparator = true,
                Width = 150
            };
            var cadastrar = new Button
            {
                DialogResult = DialogResult.OK,
                Location = new Point(222, 177),
                Size = new Size(88, 30),
                Text = "Cadastrar"
            };
            var cancelar = new Button
            {
                DialogResult = DialogResult.Cancel,
                Location = new Point(318, 177),
                Size = new Size(88, 30),
                Text = "Cancelar"
            };
            dialogo.Controls.AddRange(new Control[]
            {
                rotuloNome, entradaNome, rotuloTelefone, entradaTelefone,
                rotuloPontos, entradaPontos, cadastrar, cancelar
            });
            dialogo.AcceptButton = cadastrar;
            dialogo.CancelButton = cancelar;
            if (dialogo.ShowDialog(this) != DialogResult.OK)
            {
                return;
            }

            string nome = entradaNome.Text.Trim();
            string contato = entradaTelefone.Text.Trim();
            if (string.IsNullOrWhiteSpace(nome) || !entradaTelefone.MaskFull)
            {
                MessageBox.Show(this, "Informe o nome e o telefone do cliente.",
                    "Dados obrigatórios", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string contatoNormalizado = NormalizarContato(contato);
            if (estado.Clientes.Any(cliente => NormalizarContato(cliente.Contato) == contatoNormalizado))
            {
                MessageBox.Show(this, "Já existe um cliente cadastrado com esse telefone.",
                    "Cadastro duplicado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int pontos = decimal.ToInt32(entradaPontos.Value);
            var novoCliente = new ClienteFidelidade
            {
                Nome = nome,
                Contato = contato,
                Pontos = pontos,
                UltimaPontuacaoEm = pontos > 0 ? DateTime.Now : null
            };
            estado.Clientes.Add(novoCliente);
            try
            {
                SalvarDadosOffline();
            }
            catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or SqliteException)
            {
                estado.Clientes.Remove(novoCliente);
                MostrarErroAoSalvar(ex);
                return;
            }

            dgvAgendamentos.Refresh();
            MessageBox.Show(this, $"Cliente {nome} cadastrado com {pontos} pontos.",
                "Cadastro concluído", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnConsultarPontos_Click(object? sender, EventArgs e)
        {
            if (!GarantirPontuacoesVigentes())
            {
                return;
            }

            string? identificador = SolicitarIdentificadorCliente();
            if (string.IsNullOrWhiteSpace(identificador))
            {
                return;
            }

            string nomeBuscado = identificador.Trim();
            string contatoBuscado = NormalizarContato(nomeBuscado);
            ClienteFidelidade[] resultados = estado.Clientes.Where(cliente =>
                string.Equals(cliente.Nome.Trim(), nomeBuscado, StringComparison.OrdinalIgnoreCase) ||
                NormalizarContato(cliente.Contato) == contatoBuscado).ToArray();

            if (resultados.Length == 0)
            {
                MessageBox.Show(this, "Nenhum cadastro corresponde ao nome ou telefone informado.",
                    "Cadastro não encontrado", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (resultados.Length > 1)
            {
                MessageBox.Show(this,
                    "Mais de um cadastro corresponde a esse nome. Consulte usando o telefone para identificar o cliente.",
                    "Consulta ambígua", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            ClienteFidelidade cliente = resultados[0];
            MessageBox.Show(this,
                $"Cliente: {cliente.Nome}\nPontuação atual: {cliente.Pontos} pontos\nCategoria: {ObterCategoria(cliente.Pontos)}",
                "Consulta de pontuação", MessageBoxButtons.OK, MessageBoxIcon.Information);

            if (MessageBox.Show(this, "Adicionar mais pontos?", "Pontuação",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
            {
                return;
            }

            int? pontosAdicionais = SolicitarQuantidadePontos();
            if (pontosAdicionais.HasValue)
            {
                RegistrarPontosAdicionais(cliente, pontosAdicionais.Value);
            }
        }

        private int? SolicitarQuantidadePontos()
        {
            using var dialogo = new Form
            {
                Text = "Adicionar mais pontos",
                FormBorderStyle = FormBorderStyle.FixedDialog,
                StartPosition = FormStartPosition.CenterParent,
                ClientSize = new Size(330, 125),
                MinimizeBox = false,
                MaximizeBox = false,
                ShowInTaskbar = false
            };
            var rotulo = new Label { AutoSize = true, Location = new Point(12, 15), Text = "Quantidade de pontos:" };
            var quantidade = new NumericUpDown
            {
                Location = new Point(170, 12),
                Maximum = int.MaxValue,
                Minimum = 1,
                ThousandsSeparator = true,
                Width = 145
            };
            var adicionar = new Button
            {
                DialogResult = DialogResult.OK,
                Location = new Point(126, 72),
                Size = new Size(88, 30),
                Text = "Adicionar"
            };
            var cancelar = new Button
            {
                DialogResult = DialogResult.Cancel,
                Location = new Point(226, 72),
                Size = new Size(88, 30),
                Text = "Cancelar"
            };
            dialogo.Controls.AddRange(new Control[] { rotulo, quantidade, adicionar, cancelar });
            dialogo.AcceptButton = adicionar;
            dialogo.CancelButton = cancelar;
            return dialogo.ShowDialog(this) == DialogResult.OK ? decimal.ToInt32(quantidade.Value) : null;
        }

        private void RegistrarPontosAdicionais(ClienteFidelidade cliente, int pontos)
        {
            if (pontos <= 0)
            {
                return;
            }

            if (!GarantirPontuacoesVigentes())
            {
                return;
            }

            if (pontos > int.MaxValue - cliente.Pontos)
            {
                MessageBox.Show(this, "O limite máximo de pontos seria ultrapassado.",
                    "Pontuação", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int pontosAnteriores = cliente.Pontos;
            DateTime? dataAnterior = cliente.UltimaPontuacaoEm;
            cliente.Pontos += pontos;
            cliente.UltimaPontuacaoEm = DateTime.Now;
            try
            {
                SalvarDadosOffline();
            }
            catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or SqliteException)
            {
                cliente.Pontos = pontosAnteriores;
                cliente.UltimaPontuacaoEm = dataAnterior;
                MostrarErroAoSalvar(ex);
                return;
            }

            dgvAgendamentos.Refresh();
            AtualizarFidelidadeSelecionada();
            MessageBox.Show(this, $"{pontos} pontos adicionados ao cadastro de {cliente.Nome}.",
                "Pontuação atualizada", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private string? SolicitarIdentificadorCliente()
        {
            using var dialogo = new Form
            {
                Text = "Consultar pontuação",
                FormBorderStyle = FormBorderStyle.FixedDialog,
                StartPosition = FormStartPosition.CenterParent,
                ClientSize = new Size(410, 125),
                MinimizeBox = false,
                MaximizeBox = false,
                ShowInTaskbar = false
            };
            var rotulo = new Label
            {
                AutoSize = true,
                Location = new Point(12, 12),
                Text = "Digite o nome ou telefone do cadastro:"
            };
            var entrada = new TextBox
            {
                Location = new Point(12, 38),
                Size = new Size(386, 25),
                MaxLength = 100
            };
            var consultar = new Button
            {
                DialogResult = DialogResult.OK,
                Location = new Point(216, 78),
                Size = new Size(88, 30),
                Text = "Consultar"
            };
            var cancelar = new Button
            {
                DialogResult = DialogResult.Cancel,
                Location = new Point(310, 78),
                Size = new Size(88, 30),
                Text = "Cancelar"
            };
            dialogo.Controls.AddRange(new Control[] { rotulo, entrada, consultar, cancelar });
            dialogo.AcceptButton = consultar;
            dialogo.CancelButton = cancelar;
            return dialogo.ShowDialog(this) == DialogResult.OK ? entrada.Text : null;
        }

        private void Form1_Shown(object? sender, EventArgs e)
        {
            string dataHoje = DateTime.Today.ToString("yyyy-MM-dd");
            AgendamentoVip[] agendamentosDeHoje = estado.Agendamentos
                .Where(agendamento => agendamento.Data == dataHoje)
                .OrderBy(agendamento => agendamento.Hora)
                .ToArray();
            if (agendamentosDeHoje.Length == 0)
            {
                return;
            }

            string? erroAoRegistrar = null;
            try
            {
                if (!persistencia.RegistrarLembreteDiario(DateTime.Today))
                {
                    return;
                }
            }
            catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or SqliteException)
            {
                erroAoRegistrar = ex.Message;
            }

            string resumo = string.Join(Environment.NewLine, agendamentosDeHoje.Select(agendamento =>
                $"{agendamento.Hora} - {agendamento.Nome} ({agendamento.Contato})"));
            string detalhePersistencia = erroAoRegistrar is null
                ? string.Empty
                : $"\n\nAviso: não foi possível registrar que este lembrete já foi exibido; ele poderá aparecer novamente ao reabrir o aplicativo.\n{erroAoRegistrar}";
            MessageBox.Show(this,
                $"Agendamentos de hoje ({agendamentosDeHoje.Length}):\n\n{resumo}{detalhePersistencia}",
                "Lembrete de agendamentos", MessageBoxButtons.OK,
                erroAoRegistrar is null ? MessageBoxIcon.Information : MessageBoxIcon.Warning);
        }

        private void btnCorrigirAgendamento_Click(object? sender, EventArgs e)
        {
            if (agendamentoEmEdicao is not null)
            {
                agendamentoEmEdicao = null;
                btnAgendar.Text = "Salvar Agendamento";
                btnCorrigirAgendamento.Text = "Corrigir cadastro selecionado";
                txtNome.Clear();
                txtContato.Clear();
                return;
            }

            if (dgvAgendamentos.CurrentRow?.DataBoundItem is not AgendamentoVip selecionado)
            {
                MessageBox.Show("Selecione na lista o cadastro que deseja corrigir.",
                    "Nenhum cadastro selecionado", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            agendamentoEmEdicao = selecionado;
            txtNome.Text = selecionado.Nome;
            txtContato.Text = selecionado.Contato;
            if (DateTime.TryParseExact(selecionado.Data, "yyyy-MM-dd",
                System.Globalization.CultureInfo.InvariantCulture,
                System.Globalization.DateTimeStyles.None, out DateTime data))
            {
                dtpData.Value = data;
            }
            cmbHora.SelectedItem = selecionado.Hora;
            cmbTipo.SelectedItem = selecionado.TipoAtendimento;
            btnAgendar.Text = "Salvar correÃ§Ã£o";
            btnCorrigirAgendamento.Text = "Cancelar correÃ§Ã£o";
            txtNome.Focus();
        }

        private void btnAgendar_Click(object? sender, EventArgs e)
        {
            string nome = txtNome.Text.Trim();
            string contato = txtContato.Text.Trim();
            if (!txtContato.MaskFull)
            {
                MessageBox.Show("Informe o telefone completo no formato (xx)xxxxx-xxxx.",
                    "Telefone incompleto", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string dataFormatada = dtpData.Value.ToString("yyyy-MM-dd");
            string hora = cmbHora.SelectedItem?.ToString() ?? string.Empty;
            string tipo = cmbTipo.SelectedItem?.ToString() ?? string.Empty;

            if (string.IsNullOrWhiteSpace(nome) || string.IsNullOrWhiteSpace(contato) ||
                string.IsNullOrWhiteSpace(hora) || string.IsNullOrWhiteSpace(tipo))
            {
                MessageBox.Show("Preencha nome, contato, horÃ¡rio e tipo de atendimento.",
                    "ValidaÃ§Ã£o", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            AgendamentoVip? emEdicao = agendamentoEmEdicao;
            int agendamentosNaData = estado.Agendamentos.Count(agendamento =>
                agendamento.Data == dataFormatada && !ReferenceEquals(agendamento, emEdicao));
            if (agendamentosNaData >= LimiteAgendamentosPorData)
            {
                MessageBox.Show("Essa data jÃ¡ atingiu o limite de 4 agendamentos.",
                    "Limite diÃ¡rio atingido", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            bool existeConflito = estado.Agendamentos.Any(agendamento =>
                agendamento.Data == dataFormatada && agendamento.Hora == hora &&
                !ReferenceEquals(agendamento, emEdicao));
            if (existeConflito)
            {
                MessageBox.Show("JÃ¡ existe um agendamento reservado nessa data e horÃ¡rio.",
                    "Conflito de agenda", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (emEdicao is not null)
            {
                string nomeAnterior = emEdicao.Nome;
                string contatoAnterior = emEdicao.Contato;
                string dataAnterior = emEdicao.Data;
                string horaAnterior = emEdicao.Hora;
                string tipoAnterior = emEdicao.TipoAtendimento;
                ClienteFidelidade? clienteAnterior = ObterCliente(contatoAnterior);
                ClienteFidelidade? clienteDestino = ObterCliente(contato);
                bool clienteDestinoCriado = clienteDestino is null;
                bool clienteAnteriorMovido = false;
                string? nomeClienteDestinoAnterior = clienteDestino?.Nome;

                if (clienteDestino is null && clienteAnterior is not null &&
                    !estado.Agendamentos.Any(agendamento =>
                        !ReferenceEquals(agendamento, emEdicao) &&
                        NormalizarContato(agendamento.Contato) == NormalizarContato(contatoAnterior)))
                {
                    clienteDestino = clienteAnterior;
                    clienteAnteriorMovido = true;
                    clienteDestino.Nome = nome;
                    clienteDestino.Contato = contato;
                }
                else if (clienteDestino is null)
                {
                    clienteDestino = new ClienteFidelidade { Nome = nome, Contato = contato, Pontos = 0 };
                    estado.Clientes.Add(clienteDestino);
                }
                else
                {
                    clienteDestino.Nome = nome;
                }

                emEdicao.Nome = nome;
                emEdicao.Contato = contato;
                emEdicao.Data = dataFormatada;
                emEdicao.Hora = hora;
                emEdicao.TipoAtendimento = tipo;

                try
                {
                    SalvarDadosOffline();
                }
                catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or SqliteException)
                {
                    emEdicao.Nome = nomeAnterior;
                    emEdicao.Contato = contatoAnterior;
                    emEdicao.Data = dataAnterior;
                    emEdicao.Hora = horaAnterior;
                    emEdicao.TipoAtendimento = tipoAnterior;
                    if (clienteAnteriorMovido && clienteAnterior is not null)
                    {
                        clienteAnterior.Nome = nomeAnterior;
                        clienteAnterior.Contato = contatoAnterior;
                    }
                    else if (clienteDestinoCriado)
                    {
                        estado.Clientes.Remove(clienteDestino);
                    }
                    else
                    {
                        clienteDestino.Nome = nomeClienteDestinoAnterior ?? clienteDestino.Nome;
                    }
                    MostrarErroAoSalvar(ex);
                    return;
                }

                agendamentoEmEdicao = null;
                btnAgendar.Text = "Salvar Agendamento";
                btnCorrigirAgendamento.Text = "Corrigir cadastro selecionado";
                AtualizarInterface();
                SelecionarAgendamento(emEdicao);
                txtNome.Clear();
                txtContato.Clear();
                MessageBox.Show("Cadastro corrigido com sucesso.",
                    "CorreÃ§Ã£o concluÃ­da", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var novoAgendamento = new AgendamentoVip
            {
                Nome = nome,
                Contato = contato,
                Data = dataFormatada,
                Hora = hora,
                TipoAtendimento = tipo
            };
            ClienteFidelidade? cliente = ObterCliente(contato);
            bool novoCliente = cliente is null;
            string? nomeAnteriorCliente = cliente?.Nome;
            if (cliente is null)
            {
                cliente = new ClienteFidelidade { Nome = nome, Contato = contato, Pontos = 0 };
                estado.Clientes.Add(cliente);
            }
            else
            {
                cliente.Nome = nome;
            }
            estado.Agendamentos.Add(novoAgendamento);

            try
            {
                SalvarDadosOffline();
            }
            catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or SqliteException)
            {
                estado.Agendamentos.Remove(novoAgendamento);
                if (novoCliente)
                {
                    estado.Clientes.Remove(cliente);
                }
                else if (nomeAnteriorCliente is not null)
                {
                    cliente.Nome = nomeAnteriorCliente;
                }
                MostrarErroAoSalvar(ex);
                return;
            }

            AtualizarInterface();
            SelecionarAgendamento(novoAgendamento);
            txtNome.Clear();
            txtContato.Clear();
            MessageBox.Show("Agendamento VIP salvo neste computador.",
                "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void SelecionarAgendamento(AgendamentoVip agendamento)
        {
            foreach (DataGridViewRow linha in dgvAgendamentos.Rows)
            {
                if (ReferenceEquals(linha.DataBoundItem, agendamento))
                {
                    dgvAgendamentos.CurrentCell = linha.Cells[0];
                    linha.Selected = true;
                    return;
                }
            }
        }

        private void btnApagarAgendamento_Click(object? sender, EventArgs e)
        {
            if (dgvAgendamentos.CurrentRow?.DataBoundItem is not AgendamentoVip agendamento)
            {
                MessageBox.Show("Selecione na lista o agendamento que deseja apagar.",
                    "Nenhum agendamento selecionado", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            DialogResult confirmacao = MessageBox.Show(
                $"Apagar o agendamento de {agendamento.Nome}, em {agendamento.Data} Ã s {agendamento.Hora}?\n\nOs pontos de fidelidade do cliente serÃ£o mantidos.",
                "Confirmar exclusÃ£o",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);
            if (confirmacao != DialogResult.Yes)
            {
                return;
            }

            int indice = estado.Agendamentos.IndexOf(agendamento);
            if (indice < 0)
            {
                MessageBox.Show("O agendamento selecionado nÃ£o foi encontrado nos dados salvos.",
                    "Erro ao apagar", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            estado.Agendamentos.RemoveAt(indice);
            try
            {
                SalvarDadosOffline();
            }
            catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or SqliteException)
            {
                estado.Agendamentos.Insert(indice, agendamento);
                MostrarErroAoSalvar(ex);
                return;
            }

            AtualizarInterface();
            MessageBox.Show("Agendamento apagado. Os pontos de fidelidade foram mantidos.",
                "ExclusÃ£o concluÃ­da", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnPontos_Click(object? sender, EventArgs e)
        {
            ClienteFidelidade? cliente = clienteSelecionado;
            if (cliente is null)
            {
                MessageBox.Show("Selecione o cadastro do cliente na lista de agendamentos antes de registrar pontos.",
                    "Selecione um cadastro", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (!GarantirPontuacoesVigentes())
            {
                return;
            }

            decimal valor = numValorCompra.Value;
            if (valor < 10m)
            {
                MessageBox.Show("Compras a partir de R$ 10,00 acumulam pontos.",
                    "Fidelidade", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            int ganhos = (int)(Math.Floor(valor / 10m) * 10m);
            if (ganhos > int.MaxValue - cliente.Pontos)
            {
                MessageBox.Show("O saldo mÃ¡ximo de pontos seria ultrapassado.",
                    "Fidelidade", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int pontosAnteriores = cliente.Pontos;
            DateTime? dataPontuacaoAnterior = cliente.UltimaPontuacaoEm;
            cliente.Pontos += ganhos;
            cliente.UltimaPontuacaoEm = DateTime.Now;
            try
            {
                SalvarDadosOffline();
            }
            catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or SqliteException)
            {
                cliente.Pontos = pontosAnteriores;
                cliente.UltimaPontuacaoEm = dataPontuacaoAnterior;
                MostrarErroAoSalvar(ex);
                return;
            }

            AtualizarFidelidadeSelecionada();
            numValorCompra.Value = 0;
            MessageBox.Show($"+{ganhos} pontos adicionados ao cadastro de {cliente.Nome}!",
                "Fidelidade", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private static void MostrarErroAoSalvar(Exception ex)
        {
            MessageBox.Show(
                $"NÃ£o foi possÃ­vel salvar os dados offline. A alteraÃ§Ã£o foi desfeita.\n\nDetalhes: {ex.Message}",
                "Erro ao salvar",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }

        private void dtpData_ValueChanged(object sender, EventArgs e)
        {

        }

        private void cmbHora_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }

    public class EstadoLoja
    {
        public List<AgendamentoVip> Agendamentos { get; set; } = new();
        public List<ClienteFidelidade> Clientes { get; set; } = new();
    }

    public class ClienteFidelidade
    {
        public string Nome { get; set; } = string.Empty;
        public string Contato { get; set; } = string.Empty;
        public int Pontos { get; set; }
        public DateTime? UltimaPontuacaoEm { get; set; }
    }

    public class AgendamentoVip
    {
        public string Nome { get; set; } = string.Empty;
        public string Contato { get; set; } = string.Empty;
        public string Data { get; set; } = string.Empty;
        public string Hora { get; set; } = string.Empty;
        public string TipoAtendimento { get; set; } = string.Empty;
    }
}
