using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Testers.TagProgrammer;
using Registrations;
using Utilities;

namespace Machine
{
    public partial class FormReadUID : Form
    {
        TagInformation tagInfo;
        bool bTagIsInCurrrentBox;

        public FormReadUID(TagInformation tagInfo, bool bTagIsInCurrrentBox)
        {
            InitializeComponent();
            this.tagInfo = tagInfo;
            this.bTagIsInCurrrentBox = bTagIsInCurrrentBox;
        }

        private void FormReadUID_Load(object sender, EventArgs e)
        {
            labelTagIsInCurrentBox.Visible = bTagIsInCurrrentBox;
        }

        private void FormReadUID_Shown(object sender, EventArgs e)
        {
            timerShowTagInfo.Enabled = true;
        }

        private void FormReadUID_FormClosed(object sender, FormClosedEventArgs e)
        {
            timerShowTagInfo.Enabled = false;
        }

        private void timerShowTagInfo_Tick(object sender, EventArgs e)
        {
            timerShowTagInfo.Enabled = false;
            ShowTagInfomation();
        }

        void ShowTagInfomation()
        {
            dataGridViewTagInfo.Rows.Add("UID", tagInfo.sUID);
            dataGridViewTagInfo.Rows.Add("Date Time", tagInfo.sDateTime);
            dataGridViewTagInfo.Rows.Add("Work Order", tagInfo.sWorkOrder);
            dataGridViewTagInfo.Rows.Add("Box Number", tagInfo.sBoxNumber);
            dataGridViewTagInfo.Rows.Add("PartialBox", tagInfo.sPartialBox);
            dataGridViewTagInfo.Rows.Add("QuantityPerBox", tagInfo.sQuantityPerBox);
            dataGridViewTagInfo.Rows.Add("MachineNumber", tagInfo.sMachineNumber);
            dataGridViewTagInfo.Rows.Add("TrimFrequency", tagInfo.sTrimFrequency);
            dataGridViewTagInfo.Rows.Add("TrimValue", tagInfo.sTrimValue);
            dataGridViewTagInfo.Rows.Add("F1", tagInfo.sF1);
            dataGridViewTagInfo.Rows.Add("F2", tagInfo.sF2);
            dataGridViewTagInfo.Rows.Add("F1_RST", tagInfo.sF1_RST);
            dataGridViewTagInfo.Rows.Add("F2_RST", tagInfo.sF2_RST);
            dataGridViewTagInfo.Rows.Add("Temperature", tagInfo.sTemperature);
            dataGridViewTagInfo.ClearSelection();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
