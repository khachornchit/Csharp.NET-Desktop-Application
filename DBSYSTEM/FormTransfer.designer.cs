namespace Registrations
{
    partial class FormTransfer
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
            this.label1 = new System.Windows.Forms.Label();
            this.labelStatus = new System.Windows.Forms.Label();
            this.btnOK = new System.Windows.Forms.Button();
            this.timerTransfer = new System.Windows.Forms.Timer(this.components);
            this.progressBarTransfer = new System.Windows.Forms.ProgressBar();
            this.labelPercentComplete = new System.Windows.Forms.Label();
            this.labelTotal = new System.Windows.Forms.Label();
            this.labelTransfer = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Arial", 11.25F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(34, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(0, 17);
            this.label1.TabIndex = 0;
            // 
            // labelStatus
            // 
            this.labelStatus.Font = new System.Drawing.Font("Arial", 12F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelStatus.ForeColor = System.Drawing.Color.Blue;
            this.labelStatus.Location = new System.Drawing.Point(29, 10);
            this.labelStatus.Name = "labelStatus";
            this.labelStatus.Size = new System.Drawing.Size(327, 32);
            this.labelStatus.TabIndex = 1;
            this.labelStatus.Text = "Transfer databse to server";
            this.labelStatus.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btnOK
            // 
            this.btnOK.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnOK.Location = new System.Drawing.Point(155, 162);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 30);
            this.btnOK.TabIndex = 2;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // timerTransfer
            // 
            this.timerTransfer.Interval = 500;
            this.timerTransfer.Tick += new System.EventHandler(this.timerTransfer_Tick);
            // 
            // progressBarTransfer
            // 
            this.progressBarTransfer.Location = new System.Drawing.Point(12, 119);
            this.progressBarTransfer.Name = "progressBarTransfer";
            this.progressBarTransfer.Size = new System.Drawing.Size(361, 23);
            this.progressBarTransfer.Step = 1;
            this.progressBarTransfer.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.progressBarTransfer.TabIndex = 3;
            this.progressBarTransfer.UseWaitCursor = true;
            // 
            // labelPercentComplete
            // 
            this.labelPercentComplete.BackColor = System.Drawing.SystemColors.Control;
            this.labelPercentComplete.Font = new System.Drawing.Font("Arial", 9.75F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelPercentComplete.Location = new System.Drawing.Point(121, 94);
            this.labelPercentComplete.Name = "labelPercentComplete";
            this.labelPercentComplete.Size = new System.Drawing.Size(143, 23);
            this.labelPercentComplete.TabIndex = 4;
            this.labelPercentComplete.Text = "Complete 0 % ";
            this.labelPercentComplete.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // labelTotal
            // 
            this.labelTotal.BackColor = System.Drawing.SystemColors.Control;
            this.labelTotal.Font = new System.Drawing.Font("Arial", 9.75F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelTotal.Location = new System.Drawing.Point(59, 48);
            this.labelTotal.Name = "labelTotal";
            this.labelTotal.Size = new System.Drawing.Size(262, 23);
            this.labelTotal.TabIndex = 5;
            this.labelTotal.Text = "Total";
            this.labelTotal.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // labelTransfer
            // 
            this.labelTransfer.BackColor = System.Drawing.SystemColors.Control;
            this.labelTransfer.Font = new System.Drawing.Font("Arial", 9.75F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelTransfer.Location = new System.Drawing.Point(59, 71);
            this.labelTransfer.Name = "labelTransfer";
            this.labelTransfer.Size = new System.Drawing.Size(262, 23);
            this.labelTransfer.TabIndex = 6;
            this.labelTransfer.Text = "Transfer";
            this.labelTransfer.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // FormTransfer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(385, 201);
            this.ControlBox = false;
            this.Controls.Add(this.labelTransfer);
            this.Controls.Add(this.labelTotal);
            this.Controls.Add(this.labelPercentComplete);
            this.Controls.Add(this.progressBarTransfer);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.labelStatus);
            this.Controls.Add(this.label1);
            this.Name = "FormTransfer";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Transfer database to server";
            this.Load += new System.EventHandler(this.FormTransfer_Load);
            this.Shown += new System.EventHandler(this.FormTransfer_Shown);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormTransfer_FormClosing);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label labelStatus;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Timer timerTransfer;
        private System.Windows.Forms.ProgressBar progressBarTransfer;
        private System.Windows.Forms.Label labelPercentComplete;
        private System.Windows.Forms.Label labelTotal;
        private System.Windows.Forms.Label labelTransfer;
    }
}

