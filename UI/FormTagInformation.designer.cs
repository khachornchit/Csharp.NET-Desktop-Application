namespace Machine
{
    partial class FormTagInformation
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
            this.dataGridViewTagInfo = new System.Windows.Forms.DataGridView();
            this.Column1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.timerShowTagInfo = new System.Windows.Forms.Timer(this.components);
            this.btnOK = new System.Windows.Forms.Button();
            this.labelTagIsInCurrentBox = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewTagInfo)).BeginInit();
            this.SuspendLayout();
            // 
            // dataGridViewTagInfo
            // 
            this.dataGridViewTagInfo.AllowUserToAddRows = false;
            this.dataGridViewTagInfo.AllowUserToDeleteRows = false;
            this.dataGridViewTagInfo.AllowUserToOrderColumns = true;
            this.dataGridViewTagInfo.AllowUserToResizeColumns = false;
            this.dataGridViewTagInfo.AllowUserToResizeRows = false;
            this.dataGridViewTagInfo.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.dataGridViewTagInfo.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewTagInfo.ColumnHeadersVisible = false;
            this.dataGridViewTagInfo.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Column1,
            this.Column2});
            this.dataGridViewTagInfo.Location = new System.Drawing.Point(11, 15);
            this.dataGridViewTagInfo.Name = "dataGridViewTagInfo";
            this.dataGridViewTagInfo.ReadOnly = true;
            this.dataGridViewTagInfo.RowHeadersVisible = false;
            this.dataGridViewTagInfo.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridViewTagInfo.Size = new System.Drawing.Size(243, 334);
            this.dataGridViewTagInfo.TabIndex = 3;
            // 
            // Column1
            // 
            this.Column1.HeaderText = "NAME";
            this.Column1.Name = "Column1";
            this.Column1.ReadOnly = true;
            this.Column1.Width = 5;
            // 
            // Column2
            // 
            this.Column2.HeaderText = "VALUE";
            this.Column2.Name = "Column2";
            this.Column2.ReadOnly = true;
            this.Column2.Width = 5;
            // 
            // timerShowTagInfo
            // 
            this.timerShowTagInfo.Tick += new System.EventHandler(this.timerShowTagInfo_Tick);
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(92, 374);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 4;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // labelTagIsInCurrentBox
            // 
            this.labelTagIsInCurrentBox.BackColor = System.Drawing.Color.Green;
            this.labelTagIsInCurrentBox.Font = new System.Drawing.Font("Arial", 9F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelTagIsInCurrentBox.ForeColor = System.Drawing.Color.White;
            this.labelTagIsInCurrentBox.Location = new System.Drawing.Point(67, 352);
            this.labelTagIsInCurrentBox.Name = "labelTagIsInCurrentBox";
            this.labelTagIsInCurrentBox.Size = new System.Drawing.Size(130, 20);
            this.labelTagIsInCurrentBox.TabIndex = 5;
            this.labelTagIsInCurrentBox.Text = "Tag is in current box";
            this.labelTagIsInCurrentBox.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.labelTagIsInCurrentBox.Visible = false;
            // 
            // FormTagInformation
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(264, 407);
            this.Controls.Add(this.labelTagIsInCurrentBox);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.dataGridViewTagInfo);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormTagInformation";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Tag Information";
            this.Load += new System.EventHandler(this.FormReadUID_Load);
            this.Shown += new System.EventHandler(this.FormReadUID_Shown);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FormReadUID_FormClosed);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewTagInfo)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView dataGridViewTagInfo;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column1;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column2;
        private System.Windows.Forms.Timer timerShowTagInfo;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Label labelTagIsInCurrentBox;
    }
}