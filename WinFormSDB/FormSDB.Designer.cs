namespace WinFormSDB
{
    partial class FormSDB
    {
        /// <summary>
        /// Обязательная переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.dgvClientsStatistics = new System.Windows.Forms.DataGridView();
            this.cmbSearch = new System.Windows.Forms.ComboBox();
            this.lblSearch = new System.Windows.Forms.Label();
            this.lblStatus = new System.Windows.Forms.Label();
            this.cmbStatus = new System.Windows.Forms.ComboBox();
            this.lblLanguage = new System.Windows.Forms.Label();
            this.cmbLanguage = new System.Windows.Forms.ComboBox();
            this.lblClientBook = new System.Windows.Forms.Label();
            this.txbClientBook = new System.Windows.Forms.TextBox();
            this.btnSearch = new System.Windows.Forms.Button();
            this.lblClientsStatistics = new System.Windows.Forms.Label();
            this.btnInfoClientCS = new System.Windows.Forms.Button();
            this.btnInfoBooks = new System.Windows.Forms.Button();
            this.lblInfoClient = new System.Windows.Forms.Label();
            this.dgvClientInfo = new System.Windows.Forms.DataGridView();
            this.lblLevelLanguage = new System.Windows.Forms.Label();
            this.dgvLevelLanguage = new System.Windows.Forms.DataGridView();
            this.lblBooks = new System.Windows.Forms.Label();
            this.dgvBooks = new System.Windows.Forms.DataGridView();
            this.btnClientStatistics = new System.Windows.Forms.Button();
            this.btnInfoClientB = new System.Windows.Forms.Button();
            this.numPeriod = new System.Windows.Forms.NumericUpDown();
            this.lblPeriod = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.dgvClientsStatistics)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvClientInfo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvLevelLanguage)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvBooks)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numPeriod)).BeginInit();
            this.SuspendLayout();
            // 
            // dgvClientsStatistics
            // 
            this.dgvClientsStatistics.AllowUserToAddRows = false;
            this.dgvClientsStatistics.AllowUserToDeleteRows = false;
            this.dgvClientsStatistics.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvClientsStatistics.Location = new System.Drawing.Point(12, 65);
            this.dgvClientsStatistics.Name = "dgvClientsStatistics";
            this.dgvClientsStatistics.ReadOnly = true;
            this.dgvClientsStatistics.RowHeadersVisible = false;
            this.dgvClientsStatistics.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvClientsStatistics.Size = new System.Drawing.Size(423, 330);
            this.dgvClientsStatistics.TabIndex = 0;
            // 
            // cmbSearch
            // 
            this.cmbSearch.BackColor = System.Drawing.SystemColors.Info;
            this.cmbSearch.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbSearch.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmbSearch.FormattingEnabled = true;
            this.cmbSearch.Items.AddRange(new object[] {
            "Клиентам",
            "Книгам"});
            this.cmbSearch.Location = new System.Drawing.Point(158, 25);
            this.cmbSearch.Name = "cmbSearch";
            this.cmbSearch.Size = new System.Drawing.Size(85, 21);
            this.cmbSearch.TabIndex = 1;
            // 
            // lblSearch
            // 
            this.lblSearch.AutoSize = true;
            this.lblSearch.Location = new System.Drawing.Point(155, 9);
            this.lblSearch.Name = "lblSearch";
            this.lblSearch.Size = new System.Drawing.Size(54, 13);
            this.lblSearch.TabIndex = 2;
            this.lblSearch.Text = "Поиск по";
            // 
            // lblStatus
            // 
            this.lblStatus.AutoSize = true;
            this.lblStatus.Location = new System.Drawing.Point(262, 9);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(92, 13);
            this.lblStatus.TabIndex = 3;
            this.lblStatus.Text = "Статус подписки";
            // 
            // cmbStatus
            // 
            this.cmbStatus.BackColor = System.Drawing.SystemColors.Info;
            this.cmbStatus.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbStatus.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmbStatus.FormattingEnabled = true;
            this.cmbStatus.Location = new System.Drawing.Point(265, 25);
            this.cmbStatus.Name = "cmbStatus";
            this.cmbStatus.Size = new System.Drawing.Size(106, 21);
            this.cmbStatus.TabIndex = 4;
            // 
            // lblLanguage
            // 
            this.lblLanguage.AutoSize = true;
            this.lblLanguage.Location = new System.Drawing.Point(387, 9);
            this.lblLanguage.Name = "lblLanguage";
            this.lblLanguage.Size = new System.Drawing.Size(35, 13);
            this.lblLanguage.TabIndex = 5;
            this.lblLanguage.Text = "Язык";
            // 
            // cmbLanguage
            // 
            this.cmbLanguage.BackColor = System.Drawing.SystemColors.Info;
            this.cmbLanguage.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbLanguage.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmbLanguage.FormattingEnabled = true;
            this.cmbLanguage.Items.AddRange(new object[] {
            "Все",
            "Русский",
            "Английский",
            "Немецкий",
            "Итальянский",
            "Испанский"});
            this.cmbLanguage.Location = new System.Drawing.Point(390, 25);
            this.cmbLanguage.Name = "cmbLanguage";
            this.cmbLanguage.Size = new System.Drawing.Size(117, 21);
            this.cmbLanguage.TabIndex = 6;
            // 
            // lblClientBook
            // 
            this.lblClientBook.AutoSize = true;
            this.lblClientBook.Location = new System.Drawing.Point(528, 9);
            this.lblClientBook.Name = "lblClientBook";
            this.lblClientBook.Size = new System.Drawing.Size(88, 13);
            this.lblClientBook.TabIndex = 7;
            this.lblClientBook.Text = "Адресс клиента";
            // 
            // txbClientBook
            // 
            this.txbClientBook.BackColor = System.Drawing.SystemColors.Info;
            this.txbClientBook.Location = new System.Drawing.Point(531, 25);
            this.txbClientBook.Name = "txbClientBook";
            this.txbClientBook.Size = new System.Drawing.Size(119, 20);
            this.txbClientBook.TabIndex = 8;
            // 
            // btnSearch
            // 
            this.btnSearch.Location = new System.Drawing.Point(822, 23);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(75, 23);
            this.btnSearch.TabIndex = 9;
            this.btnSearch.Text = "Поиск";
            this.btnSearch.UseVisualStyleBackColor = true;
            // 
            // lblClientsStatistics
            // 
            this.lblClientsStatistics.AutoSize = true;
            this.lblClientsStatistics.Location = new System.Drawing.Point(155, 49);
            this.lblClientsStatistics.Name = "lblClientsStatistics";
            this.lblClientsStatistics.Size = new System.Drawing.Size(132, 13);
            this.lblClientsStatistics.TabIndex = 10;
            this.lblClientsStatistics.Text = "Статистика по клиентам";
            // 
            // btnInfoClientCS
            // 
            this.btnInfoClientCS.Enabled = false;
            this.btnInfoClientCS.Location = new System.Drawing.Point(83, 400);
            this.btnInfoClientCS.Name = "btnInfoClientCS";
            this.btnInfoClientCS.Size = new System.Drawing.Size(86, 35);
            this.btnInfoClientCS.TabIndex = 11;
            this.btnInfoClientCS.Text = "Информация о клиенте";
            this.btnInfoClientCS.UseVisualStyleBackColor = true;
            // 
            // btnInfoBooks
            // 
            this.btnInfoBooks.Enabled = false;
            this.btnInfoBooks.Location = new System.Drawing.Point(265, 401);
            this.btnInfoBooks.Name = "btnInfoBooks";
            this.btnInfoBooks.Size = new System.Drawing.Size(86, 34);
            this.btnInfoBooks.TabIndex = 12;
            this.btnInfoBooks.Text = "Информация о книгах";
            this.btnInfoBooks.UseVisualStyleBackColor = true;
            // 
            // lblInfoClient
            // 
            this.lblInfoClient.AutoSize = true;
            this.lblInfoClient.Location = new System.Drawing.Point(381, 436);
            this.lblInfoClient.Name = "lblInfoClient";
            this.lblInfoClient.Size = new System.Drawing.Size(126, 13);
            this.lblInfoClient.TabIndex = 13;
            this.lblInfoClient.Text = "Информация о клиенте";
            // 
            // dgvClientInfo
            // 
            this.dgvClientInfo.AllowUserToAddRows = false;
            this.dgvClientInfo.AllowUserToDeleteRows = false;
            this.dgvClientInfo.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvClientInfo.Location = new System.Drawing.Point(15, 452);
            this.dgvClientInfo.Name = "dgvClientInfo";
            this.dgvClientInfo.ReadOnly = true;
            this.dgvClientInfo.RowHeadersVisible = false;
            this.dgvClientInfo.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvClientInfo.Size = new System.Drawing.Size(918, 58);
            this.dgvClientInfo.TabIndex = 14;
            // 
            // lblLevelLanguage
            // 
            this.lblLevelLanguage.AutoSize = true;
            this.lblLevelLanguage.Location = new System.Drawing.Point(12, 510);
            this.lblLevelLanguage.Name = "lblLevelLanguage";
            this.lblLevelLanguage.Size = new System.Drawing.Size(182, 13);
            this.lblLevelLanguage.TabIndex = 15;
            this.lblLevelLanguage.Text = "Информация о владеемых языках";
            // 
            // dgvLevelLanguage
            // 
            this.dgvLevelLanguage.AllowUserToAddRows = false;
            this.dgvLevelLanguage.AllowUserToDeleteRows = false;
            this.dgvLevelLanguage.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvLevelLanguage.Location = new System.Drawing.Point(15, 526);
            this.dgvLevelLanguage.Name = "dgvLevelLanguage";
            this.dgvLevelLanguage.ReadOnly = true;
            this.dgvLevelLanguage.RowHeadersVisible = false;
            this.dgvLevelLanguage.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvLevelLanguage.Size = new System.Drawing.Size(223, 143);
            this.dgvLevelLanguage.TabIndex = 16;
            // 
            // lblBooks
            // 
            this.lblBooks.AutoSize = true;
            this.lblBooks.Location = new System.Drawing.Point(674, 49);
            this.lblBooks.Name = "lblBooks";
            this.lblBooks.Size = new System.Drawing.Size(37, 13);
            this.lblBooks.TabIndex = 17;
            this.lblBooks.Text = "Книги";
            // 
            // dgvBooks
            // 
            this.dgvBooks.AllowUserToAddRows = false;
            this.dgvBooks.AllowUserToDeleteRows = false;
            this.dgvBooks.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvBooks.Location = new System.Drawing.Point(454, 65);
            this.dgvBooks.Name = "dgvBooks";
            this.dgvBooks.ReadOnly = true;
            this.dgvBooks.RowHeadersVisible = false;
            this.dgvBooks.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvBooks.Size = new System.Drawing.Size(690, 330);
            this.dgvBooks.TabIndex = 18;
            // 
            // btnClientStatistics
            // 
            this.btnClientStatistics.Enabled = false;
            this.btnClientStatistics.Location = new System.Drawing.Point(847, 400);
            this.btnClientStatistics.Name = "btnClientStatistics";
            this.btnClientStatistics.Size = new System.Drawing.Size(86, 34);
            this.btnClientStatistics.TabIndex = 20;
            this.btnClientStatistics.Text = "Статистика клиента";
            this.btnClientStatistics.UseVisualStyleBackColor = true;
            // 
            // btnInfoClientB
            // 
            this.btnInfoClientB.Enabled = false;
            this.btnInfoClientB.Location = new System.Drawing.Point(665, 399);
            this.btnInfoClientB.Name = "btnInfoClientB";
            this.btnInfoClientB.Size = new System.Drawing.Size(86, 35);
            this.btnInfoClientB.TabIndex = 19;
            this.btnInfoClientB.Text = "Информация о клиенте";
            this.btnInfoClientB.UseVisualStyleBackColor = true;
            // 
            // numPeriod
            // 
            this.numPeriod.BackColor = System.Drawing.SystemColors.Info;
            this.numPeriod.Location = new System.Drawing.Point(677, 25);
            this.numPeriod.Maximum = new decimal(new int[] {
            999,
            0,
            0,
            0});
            this.numPeriod.Name = "numPeriod";
            this.numPeriod.Size = new System.Drawing.Size(120, 20);
            this.numPeriod.TabIndex = 21;
            // 
            // lblPeriod
            // 
            this.lblPeriod.AutoSize = true;
            this.lblPeriod.Location = new System.Drawing.Point(676, 9);
            this.lblPeriod.Name = "lblPeriod";
            this.lblPeriod.Size = new System.Drawing.Size(144, 13);
            this.lblPeriod.TabIndex = 22;
            this.lblPeriod.Text = "Временной период (в днях)";
            // 
            // FormSDB
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ScrollBar;
            this.ClientSize = new System.Drawing.Size(1153, 674);
            this.Controls.Add(this.lblPeriod);
            this.Controls.Add(this.numPeriod);
            this.Controls.Add(this.btnClientStatistics);
            this.Controls.Add(this.btnInfoClientB);
            this.Controls.Add(this.dgvBooks);
            this.Controls.Add(this.lblBooks);
            this.Controls.Add(this.dgvLevelLanguage);
            this.Controls.Add(this.lblLevelLanguage);
            this.Controls.Add(this.dgvClientInfo);
            this.Controls.Add(this.lblInfoClient);
            this.Controls.Add(this.btnInfoBooks);
            this.Controls.Add(this.btnInfoClientCS);
            this.Controls.Add(this.lblClientsStatistics);
            this.Controls.Add(this.btnSearch);
            this.Controls.Add(this.txbClientBook);
            this.Controls.Add(this.lblClientBook);
            this.Controls.Add(this.cmbLanguage);
            this.Controls.Add(this.lblLanguage);
            this.Controls.Add(this.cmbStatus);
            this.Controls.Add(this.lblStatus);
            this.Controls.Add(this.lblSearch);
            this.Controls.Add(this.cmbSearch);
            this.Controls.Add(this.dgvClientsStatistics);
            this.Name = "FormSDB";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "SDB";
            ((System.ComponentModel.ISupportInitialize)(this.dgvClientsStatistics)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvClientInfo)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvLevelLanguage)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvBooks)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numPeriod)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.ComboBox cmbSearch;
        private System.Windows.Forms.Label lblSearch;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.Label lblLanguage;
        private System.Windows.Forms.Label lblClientBook;
        private System.Windows.Forms.Button btnSearch;
        private System.Windows.Forms.Label lblClientsStatistics;
        private System.Windows.Forms.Button btnInfoBooks;
        private System.Windows.Forms.Label lblInfoClient;
        private System.Windows.Forms.DataGridView dgvClientInfo;
        private System.Windows.Forms.Label lblLevelLanguage;
        private System.Windows.Forms.DataGridView dgvLevelLanguage;
        private System.Windows.Forms.Label lblBooks;
        private System.Windows.Forms.Button btnClientStatistics;
        public System.Windows.Forms.ComboBox cmbStatus;
        public System.Windows.Forms.ComboBox cmbLanguage;
        public System.Windows.Forms.TextBox txbClientBook;
        public System.Windows.Forms.DataGridView dgvBooks;
        public System.Windows.Forms.DataGridView dgvClientsStatistics;
        public System.Windows.Forms.Button btnInfoClientCS;
        public System.Windows.Forms.Button btnInfoClientB;
        private System.Windows.Forms.Label lblPeriod;
        public System.Windows.Forms.NumericUpDown numPeriod;
    }
}

