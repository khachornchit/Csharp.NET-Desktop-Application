using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Registrations;
using Utilities;

namespace Machine
{
    public partial class FormReprint : Form
    {
        const string CONFIG_FILE_PATH = "config\\config.xml";

        Registration registeration;
        string sPrinterName;

        public FormReprint(Registration registeration)
        {
            this.registeration = registeration;
            InitializeComponent();
        }

        private void FormReprint_Load(object sender, EventArgs e)
        {
        }

        private void FormReprint_Shown(object sender, EventArgs e)
        {
            sPrinterName = Utility.XmlReadParam(CONFIG_FILE_PATH, "/Configuration/Printer/Name");
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            if (txtWorkOrder.Text == String.Empty)
            {
                MessageBox.Show("Please enter Work Order", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (Convert.ToInt32(txtBoxNo.Text) > 0)
            {
                Printing.PrintLabel(
                    sPrinterName,
                    txtLabelName.Text,
                    txtArticleNumber.Text,
                    txtBatch.Text,
                    txtBoxNo.Text,
                    txtWorkOrder.Text,
                    txtDate.Text,
                    txtQuantityPerBox.Text
                    );
            }
        }

        private void btnGetWorkOrder_Click(object sender, EventArgs e)
        {
            ProductionParameters prodParam = registeration.GetProductionParamFromServerDatabase(txtWorkOrder.Text);
            if(prodParam != null)
            {
                txtArticleNumber.Text = prodParam.sArticleNumber;
                txtLabelName.Text = prodParam.sProductLabel;
                txtQuantityPerBox.Text = prodParam.iQuantityPerBox.ToString();
                txtDate.Text = String.Format("{0:D2}/{1:D2}/{2:D2}", DateTime.Now.Day, DateTime.Now.Month, DateTime.Now.Year % 100);
                prodParam = null;
            }
        }
    }
}
