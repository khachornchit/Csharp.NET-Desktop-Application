using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.Diagnostics;
using ControlSystems;
using Registrations;
using Testers.TagMeter;
using Testers.TagProgrammer;
using Utilities;

namespace Machine
{
    public partial class FormMain : Form
    {
        #region <-+- Structure -+->

        #endregion

        #region <-+- Constance -+- >
        const int BOX_NUMBER_MULTIPLIER = 10000000;
        const int BOX_NUMBER_ENDING_CONST = 9999999;

        const int MESSAGE_TYPE_UPDATE_PARAMETER = OperationControl.MESSAGE_TYPE_UPDATE_PARAMETER;
        const int MESSAGE_TYPE_INFORMATION = OperationControl.MESSAGE_TYPE_INFORMATION;
        const int MESSAGE_TYPE_WARNING = OperationControl.MESSAGE_TYPE_WARNING;
        const int MESSAGE_TYPE_ERROR = OperationControl.MESSAGE_TYPE_ERROR;

        const int SYSTEM_IDLE = OperationControl.SYSTEM_IDLE;
        const int SYSTEM_STOP = OperationControl.SYSTEM_STOP;
        const int SYSTEM_ESTOP = OperationControl.SYSTEM_ESTOP;
        const int SYSTEM_NO_POWER = OperationControl.SYSTEM_NO_POWER;
        const int SYSTEM_RUNNING = OperationControl.SYSTEM_RUNNING;
        const int SYSTEM_NO_PNEUMATIC_AIR = OperationControl.SYSTEM_NO_PNEUMATIC_AIR;
        const int SYSTEM_LINEAR_ACTUATOR_ALARM = OperationControl.SYSTEM_LINEAR_ACTUATOR_ALARM;
        const string SERVER_IP = "192.168.1.14";//"192.168.1.14";
        const string QA_WORK_ORDER = "0";
        #endregion

        #region <-+- Private Constance -+->
        const string NEWLINE = "\r\n";
        #endregion

        #region <-+- Protected Object -+->
        #endregion

        #region <-+- Protected Variable -+->
        #endregion

        #region <-+- Private Object -+->
        private BackgroundWorker bgwTagMeter;
        private BackgroundWorker bgwTagProgrammer;
        private BackgroundWorker bgwRegister;
        private BackgroundWorker bgwBoxFinish;
        private BackgroundWorker bgwInitTagMeter;
        private OperationControl ctrlSys;
        private Registration registration;
        private TagProgrammer tagProgrammer;
        private TagMeter tagMeter;
        #endregion

        #region <-+- Private Variable -+->
        int iMachineNumber;
        int iEndingBoxNumber;
        int iUserID;
        bool bInitSuccess = false;
        bool bInitialTagMeter = false;        
        string sMessage;
        #endregion

        #region <-+- Constructor -+->
        #endregion

        #region <-+- Private Methode -+->
        private void Initial()
        {
            bInitSuccess = false;
            bgwTask = new BackgroundWorker();
            bgwTask.WorkerSupportsCancellation = true;
            bgwTask.WorkerReportsProgress = true;
            bgwTask.DoWork += new DoWorkEventHandler(bgwTask_DoWork);
            bgwTask.ProgressChanged += new ProgressChangedEventHandler(bgwTask_ProgressChanged);
            bgwInitial = new System.ComponentModel.BackgroundWorker();
            bgwInitial.WorkerReportsProgress = true;
            bgwInitial.WorkerSupportsCancellation = true;
            bgwInitial.DoWork += new DoWorkEventHandler(bgwInitial_DoWork);
            bgwInitial.ProgressChanged += new ProgressChangedEventHandler(bgwInitial_ProgressChanged);
            bgwInitial.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bgwInitial_RunWorkerCompleted);
            bgwTagMeter = new BackgroundWorker();
            bgwTagMeter.WorkerSupportsCancellation = true;
            bgwTagMeter.DoWork += new DoWorkEventHandler(bgwTagMeter_DoWork);
            bgwTagMeter.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bgwTagMeter_RunWorkerCompleted);
            bgwTagProgrammer = new BackgroundWorker();
            bgwTagProgrammer.WorkerSupportsCancellation = true;
            bgwTagProgrammer.WorkerReportsProgress = true;
            bgwTagProgrammer.DoWork += new DoWorkEventHandler(bgwTagProgrammer_DoWork);
            bgwTagProgrammer.ProgressChanged += bgwTagProgrammer_ProgressChanged;
            bgwTagProgrammer.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bgwTagProgrammer_RunWorkerCompleted);
            bgwRegister = new BackgroundWorker();
            bgwRegister.WorkerSupportsCancellation = true;
            bgwRegister.DoWork += new DoWorkEventHandler(bgwRegister_DoWork);
            bgwRegister.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bgwRegister_RunWorkerCompleted);
            bgwInitTagMeter = new BackgroundWorker();
            bgwInitTagMeter.WorkerSupportsCancellation = true;
            bgwInitTagMeter.WorkerReportsProgress = true;
            bgwInitTagMeter.DoWork += bgwInitTagMeter_DoWork;
            bgwInitTagMeter.ProgressChanged += bgwInitTagMeter_ProgressChanged;
            bgwInitTagMeter.RunWorkerCompleted += bgwInitTagMeter_RunWorkerCompleted;
            bgwBoxFinish = new BackgroundWorker();
            bgwBoxFinish.WorkerSupportsCancellation = true;
            bgwBoxFinish.WorkerReportsProgress = true;
            bgwBoxFinish.DoWork += new DoWorkEventHandler(bgwBoxFinish_DoWork);
            bgwBoxFinish.ProgressChanged += new ProgressChangedEventHandler(bgwBoxFinish_ProgressChanged);
            bgwBoxFinish.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bgwBoxFinish_RunWorkerCompleted);

            ctrlSys = new OperationControl(bgwTask);
            registration = new Registration();
            tagProgrammer = new TagProgrammer();
            tagMeter = new TagMeter();
            sMessage = String.Empty;
            txtMachineMessage.Text = "Machine initial.";
            bgwInitial.RunWorkerAsync();
        }

        void bgwTask_DoWork(object sender, DoWorkEventArgs e)
        {
            bgwTask.ReportProgress(Convert.ToInt32(e.Argument));
        }

        const int EV_STOP = 1;
        const int EV_UPDATE_PARAMETER = 2;
        const int EV_START_TAG_METER = 3;
        const int EV_START_TAG_PROGRAMMER = 4;
        const int EV_START_REGISTER_GOOD = 5;
        const int EV_START_REGISTER_BAD = 6;
        const int EV_BOX_FINISH = 7;
        const int EV_MESSAGE = 8;
        const int EV_START = 9;
        const int EV_STOP_TAG_METER = 10;

        void bgwTask_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            switch (e.ProgressPercentage)
            {
                case EV_UPDATE_PARAMETER:
                    if (labelWorkOrder.Text.Length == WORKORDER_DIGIT)
                    {
                        labelGood.Text = ctrlSys.PartInGoodBox.ToString();
                        labelQuantityWorked.Text = ctrlSys.QuantityWorked.ToString();
                        labelReject1.Text = ctrlSys.PartInRejectBox[0].ToString();
                        labelReject2.Text = ctrlSys.PartInRejectBox[1].ToString();
                        labelReject3.Text = ctrlSys.PartInRejectBox[2].ToString();
                        labelReject4.Text = ctrlSys.PartInRejectBox[3].ToString();
                        labelBoxFinish.Visible = ctrlSys.BoxFinished;
                        labelPartEmpty.Visible = ctrlSys.PartEmpty;
                    }
                    break;

                case EV_START_TAG_METER:
                    bgwTagMeter.RunWorkerAsync();
                    break;

                case EV_STOP_TAG_METER:
                    bgwTagMeter.CancelAsync();
                    break;

                case EV_START_TAG_PROGRAMMER:
                    bgwTagProgrammer.RunWorkerAsync();
                    break;

                case EV_START_REGISTER_GOOD:
                    bgwRegister.RunWorkerAsync(true);
                    break;

                case EV_START_REGISTER_BAD:
                    bgwRegister.RunWorkerAsync(false);
                    break;

                case EV_BOX_FINISH:
                    bgwBoxFinish.RunWorkerAsync();
                    break;

                case EV_MESSAGE:
                    switch (ctrlSys.MessageType)
                    {
                        case MESSAGE_TYPE_INFORMATION:
                            MessageBox.Show(ctrlSys.Message, "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            break;
                        case MESSAGE_TYPE_WARNING:
                            MessageBox.Show(ctrlSys.Message, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            break;
                        case MESSAGE_TYPE_ERROR:
                            MessageBox.Show(ctrlSys.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            break;
                    }
                    break;

                case EV_START:
                    btnStart.Enabled = false;
                    btnReset.Enabled = false;
                    btnStop.Enabled = true;
                    btnPause.Enabled = true;
                    labelRun.Visible = true;
                    btnLogout.Enabled = false;
                    btnReadUID.Enabled = false;
                    btnWorkOrder.Enabled = false;
                    btnRePrint.Enabled = false;
                    btnClosePartialBox.Enabled = false;
                    btnQA_ReWork.Enabled = false;
                    labelRun.Text = "RUNNING";
                    break;

                case EV_STOP:
                    if (bgwTagMeter.IsBusy)
                    {
                        bgwTagMeter.CancelAsync();
                    }
                    if (bgwTagProgrammer.IsBusy)
                    {
                        bgwTagProgrammer.CancelAsync();
                    }

                    bgwTagMeter.CancelAsync();
                    bgwTagProgrammer.CancelAsync();
                    btnStop.Enabled = false;
                    btnPause.Enabled = false;
                    labelRun.Visible = false;
                    btnStart.Enabled = bInitSuccess && !ctrlSys.Alarm && (!chkTest.Checked || bInitialTagMeter);
                    //btnStart.Enabled = bInitSuccess && !ctrlSys.Alarm && bInitialTagMeter;
                    btnLogout.Enabled = true;
                    btnReset.Enabled = true;
                    btnReadUID.Enabled = true;
                    btnStop.BackColor = this.BackColor;
                    btnWorkOrder.Enabled = true;
                    btnRePrint.Enabled = true;
                    btnClosePartialBox.Enabled = true;
                    btnQA_ReWork.Enabled = true;

                    if (labelWorkOrder.Text.Length == WORKORDER_DIGIT)
                    {
                        //labelGood.Text = proParam.iTotalGoodInDatabase.ToString();
                        labelGood.Text = ctrlSys.PartInGoodBox.ToString();
                        labelQuantityWorked.Text = ctrlSys.QuantityWorked.ToString();
                    }
            
                    btnLogout.Enabled = true;

                    switch (ctrlSys.Status)
                    {
                        case SYSTEM_NO_POWER:
                            labelMessage.Text = "No 24 VDC !";
                            labelMessage.BackColor = Color.Red;
                            labelMessage.ForeColor = Color.White;
                            labelMessage.Visible = true;
                            break;
                        case SYSTEM_ESTOP:
                            labelMessage.Text = "Emergency STOP !";
                            labelMessage.BackColor = Color.Red;
                            labelMessage.ForeColor = Color.White;
                            labelMessage.Visible = true;
                            break;
                        case SYSTEM_NO_PNEUMATIC_AIR:
                            labelMessage.Text = "No Pneumatic Air !";
                            labelMessage.BackColor = Color.Red;
                            labelMessage.ForeColor = Color.White;
                            labelMessage.Visible = true;
                            break;
                        case SYSTEM_LINEAR_ACTUATOR_ALARM:
                            labelMessage.Text = "Linear Actuator Alarm !";
                            labelMessage.BackColor = Color.Red;
                            labelMessage.ForeColor = Color.White;
                            labelMessage.Visible = true;
                            break;
                        case SYSTEM_STOP:
                            labelMessage.Visible = false;
                            labelMessage.Text = String.Empty;
                        break;
                    }
                    break;
            }
        }

        void bgwInitTagMeter_DoWork(object sender, DoWorkEventArgs e)
        {
            if (tagMeter.IsAlive())
            {
                if (!bInitialTagMeter)
                {
                    string[,] asTagMeterParameter = new string[35, 2];
                    if (registration.GetTagMeterParameterFromServerDatabase(ref asTagMeterParameter, labelArticleNumber.Text))
                    {
                        string[,] asTagMeterParamInFlash = tagMeter.GetFlashParam();
                        if (asTagMeterParamInFlash != null)
                        {
                            bool bFlashParamNotChange = false;
                            for(int iIndex = 0; iIndex < asTagMeterParameter.Length / 2 ; iIndex ++)
                            {
                                if (asTagMeterParameter[iIndex, 0] == asTagMeterParamInFlash[iIndex, 0] && asTagMeterParameter[iIndex, 1] == asTagMeterParamInFlash[iIndex, 1])
                                {
                                    bFlashParamNotChange = true;
                                }
                                else
                                {
                                    if (iIndex == 20)
                                    {
                                        if (asTagMeterParameter[20, 0] == asTagMeterParamInFlash[21, 0] && asTagMeterParameter[20, 1] == asTagMeterParamInFlash[21, 1])
                                        {
                                            bFlashParamNotChange = true;
                                        }
                                        else
                                        {
                                            bFlashParamNotChange = false;
                                            break;
                                        }
                                    }
                                    else if(iIndex == 21)
                                    {
                                        if (asTagMeterParameter[21, 0] == asTagMeterParamInFlash[20, 0] && asTagMeterParameter[21, 1] == asTagMeterParamInFlash[20, 1])
                                        {
                                            bFlashParamNotChange = true;
                                        }
                                        else
                                        {
                                            bFlashParamNotChange = false;
                                            break;
                                        }
                                    }
                                    else
                                    {
                                        bFlashParamNotChange = false;
                                        break;
                                    }
                                }
                            }

                            if(bFlashParamNotChange)
                            {
                                if (tagMeter.LoadParameter(asTagMeterParameter))
                                {
                                    bgwInitTagMeter.ReportProgress(1);
                                    if (tagMeter.Calibration())
                                    {
                                        bgwInitTagMeter.ReportProgress(3);
                                    }
                                    else
                                    {
                                        bgwInitTagMeter.ReportProgress(4);
                                        MessageBox.Show("Cannot start tag meter frequency calibration.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    }
                                    bInitialTagMeter = true;
                                }
                                else
                                {
                                    MessageBox.Show("Load tag programmer parameter fail!\nTry again.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                }
                            }
                            else
                            {
                                if (tagMeter.LoadParameter(asTagMeterParameter))
                                {
                                    bgwInitTagMeter.ReportProgress(1);
                                    if (tagMeter.SaveConfigToFlash())
                                    {
                                        bgwInitTagMeter.ReportProgress(2);
                                        if (tagMeter.Calibration())
                                        {
                                            bgwInitTagMeter.ReportProgress(3);
                                        }
                                        else
                                        {
                                            bgwInitTagMeter.ReportProgress(4);
                                            MessageBox.Show("Cannot start tag meter frequency calibration.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                        }
                                        bInitialTagMeter = true;
                                    }
                                    else
                                    {
                                        MessageBox.Show("Cannot save tag meter parameter to flash.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    }
                                }
                                else
                                {
                                    MessageBox.Show("Load tag programmer parameter fail!\nTry again.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                }
                            }
                        }
                        else
                        {
                            MessageBox.Show("Cannot read tag meter parameter!\nTry again.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    else
                    {
                        MessageBox.Show("No tag meter parameter.\nCall ENGINEER", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    if (tagMeter.LoadConfigFormFlash())
                    {
                        bgwInitTagMeter.ReportProgress(10);
                        if (tagMeter.Calibration())
                        {
                            bgwInitTagMeter.ReportProgress(11);
                            //ctrlSys.Start();
                        }
                        else
                        {
                            MessageBox.Show("Cannot start tag meter frequency calibration.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    else
                    {
                        MessageBox.Show("Cannot load tag meter parameter", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Not response form tag meter.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            e.Cancel = bgwInitTagMeter.CancellationPending;
        }

        void bgwInitTagMeter_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            switch(e.ProgressPercentage)
            {
                case 1:
                    txtMachineMessage.Text = "Tag meter load configuration is successful.\r\n";
                    break;
                case 2:
                    txtMachineMessage.Text = "Tag meter save configuration is successful.\r\n";
                    break;
                case 3:
                    txtTagMeter.Text = tagMeter.Data;
                    txtMachineMessage.Text += "Tag meter calibration is successful.\r\n";
                    txtMachineMessage.Text += txtTagMeter.Text;
                    break;
                case 4:
                    txtMachineMessage.Text += "Cannot start tag meter frequency calibration.\r\n";
                    break;
                case 10:
                    txtMachineMessage.Text += "Tag meter configuration is successful.\r\n";
                    break;
                case 11:
                    ctrlSys.Start();
                    txtTagMeter.Text = tagMeter.Data;
                    txtMachineMessage.Text += "Tag meter calibration is successful.\r\n";
                    txtMachineMessage.Text += txtTagMeter.Text;
                    btnLogout.Enabled = false;
                    btnStop.Enabled = true;
                    btnPause.Enabled = true;
                    bInitialTagMeter = true;
                    break;
            }
        }

        void bgwInitTagMeter_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            btnStart.Enabled = bInitSuccess && !ctrlSys.Alarm && (!chkTest.Checked || bInitialTagMeter);

            if (!btnStart.Enabled)
            {
                this.Cursor = Cursors.Arrow;
            }

            this.Cursor = Cursors.Arrow;
        }

        const int TAG_METER_WAIT_TIME_SECOND = 5;//5 Second wait time
        const int TAG_METER_WAIT_TIME_MS = 5;
        Stopwatch swTagMeter = new Stopwatch();
        bool bSaveTesting;

        void bgwTagMeter_DoWork(object sender, DoWorkEventArgs e)
        {
            if (prodParam.bTesting)
            {
                bSaveTesting = false;
                if (prodVar.iQuantityInBox < prodParam.iQuantityPerBox)
                {
                    tagMeter.ActivationTest();
                    swTagMeter.Reset();
                    swTagMeter.Start();
                    while (!bgwTagMeter.CancellationPending && !tagMeter.Finished)
                    {
                        System.Threading.Thread.Sleep(TAG_METER_WAIT_TIME_MS);
                        if (swTagMeter.Elapsed.Seconds >= TAG_METER_WAIT_TIME_SECOND)
                        {
                            swTagMeter.Stop();
                            break;
                        }
                    }

                    if (!bgwTagMeter.CancellationPending)
                    {
                        if (tagMeter.activationTestResult.iStatus != 0 || tagMeter.activationTestResult.iStatus != 13)
                        {
                            tagMeter.ActivationTest();
                            swTagMeter.Reset();
                            swTagMeter.Start();
                            while (!bgwTagMeter.CancellationPending && !tagMeter.Finished)
                            {
                                System.Threading.Thread.Sleep(TAG_METER_WAIT_TIME_MS);
                                if (swTagMeter.Elapsed.Seconds >= TAG_METER_WAIT_TIME_SECOND)
                                {
                                    swTagMeter.Stop();
                                    break;
                                }
                            }

                            if (tagMeter.activationTestResult.iStatus != 0 || tagMeter.activationTestResult.iStatus != 13)
                            {
                                tagMeter.ActivationTest();
                                swTagMeter.Reset();
                                swTagMeter.Start();
                                while (!bgwTagMeter.CancellationPending && !tagMeter.Finished)
                                {
                                    System.Threading.Thread.Sleep(TAG_METER_WAIT_TIME_MS);
                                    if (swTagMeter.Elapsed.Seconds >= TAG_METER_WAIT_TIME_SECOND)
                                    {
                                        swTagMeter.Stop();
                                        break;
                                    }
                                }
                            }
                        }

                        ctrlSys.TagMeterResult = tagMeter.activationTestResult.iStatus;
                        if (tagMeter.activationTestResult.iStatus > -1)
                        {
                            if (iUserID == Login.USER_ID_QA)
                            {
                                bSaveTesting = true;
                            }
                            else
                            {
                                bSaveTesting = SaveTestingData();
                            }
                        }
                    }
                }
                else
                {
                    if (prodVar.iQuantityInBox == prodParam.iQuantityPerBox)
                    {
                        ctrlSys.TagMeterResult = 0;
                    }
                    else
                    {
                        while (!bgwTagMeter.CancellationPending)
                        {
                            System.Threading.Thread.Sleep(5);
                        }
                    }
                }
            }
            else
            {
                ctrlSys.TagMeterResult = 13;
            }

            e.Cancel = bgwTagMeter.CancellationPending;
        }

        void bgwTagMeter_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (!e.Cancelled)
            {
                if (prodParam.bTesting)
                {
                    if (tagMeter.activationTestResult.iStatus > -1)
                    {
                        if (tagMeter.activationTestResult.iStatus == 0)
                        {
                            labelTagMeterStatus.Text = "OK";
                            labelTagMeterStatus.BackColor = Color.Green;
                            labelTagMeterResult.Text = "PASS";
                            labelTagMeterResult.BackColor = Color.Green;
                        }
                        else
                        {
                            if (tagMeter.activationTestResult.iStatus == 13)
                            {
                                if (prodParam.bUserTagProgrammer)
                                {
                                    labelTagMeterStatus.Text = "OK";
                                    labelTagMeterStatus.BackColor = Color.Green;
                                    labelTagMeterResult.Text = "PASS";
                                    labelTagMeterResult.BackColor = Color.Green;
                                }
                                else
                                {
                                    labelTagMeterStatus.Text = "NOK";
                                    labelTagMeterStatus.BackColor = Color.Red;
                                    labelTagMeterResult.Text = "FAIL";
                                    labelTagMeterResult.BackColor = Color.Red;
                                }
                            }
                            else
                            {
                                labelTagMeterStatus.Text = "NOK";
                                labelTagMeterStatus.BackColor = Color.Red;
                                labelTagMeterResult.Text = "FAIL";
                                labelTagMeterResult.BackColor = Color.Red;
                            }
                        }

                        labelTagMeter_F0_Unload.Text = tagMeter.activationTestResult.iF0_Unload.ToString();
                        labelTagMeter_F0_Load.Text = tagMeter.activationTestResult.iF0_Load.ToString();
                        labelTagMeter_Q_Unload.Text = tagMeter.activationTestResult.iQUnload.ToString();
                        labelTagMeter_Q_Load.Text = tagMeter.activationTestResult.iQLoad.ToString();
                        labelTagMeterActPower.Text = tagMeter.activationTestResult.iActPower.ToString();
                        labelTagMeterModulationDepth.Text = tagMeter.activationTestResult.iModuationDepth.ToString();
                        labelTagMeterAID_HEX.Text = tagMeter.activationTestResult.sAID_HEX;
                        labelTagMeterAID_DEC.Text = tagMeter.activationTestResult.sAID_DEC;
                        labelTagMeterDelataFreqVsTemp.Text = tagMeter.activationTestResult.iDeltaFrequencyVsTemp.ToString();
                        labelTagMeterTemperature.Text = String.Format("{0}.{1}", tagMeter.activationTestResult.iTemperature / 10, tagMeter.activationTestResult.iTemperature % 10);
                        if (bSaveTesting)
                        {
                            dataGridViewTesting.Rows.Add(
                                dataGridViewTesting.RowCount + 1,
                                labelWorkOrder.Text,
                                labelBoxNumber.Text,
                                tagMeter.activationTestResult.iStatus,
                                tagMeter.activationTestResult.iF0_Unload,
                                tagMeter.activationTestResult.iF0_Load,
                                tagMeter.activationTestResult.iQUnload,
                                tagMeter.activationTestResult.iQLoad,
                                tagMeter.activationTestResult.iActPower,
                                tagMeter.activationTestResult.iModuationDepth,
                                labelTagMeterTemperature.Text,
                                tagMeter.activationTestResult.iDeltaFrequencyVsTemp,
                                tagMeter.activationTestResult.sAID_HEX,
                                tagMeter.activationTestResult.sAID_DEC,
                                DateTime.Now
                                );

                            dataGridViewTesting.CurrentCell = dataGridViewTesting.Rows[dataGridViewTesting.RowCount - 1].Cells[0];
                        }
                    }
                    else
                    {
                        labelTagMeterStatus.Text = "NOK";
                        labelTagMeterStatus.BackColor = Color.Red;
                        labelTagMeterResult.Text = "FAIL";
                        labelTagMeterResult.BackColor = Color.Red;
                        labelTagMeter_F0_Unload.Text = String.Empty;
                        labelTagMeter_F0_Load.Text = String.Empty;
                        labelTagMeter_Q_Unload.Text = String.Empty;
                        labelTagMeter_Q_Load.Text = String.Empty;
                        labelTagMeterActPower.Text = String.Empty;
                        labelTagMeterModulationDepth.Text = String.Empty;
                        labelTagMeterAID_HEX.Text = String.Empty;
                        labelTagMeterAID_DEC.Text = String.Empty;
                        labelTagMeterTemperature.Text = String.Empty;
                        labelTagMeterDelataFreqVsTemp.Text = String.Empty;
                    }
                }
            }
            txtTagMeter.Text = tagMeter.Data;
            txtTagMeter.SelectionStart = txtTagMeter.Text.Length;
            txtTagMeter.ScrollToCaret();
            txtTagMeter.Refresh();
            ctrlSys.TagMeterRunning = false;
            ctrlSys.StartTagMeter = false;
        }

        const string FDX_UID = "8000F9C000000000";
        string sUID_HEX;
        bool bProgrammSuccess;
        void bgwTagProgrammer_DoWork(object sender, DoWorkEventArgs e)
        {
            bProgrammSuccess = false;
            switch(prodParam.iTagType)
            {
                case Registration.TAG_TYPE_FDX:
                    bProgrammSuccess = tagProgrammer.WriteFDX(FDX_UID, chkLock.Checked);
                    if(!bProgrammSuccess)
                    {
                        bProgrammSuccess = tagProgrammer.WriteFDX(FDX_UID, chkLock.Checked);
                    }
                    if(!bProgrammSuccess)
                    {
                        bProgrammSuccess = tagProgrammer.WriteFDX(FDX_UID, chkLock.Checked);
                    }
                    ctrlSys.TagProgrammerResult = tagProgrammer.TagProgrammerResult;            
                    break;
                case Registration.TAG_TYPE_HDX:
                    sUID_HEX = Utility.ConvertUID_FormDecimalToHexadecimal(labelUID.Text).ToString("X16");
                    bProgrammSuccess = tagProgrammer.WriteHDX(sUID_HEX, chkLock.Checked);
                    if (chkLock.Checked)
                    {
                        if (bProgrammSuccess)
                        {
                            bProgrammSuccess = tagProgrammer.ReadHDX() == sUID_HEX;
                        }
                    }
                    ctrlSys.TagProgrammerResult = tagProgrammer.TagProgrammerResult;            
                    break;
                case Registration.TAG_TYPE_HDX_PLUSE:
                    sUID_HEX = Utility.ConvertUID_FormDecimalToHexadecimal(labelUID.Text).ToString("X16");
                    bProgrammSuccess = tagProgrammer.WriteHDX_Plus(sUID_HEX, chkLock.Checked, prodParam.iTrimmingFrequency);
                    if (!bProgrammSuccess)
                    {
                        bProgrammSuccess = tagProgrammer.WriteHDX_Plus(sUID_HEX, chkLock.Checked, prodParam.iTrimmingFrequency);
                        if (!bProgrammSuccess)
                        {
                            bProgrammSuccess = tagProgrammer.WriteHDX_Plus(sUID_HEX, chkLock.Checked, prodParam.iTrimmingFrequency);
                        }
                    }

                    //if (chkLock.Checked)
                    //{
                    //    if (bProgrammSuccess)
                    //    {
                    //        bProgrammSuccess = tagProgrammer.ReadHDX_Plus() == sUID_HEX;
                    //    }
                    //}
                    ctrlSys.TagProgrammerResult = tagProgrammer.TagProgrammerResult;            
                    break;
            }
            e.Cancel = bgwTagProgrammer.CancellationPending;
        }

        void bgwTagProgrammer_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
        }

        void bgwTagProgrammer_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (!e.Cancelled)
            {
                if (ctrlSys.TagProgrammerResult == 0)
                {
                    labelTagProgrammerResult.Text = "PASS";
                    labelTagProgrammerResult.BackColor = Color.Green;
                }
                else
                {
                    labelTagProgrammerResult.Text = "FAIL";
                    labelTagProgrammerResult.BackColor = Color.Red;
                }
            }

            txtTagProgrammer.Text = tagProgrammer.Data;
            txtTagProgrammer.SelectionStart = txtTagProgrammer.Text.Length;
            txtTagProgrammer.ScrollToCaret();
            txtTagProgrammer.Refresh();
            ctrlSys.TagProgrammerRunning = false;
            ctrlSys.StartTagProgrammer = false;
        }

        bool bRegisterSuccess;
        void bgwRegister_DoWork(object sender, DoWorkEventArgs e)
        {
            bRegisterSuccess = false;
            if (prodParam.bRegister)
            {
                if (chkRegister.Checked)
                {
                    bRegisterSuccess = RegisterUID(labelUID.Text, Convert.ToBoolean(e.Argument));// bProgrammSuccess);
                }
            }
            ctrlSys.RegisterSuccess = bRegisterSuccess;
        }

        void bgwRegister_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if(bRegisterSuccess)
            {
                if (prodParam.bRegister)
                {
                    if (bProgrammSuccess)
                    {
                        dataGridViewRegistration.Rows.Add(
                            dataGridViewRegistration.RowCount + 1,
                            labelUID.Text,
                            labelWorkOrder.Text,
                            labelBoxNumber.Text,
                            labelMachineNumber.Text,
                            tagProgrammer.WriteParameter.dTrimFrequency.ToString("F03"),
                            tagProgrammer.WriteParameter.iTrimValue.ToString(),
                            tagProgrammer.WriteParameter.dF1.ToString("F03"),
                            tagProgrammer.WriteParameter.dF2.ToString("F03"),
                            tagProgrammer.WriteParameter.dF1_RST.ToString("F03"),
                            tagProgrammer.WriteParameter.dF2_RST.ToString("F03"),
                            tagProgrammer.WriteParameter.dTemperature.ToString("F01"),
                            DateTime.Now.ToString()
                        );
                        dataGridViewRegistration.CurrentCell = dataGridViewRegistration.Rows[dataGridViewRegistration.RowCount - 1].Cells[0];
                    }
                }
                IncreaseUID();
            }
            ctrlSys.RegigsterFinished = true;
        }

        void bgwBoxFinish_DoWork(object sender, DoWorkEventArgs e)
        {
            bgwBoxFinish.ReportProgress(1);
            if (prodParam.bRegister)
            {
                int iTotalGood = registration.CountTagInLocalDatabase(labelWorkOrder.Text);
                int iTagInBox = registration.CountTagInLocalDatabase(labelWorkOrder.Text, labelBoxNumber.Text);
                if (iTotalGood == iTagInBox)
                {
                    if (iTotalGood == prodParam.iQuantityPerBox)
                    {
                        labelGood.Text = prodParam.iQuantityPerBox.ToString();
                        if (registration.AddBoxNumberToBoxTable(labelBoxNumber.Text, labelWorkOrder.Text, prodParam.iQuantityPerBox, DateTime.Now))
                        {
                            if (registration.TransferDatabaseToServer(labelWorkOrder.Text))
                            {
                                if (!registration.UpdateQuantityWorkedInWorkOrderTable(labelWorkOrder.Text, labelQuantityWorked.Text))
                                {
                                    MessageBox.Show("Cannot update quantity work in server database.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                }
                                PrintLabel(prodParam.iLabelLayout);
                            }
                        }
                        else
                        {
                            e.Cancel = true;
                            MessageBox.Show("Cannot add new box number to database.\n" + registration.ErrorMessage + "\nCall ENGINEER", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    else
                    {
                        e.Cancel = true;
                        MessageBox.Show(String.Format("Quantity in box [{0}] is not equal quantity/box [{1}].", iTagInBox, prodParam.iQuantityPerBox), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    e.Cancel = true;
                    MessageBox.Show(String.Format("Quantity in work order [{0}] is not equal quantity in box [{1}].", iTotalGood, iTagInBox), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                int iTotalGood = registration.CountTestingTagInLocalDatabase(labelWorkOrder.Text);
                if (iTotalGood == prodParam.iQuantityPerBox)
                {
                    labelGood.Text = prodParam.iQuantityPerBox.ToString();
                    if (registration.TransferDatabase2ToServer(labelWorkOrder.Text))
                    {
                        if (!registration.UpdateQuantityWorkedInWorkOrderTable(labelWorkOrder.Text, labelQuantityWorked.Text))
                        {
                            MessageBox.Show("Cannot update quantity work in server database.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        PrintLabel(prodParam.iLabelLayout);
                    }
                }
                else
                {
                    e.Cancel = true;
                    MessageBox.Show(String.Format("Quantity in box [{0}] is not equal Quantity/box [{1}].", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error));
                }
            }
        }

        void bgwBoxFinish_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            switch (e.ProgressPercentage)
            {
                case 1:
                    labelQuantityWorked.Text = ctrlSys.QuantityWorked.ToString();
                    labelGood.Text = ctrlSys.PartInGoodBox.ToString();
                    labelReject1.Text = ctrlSys.PartInRejectBox[0].ToString();
                    labelReject2.Text = ctrlSys.PartInRejectBox[1].ToString();
                    labelReject3.Text = ctrlSys.PartInRejectBox[2].ToString();
                    labelReject4.Text = ctrlSys.PartInRejectBox[3].ToString();
                    labelBoxFinish.Visible = ctrlSys.BoxFinished;
                    break;
            }
        }

        void bgwBoxFinish_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (!e.Cancelled)
            {
                ctrlSys.TransferFinish = true;
                MessageBox.Show("Box Finished", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                BoxFinished();
            }
        }

        private void Release()
        {
            timerMain.Enabled = false;
            if (bgwInitial != null)
            {
                bgwInitial.CancelAsync();
            }

            if (bgwTagMeter != null)
            {
                bgwTagMeter.CancelAsync();
            }

            if (bgwTagProgrammer != null)
            {
                bgwTagProgrammer.CancelAsync();
            }

            if (bgwRegister != null)
            {
                bgwRegister.CancelAsync();
            }

            if (bgwBoxFinish != null)
            {
                bgwBoxFinish.CancelAsync();
            }

            if (bgwInitTagMeter != null)
            {
                bgwInitTagMeter.CancelAsync();
            }

            if (ctrlSys != null)
            {
                ctrlSys.Dispose();
            }
            if (tagProgrammer != null)
            {
                tagProgrammer.Dispose();
            }
            if (tagMeter != null)
            {
                tagMeter.Dispose();
            }
        }

        private void ShowIO()
        {
            ctrlSys.ShowIO(iUserID);
        }

        private void ShowTesterVersion()
        {
            tagProgrammer.GetVersion();
            tagMeter.GetVersion();
        }

        private void Start()
        {
            ClearResult();

            labelGood.Text = ctrlSys.PartInGoodBox.ToString();
            labelQuantityWorked.Text = ctrlSys.QuantityWorked.ToString();
            labelReject1.Text = ctrlSys.PartInRejectBox[0].ToString();
            labelReject2.Text = ctrlSys.PartInRejectBox[1].ToString();
            labelReject3.Text = ctrlSys.PartInRejectBox[2].ToString();
            labelReject4.Text = ctrlSys.PartInRejectBox[3].ToString();

            this.Cursor = Cursors.WaitCursor;
            if (ctrlSys.EnTagProgrammer && ctrlSys.EnTagMeter)
            {
                if (tagProgrammer.ChangePowerLevel(labelPowerLevel.Text))
                {
                    if (chkTest.Checked)
                    {
                        txtMachineMessage.Text = String.Format("Tag Programmer Power Level {0}%\r\n", labelPowerLevel.Text);
                        if (tagMeter.IsAlive())
                        {
                            if (!bgwInitTagMeter.IsBusy)
                            {
                                btnStart.Enabled = false;
                                bgwInitTagMeter.RunWorkerAsync();
                            }
                            else
                            {
                                bgwInitTagMeter.CancelAsync();
                                MessageBox.Show("Tag meter is busy.\nPlease start again.", "Tag meter busy", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                        }
                        else
                        {
                            MessageBox.Show("Not response from tag meter.\nTry again.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    else
                    {
                        ctrlSys.Start();
                        btnLogout.Enabled = false;
                        btnStart.Enabled = false;
                        btnStop.Enabled = true;
                        btnPause.Enabled = true;
                    }
                }
                else
                {
                    this.Cursor = Cursors.Arrow;
                    MessageBox.Show("Cannot change tag programmer power level.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else if (ctrlSys.EnTagProgrammer)
            {
                if (tagProgrammer.ChangePowerLevel(labelPowerLevel.Text))
                {
                    ctrlSys.Start();
                    btnLogout.Enabled = false;
                    btnStart.Enabled = false;
                    btnStop.Enabled = true;
                    btnPause.Enabled = true;
                }
                else
                {
                    this.Cursor = Cursors.Arrow;
                    MessageBox.Show("Cannot change tag programmer power level.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else if(ctrlSys.EnTagMeter)
            {
                if (tagMeter.IsAlive())
                {
                    if (!bgwInitTagMeter.IsBusy)
                    {
                        btnStart.Enabled = false;
                        bgwInitTagMeter.RunWorkerAsync();
                    }
                    else
                    {
                        bgwInitTagMeter.CancelAsync();
                        MessageBox.Show("Tag meter is busy.\nPlease start again.", "Tag meter busy", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                else
                {
                    MessageBox.Show("Not response from tag meter.\nTry again.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            this.Cursor = Cursors.Arrow;
        }

        private void Stop()
        {
            ctrlSys.Stop();
            //bgwTagMeter.CancelAsync();
            btnStop.BackColor = Color.Orange;
        }

        private bool UserLogin()
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
                ClearProductionScreen();
            }
            iUserID = 0;
            statusBarUser.Text = "Logout";
            if (IsServerConnectionOK(SERVER_IP))
            {
                Login login = new Login();
                login.ShowDialog();
                iUserID = login.UserID;
                statusBarUser.Text = login.UserName;

                btnSetting.Enabled = btnIO.Enabled = iUserID >= Login.ROLE_MAINTENANCE;
                btnArticle.Enabled = iUserID >= Login.ROLE_ENGINER;
                btnQA_ReWork.Enabled = iUserID >= Login.ROLE_SUPERVISOR && iUserID != Login.ROLE_MAINTENANCE;
                btnClosePartialBox.Enabled = iUserID >= Login.ROLE_CLOSE_PARTIAL_BOX && iUserID != Login.ROLE_MAINTENANCE;

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
                    ClearProductionScreen();
                }
            }
            else
            {
                MessageBox.Show("Cannot connect to database server.\n", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return iUserID > 0;
        }
        #endregion
    }
}
