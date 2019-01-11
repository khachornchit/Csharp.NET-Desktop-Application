using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;

namespace Testers.TagMeter
{
    public partial class FormTagMeterParameter : Form
    {
        DataGridView dataGridView = null;
        string sFilePath = null;
        string sErrorMessage;
        string[] asParameter;
        bool bSave = false;

        public FormTagMeterParameter(string sFilePath)
        {
            InitializeComponent();
            this.sFilePath = sFilePath;
        }

        public FormTagMeterParameter(DataGridView dataGridView)
        {
            InitializeComponent();
            this.dataGridView = dataGridView;
        }

        private void FormTagMeterParameter_Shown(object sender, EventArgs e)
        {
            if (sFilePath != null)
            {
                if (ImportParameter(dataGridViewTagMeterParameter, sFilePath))
                {
                    btnSave.Enabled = true;
                }
                else
                {
                    MessageBox.Show("Cannot open Tag Meter parameter.\n" + sErrorMessage, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else if(dataGridView != null)
            {
                for (int iColumn = 1; iColumn < dataGridView.ColumnCount; iColumn++)
                {
                    dataGridViewTagMeterParameter.Rows.Add(dataGridView.Columns[iColumn].HeaderText, dataGridView.Rows[dataGridView.CurrentCell.RowIndex].Cells[iColumn].Value);
                    //dataGridViewTagMeterParameter.Rows[iColumn - 1].Cells[0].Value = dataGridView.Columns[iColumn].HeaderText;
                    //dataGridViewTagMeterParameter.Rows[iColumn - 1].Cells[1].Value = dataGridView.Rows[dataGridViewTagMeterParameter.CurrentCell.RowIndex].Cells[iColumn].Value;
                }
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            asParameter = new string[dataGridViewTagMeterParameter.RowCount];
            for (int iRow = 0; iRow < dataGridViewTagMeterParameter.RowCount; iRow++)
            {
                asParameter[iRow] = dataGridViewTagMeterParameter.Rows[iRow].Cells[1].Value.ToString();
            }
            bSave = true;
            this.Close();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        public bool ImportParameter(DataGridView dataGridView, string sFilePath)
        {
            bool bSuccess = false;
            sErrorMessage = null;
            try
            {
                using (StreamReader streamReader = new StreamReader(sFilePath))
                {
                    string sParameter;
                    string[] asParameterList = null;
                    int iRow = 0;
                    while ((sParameter = streamReader.ReadLine()) != null)
                    {
                        asParameterList = sParameter.Split(new char[] { '=' });
                        asParameterList[0] = asParameterList[0].TrimEnd('\t', ' ');
                        asParameterList[1] = asParameterList[1].TrimStart(' ');
                        dataGridView.Rows.Add(asParameterList[0], asParameterList[1]);
                        dataGridView.Rows[iRow].Cells[0].ReadOnly = true;
                        iRow++;
                    }

                    bSuccess = true;
                }
            }
            catch (Exception e)
            {
                bSuccess = false;
                sErrorMessage = e.Message;
                #if DEBUG
                Console.WriteLine(sErrorMessage);
                #endif
            }

            return bSuccess;
        }

        public bool Save
        {
            get { return bSave; }
        }

        public string ErrorMessage
        {
            get { return sErrorMessage; }
        }

        public string[] Parameter
        {
            get { return asParameter; }
        }
    }
}
