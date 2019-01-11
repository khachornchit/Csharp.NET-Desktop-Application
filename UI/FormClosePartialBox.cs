using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Machine
{
    public partial class FormClosePartialBox : Form
    {
        bool bClosePartialBox = false;
        string sBoxNumber;

        public FormClosePartialBox(string sBoxNumber)
        {
            this.sBoxNumber = sBoxNumber;
            InitializeComponent();
        }

        private void FormClosePartialBox_Load(object sender, EventArgs e)
        {
            labelQuestion.Text = "Do you want to close partial box " + this.sBoxNumber + " ?";
        }

        private void btnYes_Click(object sender, EventArgs e)
        {
            bClosePartialBox = txtBoxNumber.Text == sBoxNumber;
            if (bClosePartialBox)
            {
                this.Close();
            }
            else
            {
                MessageBox.Show("The box is not " + sBoxNumber + ".\nPlease enter correct box number.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btnNo_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void txtBoxNumber_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !Char.IsDigit(e.KeyChar) && !Char.IsControl(e.KeyChar);
        }

        public bool ClosePartialBox
        {
            get { return bClosePartialBox; }
        }

    }
}
