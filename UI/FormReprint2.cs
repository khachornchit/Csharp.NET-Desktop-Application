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
    public partial class FormReprint2 : Form
    {
        const string CONFIG_FILE_PATH = "config\\config.xml";

        Registration registration;
        public FormReprint2(Registration registeration)
        {
            this.registration = registeration;
            InitializeComponent();
        }

        private void btnGetWorkOrder_Click(object sender, EventArgs e)
        {
            ProductionParameters prodParam = registration.GetProductionParamFromServerDatabase(txtWorkOrder.Text);
            if (prodParam != null)
            {
                txtArticleNumber.Text = prodParam.sArticleNumber;
                txtLabelName.Text = prodParam.sProductLabel;
                txtQuantityPerBox.Text = prodParam.iQuantityPerBox.ToString();
                txtDate.Text = String.Format("{0:D2}/{1:D2}/{2:D2}", DateTime.Now.Day, DateTime.Now.Month, DateTime.Now.Year % 100);
                prodParam = null;
            }

        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            string sPrinterName = Utility.XmlReadParam(CONFIG_FILE_PATH, "/Configuration/Printer/Name");
            if (sPrinterName != null)
            {
                Printing.PrintLabel2(sPrinterName, txtArticleNumber.Text, txtBoxNo.Text, txtLabelName.Text, txtQuantityPerBox.Text, txtWorkOrder.Text);
            }
        }
    }
}
