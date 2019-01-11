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
    public partial class FormTagInformation : Form
    {
        string[] asTagInfo;
        bool bTagIsInCurrrentBox;
        public FormTagInformation(string[] asTagInfo, bool bTagIsInCurrrentBox)
        {
            InitializeComponent();
            this.asTagInfo = asTagInfo;
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
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
