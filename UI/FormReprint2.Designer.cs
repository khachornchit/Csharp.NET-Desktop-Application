namespace Machine
{
    partial class FormReprint2
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
            this.btnPrint = new System.Windows.Forms.Button();
            this.labelQuantityPerBox = new System.Windows.Forms.Label();
            this.labelDate = new System.Windows.Forms.Label();
            this.labelWorkOrder = new System.Windows.Forms.Label();
            this.labelBoxNo = new System.Windows.Forms.Label();
            this.labelArticleNumber = new System.Windows.Forms.Label();
            this.txtQuantityPerBox = new System.Windows.Forms.TextBox();
            this.txtDate = new System.Windows.Forms.TextBox();
            this.txtWorkOrder = new System.Windows.Forms.TextBox();
            this.txtBoxNo = new System.Windows.Forms.TextBox();
            this.txtArticleNumber = new System.Windows.Forms.TextBox();
            this.txtLabelName = new System.Windows.Forms.TextBox();
            this.labelName = new System.Windows.Forms.Label();
            this.btnGetWorkOrder = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnPrint
            // 
            this.btnPrint.Location = new System.Drawing.Point(270, 153);
            this.btnPrint.Name = "btnPrint";
            this.btnPrint.Size = new System.Drawing.Size(90, 26);
            this.btnPrint.TabIndex = 15;
            this.btnPrint.Text = "Print";
            this.btnPrint.UseVisualStyleBackColor = true;
            this.btnPrint.Click += new System.EventHandler(this.btnPrint_Click);
            // 
            // labelQuantityPerBox
            // 
            this.labelQuantityPerBox.AutoSize = true;
            this.labelQuantityPerBox.Location = new System.Drawing.Point(38, 132);
            this.labelQuantityPerBox.Name = "labelQuantityPerBox";
            this.labelQuantityPerBox.Size = new System.Drawing.Size(49, 13);
            this.labelQuantityPerBox.TabIndex = 22;
            this.labelQuantityPerBox.Text = "Qty/Box:";
            // 
            // labelDate
            // 
            this.labelDate.AutoSize = true;
            this.labelDate.Location = new System.Drawing.Point(54, 161);
            this.labelDate.Name = "labelDate";
            this.labelDate.Size = new System.Drawing.Size(33, 13);
            this.labelDate.TabIndex = 21;
            this.labelDate.Text = "Date:";
            // 
            // labelWorkOrder
            // 
            this.labelWorkOrder.AutoSize = true;
            this.labelWorkOrder.Location = new System.Drawing.Point(27, 74);
            this.labelWorkOrder.Name = "labelWorkOrder";
            this.labelWorkOrder.Size = new System.Drawing.Size(60, 13);
            this.labelWorkOrder.TabIndex = 18;
            this.labelWorkOrder.Text = "Workorder:";
            // 
            // labelBoxNo
            // 
            this.labelBoxNo.AutoSize = true;
            this.labelBoxNo.Location = new System.Drawing.Point(39, 103);
            this.labelBoxNo.Name = "labelBoxNo";
            this.labelBoxNo.Size = new System.Drawing.Size(48, 13);
            this.labelBoxNo.TabIndex = 20;
            this.labelBoxNo.Text = "Box No.:";
            // 
            // labelArticleNumber
            // 
            this.labelArticleNumber.AutoSize = true;
            this.labelArticleNumber.Location = new System.Drawing.Point(10, 45);
            this.labelArticleNumber.Name = "labelArticleNumber";
            this.labelArticleNumber.Size = new System.Drawing.Size(77, 13);
            this.labelArticleNumber.TabIndex = 17;
            this.labelArticleNumber.Text = "Article number:";
            // 
            // txtQuantityPerBox
            // 
            this.txtQuantityPerBox.BackColor = System.Drawing.Color.White;
            this.txtQuantityPerBox.Location = new System.Drawing.Point(89, 128);
            this.txtQuantityPerBox.MaxLength = 5;
            this.txtQuantityPerBox.Name = "txtQuantityPerBox";
            this.txtQuantityPerBox.Size = new System.Drawing.Size(105, 20);
            this.txtQuantityPerBox.TabIndex = 29;
            // 
            // txtDate
            // 
            this.txtDate.BackColor = System.Drawing.Color.White;
            this.txtDate.Location = new System.Drawing.Point(89, 157);
            this.txtDate.MaxLength = 8;
            this.txtDate.Name = "txtDate";
            this.txtDate.Size = new System.Drawing.Size(105, 20);
            this.txtDate.TabIndex = 28;
            // 
            // txtWorkOrder
            // 
            this.txtWorkOrder.BackColor = System.Drawing.Color.White;
            this.txtWorkOrder.Location = new System.Drawing.Point(89, 70);
            this.txtWorkOrder.MaxLength = 5;
            this.txtWorkOrder.Name = "txtWorkOrder";
            this.txtWorkOrder.Size = new System.Drawing.Size(105, 20);
            this.txtWorkOrder.TabIndex = 25;
            // 
            // txtBoxNo
            // 
            this.txtBoxNo.BackColor = System.Drawing.Color.White;
            this.txtBoxNo.Location = new System.Drawing.Point(89, 99);
            this.txtBoxNo.MaxLength = 5;
            this.txtBoxNo.Name = "txtBoxNo";
            this.txtBoxNo.Size = new System.Drawing.Size(105, 20);
            this.txtBoxNo.TabIndex = 27;
            // 
            // txtArticleNumber
            // 
            this.txtArticleNumber.BackColor = System.Drawing.Color.White;
            this.txtArticleNumber.Location = new System.Drawing.Point(89, 41);
            this.txtArticleNumber.MaxLength = 12;
            this.txtArticleNumber.Name = "txtArticleNumber";
            this.txtArticleNumber.Size = new System.Drawing.Size(271, 20);
            this.txtArticleNumber.TabIndex = 24;
            // 
            // txtLabelName
            // 
            this.txtLabelName.BackColor = System.Drawing.Color.White;
            this.txtLabelName.Location = new System.Drawing.Point(89, 12);
            this.txtLabelName.MaxLength = 90;
            this.txtLabelName.Name = "txtLabelName";
            this.txtLabelName.Size = new System.Drawing.Size(271, 20);
            this.txtLabelName.TabIndex = 23;
            // 
            // labelName
            // 
            this.labelName.AutoSize = true;
            this.labelName.Location = new System.Drawing.Point(49, 16);
            this.labelName.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labelName.Name = "labelName";
            this.labelName.Size = new System.Drawing.Size(38, 13);
            this.labelName.TabIndex = 16;
            this.labelName.Text = "Name:";
            // 
            // btnGetWorkOrder
            // 
            this.btnGetWorkOrder.Location = new System.Drawing.Point(270, 116);
            this.btnGetWorkOrder.Name = "btnGetWorkOrder";
            this.btnGetWorkOrder.Size = new System.Drawing.Size(90, 26);
            this.btnGetWorkOrder.TabIndex = 30;
            this.btnGetWorkOrder.Text = "Get Work Order";
            this.btnGetWorkOrder.UseVisualStyleBackColor = true;
            this.btnGetWorkOrder.Click += new System.EventHandler(this.btnGetWorkOrder_Click);
            // 
            // FormReprint2
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.ClientSize = new System.Drawing.Size(374, 192);
            this.Controls.Add(this.btnGetWorkOrder);
            this.Controls.Add(this.btnPrint);
            this.Controls.Add(this.labelQuantityPerBox);
            this.Controls.Add(this.labelDate);
            this.Controls.Add(this.labelWorkOrder);
            this.Controls.Add(this.labelBoxNo);
            this.Controls.Add(this.labelArticleNumber);
            this.Controls.Add(this.txtQuantityPerBox);
            this.Controls.Add(this.txtDate);
            this.Controls.Add(this.txtWorkOrder);
            this.Controls.Add(this.txtBoxNo);
            this.Controls.Add(this.txtArticleNumber);
            this.Controls.Add(this.txtLabelName);
            this.Controls.Add(this.labelName);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormReprint2";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Print Label";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnPrint;
        private System.Windows.Forms.Label labelQuantityPerBox;
        private System.Windows.Forms.Label labelDate;
        private System.Windows.Forms.Label labelWorkOrder;
        private System.Windows.Forms.Label labelBoxNo;
        private System.Windows.Forms.Label labelArticleNumber;
        private System.Windows.Forms.TextBox txtQuantityPerBox;
        private System.Windows.Forms.TextBox txtDate;
        private System.Windows.Forms.TextBox txtWorkOrder;
        private System.Windows.Forms.TextBox txtBoxNo;
        private System.Windows.Forms.TextBox txtArticleNumber;
        private System.Windows.Forms.TextBox txtLabelName;
        private System.Windows.Forms.Label labelName;
        private System.Windows.Forms.Button btnGetWorkOrder;
    }
}