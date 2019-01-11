namespace Machine
{
    partial class FormWorkOrder
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormWorkOrder));
            this.labelWorkOrderTarget = new System.Windows.Forms.Label();
            this.chkHideFinishedWorkOrder = new System.Windows.Forms.CheckBox();
            this.labelWorkOrderStatus = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.labelArticleNumber = new System.Windows.Forms.Label();
            this.labelProductName = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            this.txtWorkOrder = new System.Windows.Forms.TextBox();
            this.label38 = new System.Windows.Forms.Label();
            this.label37 = new System.Windows.Forms.Label();
            this.dataGridViewWorkOrder = new System.Windows.Forms.DataGridView();
            this.dgvWorkOrder = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dgvArticle = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column7 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dgvTagType = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column8 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dgvProductName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column9 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column10 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dgvQuantityPerBox = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dgvWorkOrderTarget = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column4 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column5 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column6 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dgvWorkOrderStatus = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dgvRegister = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column14 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column15 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column16 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column17 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column18 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column19 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dgvBeginningUID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dgvEndingUID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.btnTransferDatabase = new System.Windows.Forms.Button();
            this.btnGetWorkOrder = new System.Windows.Forms.Button();
            this.btnDeleteWorkOrder = new System.Windows.Forms.Button();
            this.btnRefresh = new System.Windows.Forms.Button();
            this.btnChangBoxNumber = new System.Windows.Forms.Button();
            this.btnSelectWorkOrder = new System.Windows.Forms.Button();
            this.btnAddWorkOrder = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.timerInitial = new System.Windows.Forms.Timer(this.components);
            this.txtQuantityPerBox = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewWorkOrder)).BeginInit();
            this.SuspendLayout();
            // 
            // labelWorkOrderTarget
            // 
            this.labelWorkOrderTarget.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.labelWorkOrderTarget.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.labelWorkOrderTarget.Location = new System.Drawing.Point(12, 554);
            this.labelWorkOrderTarget.Name = "labelWorkOrderTarget";
            this.labelWorkOrderTarget.Size = new System.Drawing.Size(133, 20);
            this.labelWorkOrderTarget.TabIndex = 82;
            this.labelWorkOrderTarget.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // chkHideFinishedWorkOrder
            // 
            this.chkHideFinishedWorkOrder.AutoSize = true;
            this.chkHideFinishedWorkOrder.Checked = true;
            this.chkHideFinishedWorkOrder.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkHideFinishedWorkOrder.Location = new System.Drawing.Point(1150, 417);
            this.chkHideFinishedWorkOrder.Name = "chkHideFinishedWorkOrder";
            this.chkHideFinishedWorkOrder.Size = new System.Drawing.Size(137, 17);
            this.chkHideFinishedWorkOrder.TabIndex = 78;
            this.chkHideFinishedWorkOrder.Text = "Hide finished workorder";
            this.chkHideFinishedWorkOrder.UseVisualStyleBackColor = true;
            this.chkHideFinishedWorkOrder.Click += new System.EventHandler(this.chkHideFinishedWorkOrder_Click);
            // 
            // labelWorkOrderStatus
            // 
            this.labelWorkOrderStatus.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.labelWorkOrderStatus.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.labelWorkOrderStatus.Location = new System.Drawing.Point(152, 554);
            this.labelWorkOrderStatus.Name = "labelWorkOrderStatus";
            this.labelWorkOrderStatus.Size = new System.Drawing.Size(160, 20);
            this.labelWorkOrderStatus.TabIndex = 77;
            this.labelWorkOrderStatus.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(152, 538);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(88, 13);
            this.label6.TabIndex = 76;
            this.label6.Text = "Workorder status";
            // 
            // labelArticleNumber
            // 
            this.labelArticleNumber.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.labelArticleNumber.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.labelArticleNumber.Location = new System.Drawing.Point(12, 508);
            this.labelArticleNumber.Name = "labelArticleNumber";
            this.labelArticleNumber.Size = new System.Drawing.Size(300, 20);
            this.labelArticleNumber.TabIndex = 67;
            this.labelArticleNumber.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // labelProductName
            // 
            this.labelProductName.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.labelProductName.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.labelProductName.Location = new System.Drawing.Point(12, 462);
            this.labelProductName.Name = "labelProductName";
            this.labelProductName.Size = new System.Drawing.Size(300, 20);
            this.labelProductName.TabIndex = 66;
            this.labelProductName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(12, 585);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(68, 13);
            this.label15.TabIndex = 74;
            this.label15.Text = "Quantity/box";
            // 
            // txtWorkOrder
            // 
            this.txtWorkOrder.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.txtWorkOrder.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtWorkOrder.Location = new System.Drawing.Point(12, 415);
            this.txtWorkOrder.MaxLength = 5;
            this.txtWorkOrder.Name = "txtWorkOrder";
            this.txtWorkOrder.Size = new System.Drawing.Size(94, 20);
            this.txtWorkOrder.TabIndex = 63;
            this.txtWorkOrder.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.txtWorkOrder.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtWorkOrder_KeyPress);
            // 
            // label38
            // 
            this.label38.AutoSize = true;
            this.label38.Location = new System.Drawing.Point(12, 492);
            this.label38.Name = "label38";
            this.label38.Size = new System.Drawing.Size(76, 13);
            this.label38.TabIndex = 69;
            this.label38.Text = "Article Number";
            // 
            // label37
            // 
            this.label37.AutoSize = true;
            this.label37.Location = new System.Drawing.Point(12, 446);
            this.label37.Name = "label37";
            this.label37.Size = new System.Drawing.Size(35, 13);
            this.label37.TabIndex = 68;
            this.label37.Text = "Name";
            // 
            // dataGridViewWorkOrder
            // 
            this.dataGridViewWorkOrder.AllowUserToAddRows = false;
            this.dataGridViewWorkOrder.AllowUserToResizeColumns = false;
            this.dataGridViewWorkOrder.AllowUserToResizeRows = false;
            this.dataGridViewWorkOrder.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dataGridViewWorkOrder.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dataGridViewWorkOrder.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewWorkOrder.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dgvWorkOrder,
            this.dgvArticle,
            this.Column7,
            this.dgvTagType,
            this.Column8,
            this.dgvProductName,
            this.Column9,
            this.Column10,
            this.dgvQuantityPerBox,
            this.dgvWorkOrderTarget,
            this.Column3,
            this.Column4,
            this.Column5,
            this.Column6,
            this.dgvWorkOrderStatus,
            this.dgvRegister,
            this.Column14,
            this.Column15,
            this.Column16,
            this.Column17,
            this.Column18,
            this.Column19,
            this.dgvBeginningUID,
            this.dgvEndingUID});
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dataGridViewWorkOrder.DefaultCellStyle = dataGridViewCellStyle2;
            this.dataGridViewWorkOrder.Location = new System.Drawing.Point(6, 9);
            this.dataGridViewWorkOrder.Name = "dataGridViewWorkOrder";
            this.dataGridViewWorkOrder.ReadOnly = true;
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridViewWorkOrder.RowHeadersDefaultCellStyle = dataGridViewCellStyle3;
            this.dataGridViewWorkOrder.RowHeadersVisible = false;
            this.dataGridViewWorkOrder.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridViewWorkOrder.Size = new System.Drawing.Size(1281, 385);
            this.dataGridViewWorkOrder.TabIndex = 62;
            this.dataGridViewWorkOrder.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridViewWorkOrder_CellClick);
            // 
            // dgvWorkOrder
            // 
            this.dgvWorkOrder.HeaderText = "Work order";
            this.dgvWorkOrder.Name = "dgvWorkOrder";
            this.dgvWorkOrder.ReadOnly = true;
            this.dgvWorkOrder.Width = 85;
            // 
            // dgvArticle
            // 
            this.dgvArticle.HeaderText = "Article";
            this.dgvArticle.Name = "dgvArticle";
            this.dgvArticle.ReadOnly = true;
            this.dgvArticle.Width = 61;
            // 
            // Column7
            // 
            this.Column7.HeaderText = "ProductType";
            this.Column7.Name = "Column7";
            this.Column7.ReadOnly = true;
            this.Column7.Width = 93;
            // 
            // dgvTagType
            // 
            this.dgvTagType.HeaderText = "Tag Type";
            this.dgvTagType.Name = "dgvTagType";
            this.dgvTagType.ReadOnly = true;
            this.dgvTagType.Width = 78;
            // 
            // Column8
            // 
            this.Column8.HeaderText = "Customer Name";
            this.Column8.Name = "Column8";
            this.Column8.ReadOnly = true;
            this.Column8.Width = 107;
            // 
            // dgvProductName
            // 
            this.dgvProductName.HeaderText = "Product Name";
            this.dgvProductName.Name = "dgvProductName";
            this.dgvProductName.ReadOnly = true;
            // 
            // Column9
            // 
            this.Column9.HeaderText = "Label Name";
            this.Column9.Name = "Column9";
            this.Column9.ReadOnly = true;
            this.Column9.Width = 89;
            // 
            // Column10
            // 
            this.Column10.HeaderText = "Label Layout";
            this.Column10.Name = "Column10";
            this.Column10.ReadOnly = true;
            this.Column10.Width = 93;
            // 
            // dgvQuantityPerBox
            // 
            this.dgvQuantityPerBox.HeaderText = "Quantity/Box";
            this.dgvQuantityPerBox.Name = "dgvQuantityPerBox";
            this.dgvQuantityPerBox.ReadOnly = true;
            this.dgvQuantityPerBox.Width = 94;
            // 
            // dgvWorkOrderTarget
            // 
            this.dgvWorkOrderTarget.HeaderText = "Work Order Target";
            this.dgvWorkOrderTarget.Name = "dgvWorkOrderTarget";
            this.dgvWorkOrderTarget.ReadOnly = true;
            this.dgvWorkOrderTarget.Width = 121;
            // 
            // Column3
            // 
            this.Column3.HeaderText = "Quantity Worked";
            this.Column3.Name = "Column3";
            this.Column3.ReadOnly = true;
            this.Column3.Width = 112;
            // 
            // Column4
            // 
            this.Column4.HeaderText = "Good UID";
            this.Column4.Name = "Column4";
            this.Column4.ReadOnly = true;
            this.Column4.Width = 80;
            // 
            // Column5
            // 
            this.Column5.HeaderText = "Bad UID";
            this.Column5.Name = "Column5";
            this.Column5.ReadOnly = true;
            this.Column5.Width = 73;
            // 
            // Column6
            // 
            this.Column6.HeaderText = "Partial Box Closed";
            this.Column6.Name = "Column6";
            this.Column6.ReadOnly = true;
            this.Column6.Width = 117;
            // 
            // dgvWorkOrderStatus
            // 
            this.dgvWorkOrderStatus.HeaderText = "Work Order Status";
            this.dgvWorkOrderStatus.Name = "dgvWorkOrderStatus";
            this.dgvWorkOrderStatus.ReadOnly = true;
            this.dgvWorkOrderStatus.Width = 120;
            // 
            // dgvRegister
            // 
            this.dgvRegister.HeaderText = "Reigister UID";
            this.dgvRegister.Name = "dgvRegister";
            this.dgvRegister.ReadOnly = true;
            this.dgvRegister.Width = 95;
            // 
            // Column14
            // 
            this.Column14.HeaderText = "Use Tag Meter";
            this.Column14.Name = "Column14";
            this.Column14.ReadOnly = true;
            this.Column14.Width = 103;
            // 
            // Column15
            // 
            this.Column15.HeaderText = "Testing";
            this.Column15.Name = "Column15";
            this.Column15.ReadOnly = true;
            this.Column15.Width = 67;
            // 
            // Column16
            // 
            this.Column16.HeaderText = "Use Tag Programmer";
            this.Column16.Name = "Column16";
            this.Column16.ReadOnly = true;
            this.Column16.Width = 132;
            // 
            // Column17
            // 
            this.Column17.HeaderText = "Program Option";
            this.Column17.Name = "Column17";
            this.Column17.ReadOnly = true;
            this.Column17.Width = 105;
            // 
            // Column18
            // 
            this.Column18.HeaderText = "Power Level [%]";
            this.Column18.Name = "Column18";
            this.Column18.ReadOnly = true;
            this.Column18.Width = 108;
            // 
            // Column19
            // 
            this.Column19.HeaderText = "Trimming Frequency [Hz]";
            this.Column19.Name = "Column19";
            this.Column19.ReadOnly = true;
            this.Column19.Width = 149;
            // 
            // dgvBeginningUID
            // 
            this.dgvBeginningUID.HeaderText = "Beginning UID";
            this.dgvBeginningUID.Name = "dgvBeginningUID";
            this.dgvBeginningUID.ReadOnly = true;
            this.dgvBeginningUID.Width = 101;
            // 
            // dgvEndingUID
            // 
            this.dgvEndingUID.HeaderText = "Ending UID";
            this.dgvEndingUID.Name = "dgvEndingUID";
            this.dgvEndingUID.ReadOnly = true;
            this.dgvEndingUID.Width = 87;
            // 
            // btnTransferDatabase
            // 
            this.btnTransferDatabase.Image = global::Machine.Properties.Resources.download_database_16x16;
            this.btnTransferDatabase.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnTransferDatabase.Location = new System.Drawing.Point(1150, 633);
            this.btnTransferDatabase.Name = "btnTransferDatabase";
            this.btnTransferDatabase.Size = new System.Drawing.Size(132, 27);
            this.btnTransferDatabase.TabIndex = 79;
            this.btnTransferDatabase.Text = "     Transfer Database";
            this.btnTransferDatabase.UseVisualStyleBackColor = true;
            this.btnTransferDatabase.Visible = false;
            this.btnTransferDatabase.Click += new System.EventHandler(this.btnTransferDatabase_Click);
            // 
            // btnGetWorkOrder
            // 
            this.btnGetWorkOrder.Image = global::Machine.Properties.Resources.download_database_16x16;
            this.btnGetWorkOrder.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnGetWorkOrder.Location = new System.Drawing.Point(117, 412);
            this.btnGetWorkOrder.Name = "btnGetWorkOrder";
            this.btnGetWorkOrder.Size = new System.Drawing.Size(123, 27);
            this.btnGetWorkOrder.TabIndex = 72;
            this.btnGetWorkOrder.Text = "Get work order";
            this.btnGetWorkOrder.UseVisualStyleBackColor = true;
            this.btnGetWorkOrder.Click += new System.EventHandler(this.btnGetWorkOrder_Click);
            // 
            // btnDeleteWorkOrder
            // 
            this.btnDeleteWorkOrder.Enabled = false;
            this.btnDeleteWorkOrder.Image = global::Machine.Properties.Resources.edit_delete_16x16;
            this.btnDeleteWorkOrder.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnDeleteWorkOrder.Location = new System.Drawing.Point(114, 632);
            this.btnDeleteWorkOrder.Name = "btnDeleteWorkOrder";
            this.btnDeleteWorkOrder.Size = new System.Drawing.Size(90, 27);
            this.btnDeleteWorkOrder.TabIndex = 71;
            this.btnDeleteWorkOrder.Text = "Delete";
            this.btnDeleteWorkOrder.UseVisualStyleBackColor = true;
            this.btnDeleteWorkOrder.Click += new System.EventHandler(this.btnDeleteWorkOrder_Click);
            // 
            // btnRefresh
            // 
            this.btnRefresh.Image = ((System.Drawing.Image)(resources.GetObject("btnRefresh.Image")));
            this.btnRefresh.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnRefresh.Location = new System.Drawing.Point(1150, 477);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(132, 27);
            this.btnRefresh.TabIndex = 80;
            this.btnRefresh.Text = "Refresh";
            this.btnRefresh.UseVisualStyleBackColor = true;
            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
            // 
            // btnChangBoxNumber
            // 
            this.btnChangBoxNumber.Image = global::Machine.Properties.Resources.Apps_utilities_file_archiver_icon_16x16;
            this.btnChangBoxNumber.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnChangBoxNumber.Location = new System.Drawing.Point(1150, 509);
            this.btnChangBoxNumber.Name = "btnChangBoxNumber";
            this.btnChangBoxNumber.Size = new System.Drawing.Size(132, 27);
            this.btnChangBoxNumber.TabIndex = 75;
            this.btnChangBoxNumber.Text = "     Change Box Number";
            this.btnChangBoxNumber.UseVisualStyleBackColor = true;
            this.btnChangBoxNumber.Visible = false;
            this.btnChangBoxNumber.Click += new System.EventHandler(this.btnChangBoxNumber_Click);
            // 
            // btnSelectWorkOrder
            // 
            this.btnSelectWorkOrder.Image = global::Machine.Properties.Resources.folder_open_go_16x16;
            this.btnSelectWorkOrder.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnSelectWorkOrder.Location = new System.Drawing.Point(1150, 445);
            this.btnSelectWorkOrder.Name = "btnSelectWorkOrder";
            this.btnSelectWorkOrder.Size = new System.Drawing.Size(132, 27);
            this.btnSelectWorkOrder.TabIndex = 73;
            this.btnSelectWorkOrder.Text = "Select";
            this.btnSelectWorkOrder.UseVisualStyleBackColor = true;
            this.btnSelectWorkOrder.Click += new System.EventHandler(this.btnSelectWorkOrder_Click);
            // 
            // btnAddWorkOrder
            // 
            this.btnAddWorkOrder.Enabled = false;
            this.btnAddWorkOrder.Image = ((System.Drawing.Image)(resources.GetObject("btnAddWorkOrder.Image")));
            this.btnAddWorkOrder.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnAddWorkOrder.Location = new System.Drawing.Point(9, 632);
            this.btnAddWorkOrder.Name = "btnAddWorkOrder";
            this.btnAddWorkOrder.Size = new System.Drawing.Size(90, 27);
            this.btnAddWorkOrder.TabIndex = 70;
            this.btnAddWorkOrder.Text = "Add";
            this.btnAddWorkOrder.UseVisualStyleBackColor = true;
            this.btnAddWorkOrder.Click += new System.EventHandler(this.btnAddWorkOrder_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 538);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(87, 13);
            this.label1.TabIndex = 83;
            this.label1.Text = "Workorder target";
            // 
            // timerInitial
            // 
            this.timerInitial.Interval = 5;
            this.timerInitial.Tick += new System.EventHandler(this.timerInitial_Tick);
            // 
            // txtQuantityPerBox
            // 
            this.txtQuantityPerBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.txtQuantityPerBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtQuantityPerBox.Location = new System.Drawing.Point(12, 600);
            this.txtQuantityPerBox.MaxLength = 5;
            this.txtQuantityPerBox.Name = "txtQuantityPerBox";
            this.txtQuantityPerBox.ReadOnly = true;
            this.txtQuantityPerBox.Size = new System.Drawing.Size(133, 20);
            this.txtQuantityPerBox.TabIndex = 84;
            // 
            // FormWorkOrder
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1294, 672);
            this.Controls.Add(this.txtQuantityPerBox);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.labelWorkOrderTarget);
            this.Controls.Add(this.chkHideFinishedWorkOrder);
            this.Controls.Add(this.labelWorkOrderStatus);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.labelArticleNumber);
            this.Controls.Add(this.labelProductName);
            this.Controls.Add(this.label15);
            this.Controls.Add(this.txtWorkOrder);
            this.Controls.Add(this.label38);
            this.Controls.Add(this.label37);
            this.Controls.Add(this.dataGridViewWorkOrder);
            this.Controls.Add(this.btnRefresh);
            this.Controls.Add(this.btnTransferDatabase);
            this.Controls.Add(this.btnChangBoxNumber);
            this.Controls.Add(this.btnGetWorkOrder);
            this.Controls.Add(this.btnSelectWorkOrder);
            this.Controls.Add(this.btnDeleteWorkOrder);
            this.Controls.Add(this.btnAddWorkOrder);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "FormWorkOrder";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Work Order List";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FormWorkOrder_FormClosed);
            this.Load += new System.EventHandler(this.FormWorkOrder_Load);
            this.Shown += new System.EventHandler(this.FormWorkOrder_Shown);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewWorkOrder)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label labelWorkOrderTarget;
        private System.Windows.Forms.CheckBox chkHideFinishedWorkOrder;
        private System.Windows.Forms.Label labelWorkOrderStatus;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label labelArticleNumber;
        private System.Windows.Forms.Label labelProductName;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.TextBox txtWorkOrder;
        private System.Windows.Forms.Label label38;
        private System.Windows.Forms.Label label37;
        private System.Windows.Forms.DataGridView dataGridViewWorkOrder;
        private System.Windows.Forms.Button btnRefresh;
        private System.Windows.Forms.Button btnTransferDatabase;
        private System.Windows.Forms.Button btnChangBoxNumber;
        private System.Windows.Forms.Button btnGetWorkOrder;
        private System.Windows.Forms.Button btnSelectWorkOrder;
        private System.Windows.Forms.Button btnDeleteWorkOrder;
        private System.Windows.Forms.Button btnAddWorkOrder;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Timer timerInitial;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgvWorkOrder;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgvArticle;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column7;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgvTagType;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column8;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgvProductName;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column9;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column10;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgvQuantityPerBox;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgvWorkOrderTarget;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column3;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column4;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column5;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column6;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgvWorkOrderStatus;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgvRegister;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column14;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column15;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column16;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column17;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column18;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column19;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgvBeginningUID;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgvEndingUID;
        private System.Windows.Forms.TextBox txtQuantityPerBox;
    }
}