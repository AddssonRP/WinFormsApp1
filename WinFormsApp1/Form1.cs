using System.ComponentModel;
using System.Text.Json;

namespace WinFormsApp1
{
    public partial class Form1 : Form
    {
        private const int LimiteAgendamentosPorData = 4;

        private readonly string caminhoArquivo = Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory,
            "dados_desejos_offline.json");
        private readonly EstadoLoja estado = new();
        private readonly BindingList<AgendamentoVip> listaBinding = new();
        private ClienteFidelidade? clienteSelecionado;
        private AgendamentoVip? agendamentoEmEdicao;
        private Label lblPontosLegados = null!;
        private Button btnVincularSaldoLegado = null!;
        private Button btnApagarAgendamento = null!;
        private Button btnCorrigirAgendamento = null!;
        private bool persistenciaDisponivel;

        public Form1()
        {
            InitializeComponent();
            ConfigurarControles();
            AplicarTema();
            persistenciaDisponivel = CarregarDadosOffline();
            btnAgendar.Enabled = persistenciaDisponivel;
            btnPontos.Enabled = persistenciaDisponivel;
            btnVincularSaldoLegado.Enabled = persistenciaDisponivel;
            btnApagarAgendamento.Enabled = persistenciaDisponivel;
            btnCorrigirAgendamento.Enabled = persistenciaDisponivel;
            AtualizarInterface();
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
                "SeleÃ§Ã£o de presentes e kits"
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
            dgvAgendamentos.DataSource = listaBinding;
            dgvAgendamentos.SelectionChanged += dgvAgendamentos_SelectionChanged;
            dgvAgendamentos.Dock = DockStyle.None;
            dgvAgendamentos.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            dgvAgendamentos.Location = new Point(4, 24);
            dgvAgendamentos.Size = new Size(grpLista.ClientSize.Width - 8, 156);

            lblPontosLegados = new Label
            {
                AutoSize = true,
                Font = new Font("Arial", 8.5F),
                Location = new Point(21, 112),
                Name = "lblPontosLegados"
            };
            grpFidelidade.Controls.Add(lblPontosLegados);

            btnVincularSaldoLegado = new Button
            {
                Font = new Font("Arial", 8.5F, FontStyle.Bold),
                Location = new Point(21, 298),
                Name = "btnVincularSaldoLegado",
                Size = new Size(407, 30),
                Text = "Vincular saldo antigo ao cadastro selecionado",
                UseVisualStyleBackColor = true
            };
            btnVincularSaldoLegado.Click += btnVincularSaldoLegado_Click;
            grpFidelidade.Controls.Add(btnVincularSaldoLegado);

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
            ConfigurarBotao(btnCorrigirAgendamento, dourado, preto, vermelho);
            ConfigurarBotao(btnApagarAgendamento, vermelho, branco, dourado);
            ConfigurarBotao(btnVincularSaldoLegado, cinzaEscuro, branco, dourado);
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
            txtContato = new TextBox { Dock = DockStyle.Fill, MaxLength = 40 };
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
                "SeleÃ§Ã£o de presentes e kits"
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
                bool arquivoExistia = File.Exists(caminhoArquivo);
                if (arquivoExistia)
                {
                    string json = File.ReadAllText(caminhoArquivo);
                    EstadoLoja? carregado = JsonSerializer.Deserialize<EstadoLoja>(json);
                    if (carregado is null || carregado.Agendamentos is null || carregado.Clientes is null ||
                        carregado.PontosFidelidade < 0 ||
                        carregado.Clientes.Any(cliente => cliente is null || cliente.Pontos < 0 || string.IsNullOrWhiteSpace(cliente.Contato)))
                    {
                        throw new InvalidDataException("O arquivo de dados nÃ£o contÃ©m um cadastro vÃ¡lido.");
                    }

                    estado.PontosFidelidade = carregado.PontosFidelidade;
                    estado.Agendamentos = carregado.Agendamentos;
                    estado.Clientes = carregado.Clientes;
                }

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

                if (clientesAtualizados || !arquivoExistia)
                {
                    SalvarDadosOffline();
                }

                return true;
            }
            catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or JsonException or InvalidDataException)
            {
                MessageBox.Show(
                    $"NÃ£o foi possÃ­vel carregar ou preparar os dados offline. O salvamento foi desativado para evitar perda de dados.\n\nDetalhes: {ex.Message}",
                    "Erro nos dados locais",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return false;
            }
        }

        private void SalvarDadosOffline()
        {
            var opcoes = new JsonSerializerOptions { WriteIndented = true };
            string json = JsonSerializer.Serialize(estado, opcoes);
            string arquivoTemporario = caminhoArquivo + ".tmp";
            File.WriteAllText(arquivoTemporario, json);
            File.Move(arquivoTemporario, caminhoArquivo, true);
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
            lblPontosLegados.Text = $"Saldo antigo sem vinculo: {estado.PontosFidelidade} pts";
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

            lblSaldoPontos.Text = $"Saldo: {clienteSelecionado.Pontos} pts";

            if (clienteSelecionado.Pontos >= 200)
            {
                lblNivel.Text = "VIP Ouro";
                lblNivel.ForeColor = Color.FromArgb(212, 175, 55);
            }
            else if (clienteSelecionado.Pontos >= 100)
            {
                lblNivel.Text = "VIP Prata";
                lblNivel.ForeColor = Color.Silver;
            }
            else
            {
                lblNivel.Text = "Bronze";
                lblNivel.ForeColor = Color.Brown;
            }
        }

        private void dgvAgendamentos_SelectionChanged(object? sender, EventArgs e)
        {
            AtualizarFidelidadeSelecionada();
        }

        private void btnVincularSaldoLegado_Click(object? sender, EventArgs e)
        {
            ClienteFidelidade? cliente = clienteSelecionado;
            if (cliente is null)
            {
                MessageBox.Show("Selecione um cadastro na lista de agendamentos antes de vincular o saldo antigo.",
                    "Selecione um cadastro", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (estado.PontosFidelidade <= 0)
            {
                MessageBox.Show("NÃ£o hÃ¡ saldo antigo sem vÃ­nculo para transferir.",
                    "Fidelidade", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (MessageBox.Show(
                $"Deseja transferir os {estado.PontosFidelidade} pontos antigos para {cliente.Nome}?",
                "Confirmar transferÃªncia de saldo",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question) != DialogResult.Yes)
            {
                return;
            }

            if (estado.PontosFidelidade > int.MaxValue - cliente.Pontos)
            {
                MessageBox.Show("O saldo mÃ¡ximo de pontos seria ultrapassado.",
                    "Fidelidade", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int saldoAnterior = estado.PontosFidelidade;
            int pontosClienteAnterior = cliente.Pontos;
            estado.PontosFidelidade = 0;
            cliente.Pontos += saldoAnterior;

            try
            {
                SalvarDadosOffline();
            }
            catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
            {
                estado.PontosFidelidade = saldoAnterior;
                cliente.Pontos = pontosClienteAnterior;
                MostrarErroAoSalvar(ex);
                return;
            }

            lblPontosLegados.Text = $"Saldo antigo sem vinculo: {estado.PontosFidelidade} pts";
            AtualizarFidelidadeSelecionada();
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
                catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
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
            catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
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
            catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
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

            cliente.Pontos += ganhos;
            try
            {
                SalvarDadosOffline();
            }
            catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
            {
                cliente.Pontos -= ganhos;
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
        public int PontosFidelidade { get; set; }
        public List<AgendamentoVip> Agendamentos { get; set; } = new();
        public List<ClienteFidelidade> Clientes { get; set; } = new();
    }

    public class ClienteFidelidade
    {
        public string Nome { get; set; } = string.Empty;
        public string Contato { get; set; } = string.Empty;
        public int Pontos { get; set; }
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
