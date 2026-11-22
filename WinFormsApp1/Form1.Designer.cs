namespace WinFormsApp1
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null!;

        private System.Windows.Forms.Label lblTitulo = null!;
        private System.Windows.Forms.Label lblSubtitulo = null!;
        private System.Windows.Forms.GroupBox grpFidelidade = null!;
        private System.Windows.Forms.Label lblRotuloSaldo = null!;
        private System.Windows.Forms.Label lblSaldoPontos = null!;
        private System.Windows.Forms.Label lblRotuloNivel = null!;
        private System.Windows.Forms.Label lblNivel = null!;
        private System.Windows.Forms.Label lblValorCompra = null!;
        private System.Windows.Forms.NumericUpDown numValorCompra = null!;
        private System.Windows.Forms.Button btnPontos = null!;
        private System.Windows.Forms.GroupBox grpAgendamento = null!;
        private System.Windows.Forms.Label lblNome = null!;
        private System.Windows.Forms.TextBox txtNome = null!;
        private System.Windows.Forms.Label lblContato = null!;
        private System.Windows.Forms.MaskedTextBox txtContato = null!;
        private System.Windows.Forms.Label lblData = null!;
        private System.Windows.Forms.DateTimePicker dtpData = null!;
        private System.Windows.Forms.Label lblHora = null!;
        private System.Windows.Forms.ComboBox cmbHora = null!;
        private System.Windows.Forms.Label lblTipo = null!;
        private System.Windows.Forms.ComboBox cmbTipo = null!;
        private System.Windows.Forms.Button btnAgendar = null!;
        private System.Windows.Forms.GroupBox grpLista = null!;
        private System.Windows.Forms.DataGridView dgvAgendamentos = null!;

        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null)
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            DataGridViewCellStyle dataGridViewCellStyle2 = new DataGridViewCellStyle();
            lblTitulo = new Label();
            lblSubtitulo = new Label();
            grpFidelidade = new GroupBox();
            lblRotuloSaldo = new Label();
            lblSaldoPontos = new Label();
            lblRotuloNivel = new Label();
            lblNivel = new Label();
            lblValorCompra = new Label();
            numValorCompra = new NumericUpDown();
            btnPontos = new Button();
            grpAgendamento = new GroupBox();
            lblNome = new Label();
            txtNome = new TextBox();
            lblContato = new Label();
            txtContato = new MaskedTextBox();
            lblData = new Label();
            dtpData = new DateTimePicker();
            lblHora = new Label();
            cmbHora = new ComboBox();
            lblTipo = new Label();
            cmbTipo = new ComboBox();
            btnAgendar = new Button();
            grpLista = new GroupBox();
            dgvAgendamentos = new DataGridView();
            grpFidelidade.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)numValorCompra).BeginInit();
            grpAgendamento.SuspendLayout();
            grpLista.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvAgendamentos).BeginInit();
            SuspendLayout();
            // 
            // lblTitulo
            // 
            lblTitulo.AutoSize = true;
            lblTitulo.Font = new Font("Arial", 14F, FontStyle.Bold);
            lblTitulo.Location = new Point(21, 18);
            lblTitulo.Margin = new Padding(4, 0, 4, 0);
            lblTitulo.Name = "lblTitulo";
            lblTitulo.Size = new Size(420, 29);
            lblTitulo.TabIndex = 0;
            lblTitulo.Text = "Desejos Sexshop - App Vers\u00e3o Offline";
            // 
            // lblSubtitulo
            // 
            lblSubtitulo.AutoSize = true;
            lblSubtitulo.Font = new Font("Arial", 9.5F);
            lblSubtitulo.Location = new Point(24, 55);
            lblSubtitulo.Margin = new Padding(4, 0, 4, 0);
            lblSubtitulo.Name = "lblSubtitulo";
            lblSubtitulo.Size = new Size(460, 18);
            lblSubtitulo.TabIndex = 1;
            lblSubtitulo.Text = "Fidelidade e Agendamento VIP ('Portas Fechadas') - Colinas/MA";
            // 
            // grpFidelidade
            // 
            grpFidelidade.Controls.Add(lblRotuloSaldo);
            grpFidelidade.Controls.Add(lblSaldoPontos);
            grpFidelidade.Controls.Add(lblRotuloNivel);
            grpFidelidade.Controls.Add(lblNivel);
            grpFidelidade.Controls.Add(lblValorCompra);
            grpFidelidade.Controls.Add(numValorCompra);
            grpFidelidade.Controls.Add(btnPontos);
            grpFidelidade.Font = new Font("Arial", 9.5F, FontStyle.Bold);
            grpFidelidade.Location = new Point(24, 100);
            grpFidelidade.Margin = new Padding(4, 5, 4, 5);
            grpFidelidade.Name = "grpFidelidade";
            grpFidelidade.Padding = new Padding(4, 5, 4, 5);
            grpFidelidade.Size = new Size(453, 338);
            grpFidelidade.TabIndex = 2;
            grpFidelidade.TabStop = false;
            grpFidelidade.Text = "Clube de Fidelidade";
            // 
            // lblRotuloSaldo
            // 
            lblRotuloSaldo.AutoSize = true;
            lblRotuloSaldo.Font = new Font("Arial", 9F);
            lblRotuloSaldo.Location = new Point(21, 49);
            lblRotuloSaldo.Margin = new Padding(4, 0, 4, 0);
            lblRotuloSaldo.Name = "lblRotuloSaldo";
            lblRotuloSaldo.Size = new Size(112, 17);
            lblRotuloSaldo.TabIndex = 0;
            lblRotuloSaldo.Text = "Pontuação atual:";
            // 
            // lblSaldoPontos
            // 
            lblSaldoPontos.AutoSize = true;
            lblSaldoPontos.Font = new Font("Arial", 13F, FontStyle.Bold);
            lblSaldoPontos.Location = new Point(21, 77);
            lblSaldoPontos.Margin = new Padding(4, 0, 4, 0);
            lblSaldoPontos.Name = "lblSaldoPontos";
            lblSaldoPontos.Size = new Size(86, 26);
            lblSaldoPontos.TabIndex = 1;
            lblSaldoPontos.Text = "120 pts";
            // 
            // lblRotuloNivel
            // 
            lblRotuloNivel.AutoSize = true;
            lblRotuloNivel.Font = new Font("Arial", 9F);
            lblRotuloNivel.Location = new Point(253, 49);
            lblRotuloNivel.Margin = new Padding(4, 0, 4, 0);
            lblRotuloNivel.Name = "lblRotuloNivel";
            lblRotuloNivel.Size = new Size(75, 17);
            lblRotuloNivel.TabIndex = 2;
            lblRotuloNivel.Text = "Categoria:";
            // 
            // lblNivel
            // 
            lblNivel.AutoSize = true;
            lblNivel.Font = new Font("Arial", 11F, FontStyle.Bold);
            lblNivel.Location = new Point(253, 80);
            lblNivel.Margin = new Padding(4, 0, 4, 0);
            lblNivel.Name = "lblNivel";
            lblNivel.Size = new Size(95, 22);
            lblNivel.TabIndex = 3;
            lblNivel.Text = "Bronze";
            // 
            // lblValorCompra
            // 
            lblValorCompra.AutoSize = true;
            lblValorCompra.Font = new Font("Arial", 9F);
            lblValorCompra.Location = new Point(21, 146);
            lblValorCompra.Margin = new Padding(4, 0, 4, 0);
            lblValorCompra.Name = "lblValorCompra";
            lblValorCompra.Size = new Size(151, 17);
            lblValorCompra.TabIndex = 4;
            lblValorCompra.Text = "Valor da compra (R$):";
            // 
            // numValorCompra
            // 
            numValorCompra.DecimalPlaces = 2;
            numValorCompra.Font = new Font("Arial", 9.5F);
            numValorCompra.Location = new Point(21, 177);
            numValorCompra.Margin = new Padding(4, 5, 4, 5);
            numValorCompra.Maximum = new decimal(new int[] { 100000, 0, 0, 0 });
            numValorCompra.Name = "numValorCompra";
            numValorCompra.Size = new Size(193, 26);
            numValorCompra.TabIndex = 5;
            // 
            // btnPontos
            // 
            btnPontos.Font = new Font("Arial", 9.5F, FontStyle.Bold);
            btnPontos.Location = new Point(21, 238);
            btnPontos.Margin = new Padding(4, 5, 4, 5);
            btnPontos.Name = "btnPontos";
            btnPontos.Size = new Size(407, 38);
            btnPontos.TabIndex = 6;
            btnPontos.Text = "Registrar pontos";
            btnPontos.UseVisualStyleBackColor = true;
            btnPontos.Click += btnPontos_Click;
            // 
            // grpAgendamento
            // 
            grpAgendamento.Controls.Add(lblNome);
            grpAgendamento.Controls.Add(txtNome);
            grpAgendamento.Controls.Add(lblContato);
            grpAgendamento.Controls.Add(txtContato);
            grpAgendamento.Controls.Add(lblData);
            grpAgendamento.Controls.Add(dtpData);
            grpAgendamento.Controls.Add(lblHora);
            grpAgendamento.Controls.Add(cmbHora);
            grpAgendamento.Controls.Add(lblTipo);
            grpAgendamento.Controls.Add(cmbTipo);
            grpAgendamento.Controls.Add(btnAgendar);
            grpAgendamento.Font = new Font("Arial", 9.5F, FontStyle.Bold);
            grpAgendamento.Location = new Point(499, 100);
            grpAgendamento.Margin = new Padding(4, 5, 4, 5);
            grpAgendamento.Name = "grpAgendamento";
            grpAgendamento.Padding = new Padding(4, 5, 4, 5);
            grpAgendamento.Size = new Size(627, 338);
            grpAgendamento.TabIndex = 3;
            grpAgendamento.TabStop = false;
            grpAgendamento.Text = "Agendamento VIP ('Portas Fechadas')";
            // 
            // lblNome
            // 
            lblNome.AutoSize = true;
            lblNome.Font = new Font("Arial", 9F);
            lblNome.Location = new Point(21, 43);
            lblNome.Margin = new Padding(4, 0, 4, 0);
            lblNome.Name = "lblNome";
            lblNome.Size = new Size(147, 17);
            lblNome.TabIndex = 0;
            lblNome.Text = "Nome ou apelido VIP:";
            // 
            // txtNome
            // 
            txtNome.Font = new Font("Arial", 9.5F);
            txtNome.Location = new Point(21, 71);
            txtNome.Margin = new Padding(4, 5, 4, 5);
            txtNome.Name = "txtNome";
            txtNome.Size = new Size(279, 26);
            txtNome.TabIndex = 1;
            // 
            // lblContato
            // 
            lblContato.AutoSize = true;
            lblContato.Font = new Font("Arial", 9F);
            lblContato.Location = new Point(323, 43);
            lblContato.Margin = new Padding(4, 0, 4, 0);
            lblContato.Name = "lblContato";
            lblContato.Size = new Size(146, 17);
            lblContato.TabIndex = 2;
            lblContato.Text = "WhatsApp / Telefone:";
            // 
            // txtContato
            // 
            txtContato.Font = new Font("Arial", 9.5F);
            txtContato.Location = new Point(323, 71);
            txtContato.Margin = new Padding(4, 5, 4, 5);
            txtContato.Mask = "(00)00000-0000";
            txtContato.Name = "txtContato";
            txtContato.PromptChar = '_';
            txtContato.Size = new Size(279, 26);
            txtContato.TabIndex = 3;
            // 
            // lblData
            // 
            lblData.AutoSize = true;
            lblData.Font = new Font("Arial", 9F);
            lblData.Location = new Point(21, 120);
            lblData.Margin = new Padding(4, 0, 4, 0);
            lblData.Name = "lblData";
            lblData.Size = new Size(43, 17);
            lblData.TabIndex = 4;
            lblData.Text = "Data:";
            // 
            // dtpData
            // 
            dtpData.Font = new Font("Arial", 9.5F);
            dtpData.Format = DateTimePickerFormat.Short;
            dtpData.Location = new Point(21, 148);
            dtpData.Margin = new Padding(4, 5, 4, 5);
            dtpData.Name = "dtpData";
            dtpData.Size = new Size(172, 26);
            dtpData.TabIndex = 5;
            dtpData.ValueChanged += dtpData_ValueChanged;
            // 
            // lblHora
            // 
            lblHora.AutoSize = true;
            lblHora.Font = new Font("Arial", 9F);
            lblHora.Location = new Point(211, 120);
            lblHora.Margin = new Padding(4, 0, 4, 0);
            lblHora.Name = "lblHora";
            lblHora.Size = new Size(59, 17);
            lblHora.TabIndex = 6;
            lblHora.Text = "Horario:";
            // 
            // cmbHora
            // 
            cmbHora.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbHora.Font = new Font("Arial", 9.5F);
            cmbHora.Location = new Point(211, 148);
            cmbHora.Margin = new Padding(4, 5, 4, 5);
            cmbHora.Name = "cmbHora";
            cmbHora.Size = new Size(132, 26);
            cmbHora.TabIndex = 7;
            cmbHora.SelectedIndexChanged += cmbHora_SelectedIndexChanged;
            // 
            // lblTipo
            // 
            lblTipo.AutoSize = true;
            lblTipo.Font = new Font("Arial", 9F);
            lblTipo.Location = new Point(21, 197);
            lblTipo.Margin = new Padding(4, 0, 4, 0);
            lblTipo.Name = "lblTipo";
            lblTipo.Size = new Size(143, 17);
            lblTipo.TabIndex = 8;
            lblTipo.Text = "Tipo de atendimento:";
            // 
            // cmbTipo
            // 
            cmbTipo.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbTipo.Font = new Font("Arial", 9.5F);
            cmbTipo.Location = new Point(21, 225);
            cmbTipo.Margin = new Padding(4, 5, 4, 5);
            cmbTipo.Name = "cmbTipo";
            cmbTipo.Size = new Size(321, 26);
            cmbTipo.TabIndex = 9;
            // 
            // btnAgendar
            // 
            btnAgendar.Font = new Font("Arial", 9.5F, FontStyle.Bold);
            btnAgendar.Location = new Point(363, 206);
            btnAgendar.Margin = new Padding(4, 5, 4, 5);
            btnAgendar.Name = "btnAgendar";
            btnAgendar.Size = new Size(240, 65);
            btnAgendar.TabIndex = 10;
            btnAgendar.Text = "Salvar Agendamento";
            btnAgendar.UseVisualStyleBackColor = true;
            btnAgendar.Click += btnAgendar_Click;
            // 
            // grpLista
            // 
            grpLista.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            grpLista.Controls.Add(dgvAgendamentos);
            grpLista.Font = new Font("Arial", 9.5F, FontStyle.Bold);
            grpLista.Location = new Point(24, 458);
            grpLista.Margin = new Padding(4, 5, 4, 5);
            grpLista.Name = "grpLista";
            grpLista.Padding = new Padding(4, 5, 4, 5);
            grpLista.Size = new Size(1285, 260);
            grpLista.TabIndex = 4;
            grpLista.TabStop = false;
            grpLista.Text = "Registro de Agendamentos (máximo 4 por data)";
            // 
            // dgvAgendamentos
            // 
            dgvAgendamentos.AllowUserToAddRows = false;
            dgvAgendamentos.AllowUserToDeleteRows = false;
            dgvAgendamentos.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridViewCellStyle2.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = SystemColors.Control;
            dataGridViewCellStyle2.Font = new Font("Arial", 10F, FontStyle.Bold);
            dataGridViewCellStyle2.ForeColor = SystemColors.WindowText;
            dataGridViewCellStyle2.SelectionBackColor = SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = DataGridViewTriState.True;
            dgvAgendamentos.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
            dgvAgendamentos.ColumnHeadersHeight = 36;
            dgvAgendamentos.Dock = DockStyle.Fill;
            dgvAgendamentos.Font = new Font("Arial", 11F);
            dgvAgendamentos.Location = new Point(4, 24);
            dgvAgendamentos.Margin = new Padding(4, 5, 4, 5);
            dgvAgendamentos.Name = "dgvAgendamentos";
            dgvAgendamentos.ReadOnly = true;
            dgvAgendamentos.RowHeadersVisible = false;
            dgvAgendamentos.RowHeadersWidth = 51;
            dgvAgendamentos.RowTemplate.Height = 30;
            dgvAgendamentos.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvAgendamentos.Size = new Size(1277, 534);
            dgvAgendamentos.TabIndex = 0;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1333, 720);
            Controls.Add(grpLista);
            Controls.Add(grpAgendamento);
            Controls.Add(grpFidelidade);
            Controls.Add(lblSubtitulo);
            Controls.Add(lblTitulo);
            Margin = new Padding(4, 5, 4, 5);
            MinimumSize = new Size(1165, 720);
            Name = "Form1";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Desejos Sexshop - App Vers\u00e3o Offline";
            grpFidelidade.ResumeLayout(false);
            grpFidelidade.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)numValorCompra).EndInit();
            grpAgendamento.ResumeLayout(false);
            grpAgendamento.PerformLayout();
            grpLista.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dgvAgendamentos).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }
    }
}
