using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ControlSystems;
using Registrations;
using Utilities;

namespace Machine
{
    public partial class FormMain : Form
    {
        public const int UID_DIGIT = 23;
        public const int WORKORDER_DIGIT = 5;
        public const int ARTICLE_DIGIT = 12;

        #region <-+- Private Object -+->
        StatusBar statusBar;
        StatusBarPanel statusBarPanel;
        StatusBarPanel statusBarPanelDate;
        StatusBarPanel statusBarPanelTime;
        StatusBarPanel statusBarUser;
        BackgroundWorker bgwInitial;
        BackgroundWorker bgwTask;
        ProductionParameters prodParam;
        ProductionVariable prodVar;
        #endregion

        public FormMain()
        {
            InitializeComponent();
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");
            if (XSystem.System.IsProcessRuning())
            {
                this.Close();
                return;
            }

            if (!VerifyDateTime())
            {
                this.Close();
                return;
            }

            statusBar = new StatusBar();
            statusBarPanel = new StatusBarPanel();

            statusBarPanel.BorderStyle = StatusBarPanelBorderStyle.Sunken;
            statusBarPanel.AutoSize = StatusBarPanelAutoSize.Spring;

            statusBarPanelDate = new StatusBarPanel();
            statusBarPanelDate.BorderStyle = StatusBarPanelBorderStyle.Sunken;
            statusBarPanelDate.Text = System.DateTime.Today.ToShortDateString();
            statusBarPanelDate.AutoSize = StatusBarPanelAutoSize.Contents;
            statusBarPanelDate.Alignment = HorizontalAlignment.Center;

            statusBarPanelTime = new StatusBarPanel();
            statusBarPanelTime.BorderStyle = StatusBarPanelBorderStyle.Sunken;
            statusBarPanelTime.Text = DateTime.Now.ToLongTimeString();
            statusBarPanelTime.AutoSize = StatusBarPanelAutoSize.Contents;
            statusBarPanelTime.Alignment = HorizontalAlignment.Center;

            statusBarUser = new StatusBarPanel();
            statusBarUser.BorderStyle = StatusBarPanelBorderStyle.Sunken;
            statusBarUser.Text = "Logout";
            statusBarUser.AutoSize = StatusBarPanelAutoSize.Contents;
            statusBarUser.Alignment = HorizontalAlignment.Center;

            statusBar.Panels.Add(statusBarPanel);
            statusBar.Panels.Add(statusBarUser);
            statusBar.Panels.Add(statusBarPanelDate);
            statusBar.Panels.Add(statusBarPanelTime);

            statusBar.ShowPanels = true;
            this.Controls.Add(statusBar);
            dataGridViewRegistration.Columns[dataGridViewRegistration.ColumnCount - 1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            //prodParam = new ProductionParameters();
        }

        private void FormMain_Shown(object sender, EventArgs e)
        {
            this.Enabled = false;
            if (LoadConfig())
            {
                Initial();
            }
            else
            {
                iUserID = 0;
                this.Close();
            }
        }

        private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (iUserID > 0)
            {
                MessageBox.Show("Please logout.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            e.Cancel = iUserID > 0;
        }

        private void FormMain_FormClosed(object sender, FormClosedEventArgs e)
        {
            Release();
        }

        private void timerMain_Tick(object sender, EventArgs e)
        {
            statusBarPanelDate.Text = DateTime.Today.ToShortDateString();
            statusBarPanelTime.Text = DateTime.Now.ToLongTimeString();
        }
        
        private void btnStart_Click(object sender, EventArgs e)
        {
            if (VerifyDateTime())
            {
                if (labelWorkOrder.Text.Length == WORKORDER_DIGIT)
                {
                    if (!prodParam.bPartialBoxClosed)
                    {
                        btnPause.BackColor = this.BackColor;

                        if (prodVar.iBoxNumber > iEndingBoxNumber)
                        {
                            MessageBox.Show("Box number is over " + iEndingBoxNumber, "Stop", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        }
                        else
                        {
                            if (prodVar.iQuantityInBox < prodParam.iQuantityPerBox || prodVar.iQuantityInBox == 0)
                            {
                                System.Windows.Forms.DialogResult dlgResult = System.Windows.Forms.DialogResult.No;
                                if (ctrlSys.QuantityWorked >= ctrlSys.WorkOrderTarget)
                                {
                                    dlgResult = MessageBox.Show(String.Format("Work order finish Qunatity worked [{0}] >= Work order target [{1}] \n\nDo you want to continue ?", ctrlSys.QuantityWorked, ctrlSys.WorkOrderTarget), "Stop", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                                }
                                else
                                {
                                    dlgResult = System.Windows.Forms.DialogResult.Yes;
                                }

                                if (dlgResult == System.Windows.Forms.DialogResult.Yes)
                                {
                                    if (prodParam.bRegister)
                                    {
                                        if (labelUID.Text.Length == UID_DIGIT && labelEndingUID.Text.Length == UID_DIGIT)
                                        {
                                            if (Utility.CompareUID(labelUID.Text, labelEndingUID.Text) <= 0)
                                            {
                                                if (dataGridViewRegistration.RowCount == 0)
                                                {
                                                    int iBoxNumber = registration.GetLastBoxNumberFromServerDatabase();
                                                    if (iBoxNumber >= 0)
                                                    {
                                                        if (iBoxNumber == 0)
                                                        {
                                                            prodVar.iBoxNumber = iMachineNumber * BOX_NUMBER_MULTIPLIER + 1;//Fist box number initial
                                                        }
                                                        else
                                                        {
                                                            prodVar.iBoxNumber = iBoxNumber + 1;
                                                        }
                                                        labelBoxNumber.Text = prodVar.iBoxNumber.ToString();
                                                        Start();
                                                    }
                                                    else
                                                    {

                                                    }
                                                }
                                                else
                                                {
                                                    Start();
                                                }
                                            }
                                            else
                                            {
                                                MessageBox.Show("UID is over ending UID.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                            }
                                        }
                                        else
                                        {
                                            MessageBox.Show("Cannot start UID is not correct.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                        }
                                    }
                                    else
                                    {
                                        Start();
                                    }
                                }
                            }
                            else
                            {
                                if (prodVar.iQuantityInBox == prodParam.iQuantityPerBox)
                                {
                                    if (prodParam.bRegister)
                                    {
                                        if (registration.AddBoxNumberToBoxTable(labelBoxNumber.Text, labelWorkOrder.Text, prodVar.iQuantityInBox, DateTime.Now))
                                        {
                                            if (registration.TransferDatabaseToServer(labelWorkOrder.Text))
                                            {
                                                PrintLabel(prodParam.iLabelLayout);
                                                ctrlSys.BoxFinished = true;
                                                BoxFinished();
                                            }
                                            else
                                            {
                                                MessageBox.Show("Transfer database fail!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                            }
                                        }
                                        else
                                        {
                                            MessageBox.Show("Cannot add new box number to database.\n" + registration.ErrorMessage + "\nCall ENGINEER", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                        }
                                    }
                                    else
                                    {
                                        if (registration.TransferDatabase2ToServer(labelWorkOrder.Text))
                                        {
                                            PrintLabel(prodParam.iLabelLayout);
                                            ctrlSys.BoxFinished = true;
                                            BoxFinished();
                                        }
                                        else
                                        {
                                            MessageBox.Show("Transfer database fail!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                        }
                                    }
                                }
                                else
                                {
                                    MessageBox.Show(String.Format("Work order finish quantity in box [{0}] > quantity per box [{1}]\nCall ENGINEER.", prodVar.iQuantityInBox, prodParam.iQuantityPerBox), "Stop", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                                }
                            }
                        }
                    }
                    else
                    {
                        MessageBox.Show("Partial box closed already.", "Stop", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }

                }
                else if (iUserID == Login.USER_ID_QA)
                {
                    Start();
                }
                else
                {
                    MessageBox.Show("Please select work order.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            Stop();
            //btnStop.Enabled = false;
            //btnStart.Enabled = true;
            //btnLogout.Enabled = true;
            //btnPause.Enabled = false;
            //btnPause.BackColor = this.BackColor;
        }

        private void bgwInitial_DoWork(object sender, DoWorkEventArgs e)
        {
            bgwInitial.ReportProgress(10);
            System.Threading.Thread.Sleep(100);
            if (tagMeter.Initial)
            {
                if (tagMeter.GetVersion())
                {
                    bgwInitial.ReportProgress(100);
                }
                else
                {
                    bgwInitial.ReportProgress(101);
                }                
            }
            else
            {
                bgwInitial.ReportProgress(102);
            }

            if (tagProgrammer.Initial)
            {
                if (tagProgrammer.GetVersion())
                {
                    bgwInitial.ReportProgress(200);
                }
                else
                {
                    bgwInitial.ReportProgress(201);
                }
            }
            else
            {
                bgwInitial.ReportProgress(202);
            }
        }

        private void bgwInitial_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {            
            switch (e.ProgressPercentage)
            {
                case 10:
                    if (registration.Initial())
                    {
                        bInitSuccess = true;
                    }
                    else
                    {
                        txtMachineMessage.Text += "Database registration initial error!\r\n" + registration.ErrorMessage;
                        sMessage += "Registration initial error!\n";
                        if (registration.ErrorMessage != null)
                        {
                            sMessage += registration.ErrorMessage + "\n\n";
                        }
                        else
                        {
                            sMessage += "\n";
                        }
                        bInitSuccess = false;
                    }
                    break;

                case 100:
                    txtTagMeter.Text = tagMeter.Data;
                    txtMachineMessage.Text += NEWLINE + "Tag meter initial is successful.";
                    bInitSuccess &= true;
                    break;

                case 101:
                    txtTagMeter.Text = tagMeter.Data;
                    txtMachineMessage.Text += NEWLINE + "Tag meter initial error!\r\n" + tagMeter.ErrorMessage;
                    sMessage += "Tag meter initial error!\n";
                    if (tagMeter.ErrorMessage != null)
                    {
                        sMessage += tagMeter.ErrorMessage + "\n\n";
                    }
                    else
                    {
                        sMessage += "\n";
                    }
                    bInitSuccess = false;
                    break;
                case 102:
                    txtMachineMessage.Text += NEWLINE + "Tag meter initial error!\r\n" + tagMeter.ErrorMessage;
                    sMessage += "Tag meter initial error!\n";
                    if (tagMeter.ErrorMessage != null)
                    {
                        sMessage += tagMeter.ErrorMessage + "\n\n";
                    }
                    else
                    {
                        sMessage += "\n";
                    }
                    bInitSuccess = false;
                    break;
                
                case 200:
                    txtTagProgrammer.Text = tagProgrammer.Data;
                    txtMachineMessage.Text +=  NEWLINE + "Tag programmer initial is successful.";
                    bInitSuccess &= true;
                    break;

                case 201:
                    txtTagProgrammer.Text = tagProgrammer.Data;
                    txtMachineMessage.Text += NEWLINE + "Tag programmer initial error!\r\n" + tagProgrammer.ErrorMessage;
                    sMessage += "Tag programmer initial error!\n";
                    if (tagProgrammer.ErrorMessage != null)
                    {
                        sMessage += tagProgrammer.ErrorMessage + "\n\n";
                    }
                    else
                    {
                        sMessage += "\n";
                    }
                    bInitSuccess = false;
                    break;

                case 202:
                    txtTagProgrammer.Text = tagProgrammer.Data;
                    txtMachineMessage.Text += NEWLINE + "Tag programmer initial error!\r\n" + tagProgrammer.ErrorMessage;
                    sMessage += "Tag programmer initial error!\n";
                    if (tagProgrammer.ErrorMessage != null)
                    {
                        sMessage += tagProgrammer.ErrorMessage + "\n\n";
                    }
                    else
                    {
                        sMessage += "\n";
                    }
                    bInitSuccess = false;
                    break;
            }
        }

        private void bgwInitial_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            this.Enabled = true;

            if (ctrlSys.IsReady)
            {
                txtMachineMessage.Text += NEWLINE + "Control system initial is successful.";
                bInitSuccess &= true;
            }
            else
            {
                txtMachineMessage.Text += NEWLINE + "Control system initial error!";
                sMessage += "Control system initial error!\n\n";
                bInitSuccess = false;
            }

            if (bInitSuccess)
            {
                txtMachineMessage.Text += NEWLINE + "Machine initial is successful.";
            }
            else
            {
                MessageBox.Show(sMessage, "Initial error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            timerMain.Enabled = true;
            if (UserLogin())
            {
                if (!bInitSuccess)
                {
                    if (iUserID < Login.USER_ID_ENGINEER)
                    {
                        iUserID = 0;
                        this.Close();
                    }
                }
            }
            else
            {
                this.Close();
            }
        }

        private void btnLogout_Click(object sender, EventArgs e)
        {
            if (!UserLogin())
            {
                this.Close();
            }
        }

        private void btnWorkOrder_Click(object sender, EventArgs e)
        {
            FormWorkOrder formWorkOrder = new FormWorkOrder(iUserID, this.registration);
            formWorkOrder.ShowDialog();

            if (formWorkOrder.WorkOrderSelected)
            {
                prodParam = formWorkOrder.prodParam;
                prodVar = formWorkOrder.prodVar;

                if(prodVar.iBoxNumber == 0)
                {
                    switch (prodParam.iLabelLayout)
                    {
                        case 0:
                            prodVar.iBoxNumber = iMachineNumber * BOX_NUMBER_MULTIPLIER + 1;//Fist box number initial
                            break;
                        case 1:
                            prodVar.iBoxNumber = 1;
                            break;
                    }
                }

                switch (prodParam.iLabelLayout)
                {
                    case 0:
                        labelBoxNumber.Text = String.Format("{0}", prodVar.iBoxNumber);
                        break;
                    case 1:
                        labelBoxNumber.Text = String.Format("{0:D5}", prodVar.iBoxNumber);
                        break;
                }

                if (prodParam.iPowerLevel < 25)
                {
                    prodParam.iPowerLevel = 25;
                }
                labelArticleNumber.Text = prodParam.sArticleNumber;
                labelWorkOrder.Text = prodParam.iWorkOrder.ToString();
                labelMachineNumber.Text = String.Format("{0}", iMachineNumber);
                labelProductLabel.Text = prodParam.sProductLabel;
                labelWorkOrderTarget.Text = prodParam.iWorkOrderTarget.ToString();
                labelQuantityWorked.Text = prodVar.iQuantityWorked.ToString();
                labelQuantityPerBox.Text = prodParam.iQuantityPerBox.ToString();
                labelCustomerName.Text = prodParam.sCustormerName;
                labelPowerLevel.Text = prodParam.iPowerLevel.ToString();
                labelTrimmingFrequency.Text = String.Format("{0}.{1}", prodParam.iTrimmingFrequency / 1000, prodParam.iTrimmingFrequency % 1000);
                labelGood.Text = prodVar.iQuantityInBox.ToString();
                labelTagType.Text = Registration.ParseTagTypeToString(prodParam.iTagType);
                chkRegister.Checked = prodParam.bRegister;
                chkTest.Checked = prodParam.bUseTagMeter && prodParam.bTesting;
                chkProgram.Checked = prodParam.bUserTagProgrammer;
                chkLock.Checked = prodParam.iProgramOption == 1;
                chkTrim.Checked = prodParam.iProgramOption == 2 || (prodParam.iTagType == Registration.TAG_TYPE_HDX_PLUSE && prodParam.iTrimmingFrequency > 0);
                labelBeginningUID.Text = prodParam.sBeginningUID;
                labelEndingUID.Text = prodParam.sEndingUID;
                labelUID.Text = prodParam.sUID;

                ctrlSys.WorkOrderTarget = prodParam.iWorkOrderTarget;
                ctrlSys.QuantityPerBox = prodParam.iQuantityPerBox;
                ctrlSys.QuantityWorked = prodVar.iQuantityWorked;
                ctrlSys.PartInGoodBox = prodVar.iQuantityInBox;
                ctrlSys.EnRegister = prodParam.bRegister;
                ctrlSys.EnTagProgrammer = prodParam.bUserTagProgrammer;
                ctrlSys.EnTagMeter = prodParam.bUseTagMeter;
                ctrlSys.SktipTagMeter = !prodParam.bTesting;

                dataGridViewRegistration.Visible = chkRegister.Checked;
                dataGridViewTesting.Visible = !chkRegister.Checked;
                dataGridViewRegistration.Rows.Clear();
                dataGridViewTesting.Rows.Clear();

                if (chkRegister.Checked)
                {
                    int iReject1 = registration.CountBadUIDinLocalDatabase(labelWorkOrder.Text);
  
                    ctrlSys.PartInRejectBox[0] = iReject1;
                    ctrlSys.PartInRejectBox[1] = 0;
                    ctrlSys.PartInRejectBox[2] = 0;
                    ctrlSys.PartInRejectBox[3] = 0;
                    labelReject1.Text = iReject1.ToString();
                    labelReject2.Text = "0";
                    labelReject3.Text = "0";
                    labelReject4.Text = "0";

                    grpBoxMain.Text = "Registratoin data";
                    registration.RetrieveRegistrationDataFromLocalDatabase(dataGridViewRegistration, labelWorkOrder.Text, labelBoxNumber.Text);
                }
                else
                {
                    int iReject1 = registration.CountReject1TestingTagInLocalDatabase(labelWorkOrder.Text);
                    int iReject2 = registration.CountReject2TestingTagInLocalDatabase(labelWorkOrder.Text);
                    int iReject3 = registration.CountReject3TestingTagInLocalDatabase(labelWorkOrder.Text);
                    int iReject4 = registration.CountReject4TestingTagInLocalDatabase(labelWorkOrder.Text);

                    ctrlSys.PartInRejectBox[0] = iReject1;
                    ctrlSys.PartInRejectBox[1] = iReject2;
                    ctrlSys.PartInRejectBox[2] = iReject3;
                    ctrlSys.PartInRejectBox[3] = iReject4;
                    labelReject1.Text = iReject1.ToString();
                    labelReject2.Text = iReject2.ToString();
                    labelReject3.Text = iReject3.ToString();
                    labelReject4.Text = iReject4.ToString();

                    grpBoxMain.Text = "Testing data";
                    registration.RetrieveTestingDataFromLocalDatabase(dataGridViewTesting, labelWorkOrder.Text, labelBoxNumber.Text);
                }

                if (chkTest.Checked)
                {
                    bInitialTagMeter = false;
                    btnStart.Enabled = false;
                    this.Cursor = Cursors.WaitCursor;
                    bgwInitTagMeter.RunWorkerAsync();
                }
                else
                {
                    btnStart.Enabled = true;
                }
            }
        }

        private void btnArticle_Click(object sender, EventArgs e)
        {
            FormArticle formArticle = new FormArticle(registration);
            formArticle.ShowDialog();
        }

        private void btnClosePartialBox_Click(object sender, EventArgs e)
        {
            if (labelWorkOrder.Text.Length == WORKORDER_DIGIT)
            {
                FormClosePartialBox formClosePartialBox = new FormClosePartialBox(labelBoxNumber.Text);
                formClosePartialBox.ShowDialog();
                if (formClosePartialBox.ClosePartialBox)
                {
                    if (prodParam.bRegister)
                    {
                        int iTotalTagInLocal = registration.CountTagInLocalDatabase(labelWorkOrder.Text);
                        if (iTotalTagInLocal > 0)
                        {
                            int iTagInBox = registration.CountTagInLocalDatabase(labelWorkOrder.Text, labelBoxNumber.Text);
                            if (iTagInBox > 0)
                            {
                                if (iTotalTagInLocal == iTagInBox)
                                {
                                    if (iTagInBox == Convert.ToInt32(labelGood.Text))
                                    {                                        
                                        if (registration.AddBoxNumberToBoxTable(labelBoxNumber.Text, labelWorkOrder.Text, iTagInBox, DateTime.Now))
                                        {
                                            if (registration.TransferDatabaseToServer(labelWorkOrder.Text))
                                            {
                                                //if (registration.UpdatePartialBoxClosedStatusInWorkOrderTable(labelWorkOrder.Text, 1))
                                                {
                                                    PrintLabel(prodParam.iLabelLayout);
                                                    dataGridViewRegistration.Rows.Clear();
                                                    prodParam.bPartialBoxClosed = true;
                                                }
                                            }
                                            else
                                            {
                                                MessageBox.Show("Cannot update partial box closed status in work order list database.\nCall ENGINEER." + registration.ErrorMessage, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                            }
                                        }
                                        else
                                        {
                                            MessageBox.Show("Cannot add new box number to database.\n" + registration.ErrorMessage + "\nCall ENGINEER", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                        }
                                    }
                                    else
                                    {
                                        MessageBox.Show("Quantity in local database is not equal quantity in box.\nCall ENGINEER." + registration.ErrorMessage, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    }
                                }
                                else
                                {
                                    MessageBox.Show("Quantity in local database is not corrext.\nCall ENGINEER." + registration.ErrorMessage, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                }
                            }
                        }
                    }
                    else
                    {
                        if(registration.TransferDatabase2ToServer(labelWorkOrder.Text))
                        {
                            //if (registration.UpdatePartialBoxClosedStatusInWorkOrderTable(labelWorkOrder.Text, 1))
                            {
                                PrintLabel(prodParam.iLabelLayout);
                                dataGridViewTesting.Rows.Clear();
                                prodParam.bPartialBoxClosed = true;
                            }
                            //else
                            //{
                            //    MessageBox.Show("Cannot update partial box closed status in work order list database.\n" + registration.ErrorMessage, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            //}
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show("Please select work order.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btnRePrint_Click(object sender, EventArgs e)
        {
            if (labelWorkOrder.Text.Length == WORKORDER_DIGIT)
            {
                if (prodParam != null)
                {
                    switch (prodParam.iLabelLayout)
                    {
                        case 0:
                            FormReprint formReprint = new FormReprint(registration);
                            formReprint.ShowDialog();
                            break;
                        case 1:
                            FormReprint2 formReprint2 = new FormReprint2(registration);
                            formReprint2.ShowDialog();
                            break;
                    }
                }
            }
            else
            {
                MessageBox.Show("Please select work order.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btnReadUID_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                btnReadUID.ContextMenuStrip = contextMenuStripRead;
                btnReadUID.ContextMenuStrip.Show(btnReadUID, new System.Drawing.Point(0, btnReadUID.Height));
            }
            else
            {
                btnReadUID.ContextMenuStrip = null;
            }
        }

        private void btnQA_ReWork_Click(object sender, EventArgs e)
        {
            FormChangeUID formChangeUID = new FormChangeUID(registration, tagProgrammer);
            formChangeUID.ShowDialog();
        }

        private void btnIO_Click(object sender, EventArgs e)
        {
            ctrlSys.ShowIO(iUserID);
        }

        private void btnPause_Click(object sender, EventArgs e)
        {
            ctrlSys.Pause();            
            if (ctrlSys.Paused)
            {
                btnPause.BackColor = Color.Orange;
            }
            else
            {
                btnPause.BackColor = this.BackColor;
            }
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            if(iUserID == Login.USER_ID_QA)
            {
                ctrlSys.Reset();
                labelGood.Text = "0";
                labelReject1.Text = "0";
                labelReject2.Text = "0";
                labelReject3.Text = "0";
                labelReject4.Text = "0";
                dataGridViewTesting.Rows.Clear();
                dataGridViewRegistration.Rows.Clear();
            }
            ctrlSys.AlarmReset();
        }

        private void btnSetting_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                btnSetting.ContextMenuStrip = contextMenuStripSetting;
                btnSetting.ContextMenuStrip.Show(btnSetting, new System.Drawing.Point(0, btnSetting.Height));
            }
            else
            {
                btnSetting.ContextMenuStrip = null;
            }
        }

        private void toolStripMenuItemTagMeter_Click(object sender, EventArgs e)
        {
            tagMeter.CommSetting();
        }

        private void toolStripMenuItemTagProgrammer_Click(object sender, EventArgs e)
        {
            tagProgrammer.CommSetting();
        }

        private void toolStripMenuItemPrinter_Click(object sender, EventArgs e)
        {
            FormPrinter formPrinter = new FormPrinter();
            formPrinter.Show();
        }

        private void toolStripMenuItemReadFDX_Click(object sender, EventArgs e)
        {
            string sTagUID = tagProgrammer.ReadFDX();
            if (sTagUID != null)
            {
                MessageBox.Show("UID is " + sTagUID, "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("Cannot read UID.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void toolStripMenuItemReadHDX_Click(object sender, EventArgs e)
        {
            string sTagUID = tagProgrammer.ReadHDX();
            if (sTagUID != null)
            {
                sTagUID = Utility.ConvertUID_HexadecimalToDecimal(sTagUID);
            }
            ShowTagInfo(sTagUID);
        }

        private void toolStripMenuItemReadHDXPlus_Click(object sender, EventArgs e)
        {
            string sTagUID = tagProgrammer.ReadHDX_Plus();
            if (sTagUID != null)
            {
                sTagUID = Utility.ConvertUID_HexadecimalToDecimal(sTagUID);
            }
            ShowTagInfo(sTagUID);
        }

        void ShowTagInfo(string sTagUID)
        {
            if (sTagUID != null)
            {
                TagInformation tagInfo = registration.GetTagInformationFormLocalDatabase(sTagUID);
                if (tagInfo == null)
                {
                    tagInfo = registration.GetTagInformationFormServerDatabase(sTagUID);
                }
                if (tagInfo != null)
                {
                    bool bTagInCurrentBox = tagInfo.sBoxNumber == labelBoxNumber.Text;
                    FormReadUID formReadUID = new FormReadUID(tagInfo, bTagInCurrentBox);
                    formReadUID.ShowDialog();
                }
                else
                {
                    MessageBox.Show("UID is " + sTagUID + "\n\nNo information in database.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            else
            {
                MessageBox.Show("Cannot read UID.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void labelReject3_Click(object sender, EventArgs e)
        {

        }
    }
}
