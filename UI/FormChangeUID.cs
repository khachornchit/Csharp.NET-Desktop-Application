using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Testers.TagMeter;
using Testers.TagProgrammer;
using Registrations;
using Utilities;

namespace Machine
{
    public partial class FormChangeUID : Form
    {
        const string CONFIG_FILE_PATH = "config\\config.xml";

        const int TAG_TYPE_HDX = Registration.TAG_TYPE_HDX;
        const int TAG_TYPE_HDX_PLUSE = Registration.TAG_TYPE_HDX_PLUSE;
        const string QA_REJECT = Registration.QA_REJECT;
        const string FAIL_BOX_PREFIX = Registration.FAIL_BOX_PREFIX;
        const string FAIL_QUANTITY = Registration.FAIL_QUANTITY;

        const int SERVER_DTABASE = 1;
        const int LOCAL_DTABASE = 2;

        const int FN_GET_TAG_INFORMATION = 1;
        const int FN_PROGRAM_TAG = 2;

        const int STEP_START = 100;
        const int STEP101 = 101;
        const int STEP102 = 102;
        const int STEP103 = 103;
        const int STEP104 = 104;
        const int STEP105 = 105;
        const int STEP201 = 201;
        const int STEP202 = 202;

        string sHexUID1st, sHexUID2nd, sTargetUID, sNewUID;
        TagInformation tagInfo;
        ProductionParameters prodParam;

        int iTagType = -1;
        int iFunction = 0;
        int iStep = 0;
        int iDatabase = 0;
        int iTotalTagInBox;
        bool bDone = false;

        Registration registration;
        TagProgrammer tagProgrammer;

        string sPrinterName = null;
        bool bLock;

        public FormChangeUID(Registration registration, TagProgrammer tagProgrammer)
        {
            InitializeComponent();
            this.registration = registration;
            this.tagProgrammer = tagProgrammer;
        }

        private void FormChangeUID_Load(object sender, EventArgs e)
        {

        }

        private void FormChangeUID_Shown(object sender, EventArgs e)
        {
            sPrinterName = Utility.XmlReadParam(CONFIG_FILE_PATH, "/Configuration/Printer/Name");
            string sLock = Utility.XmlReadParam(CONFIG_FILE_PATH, "/Configuration/TagProgrammer1/Parameter/Lock");
            if (sPrinterName != null && sLock != null)
            {
                bLock = Convert.ToInt32(sLock) == 1;
            }
            else
            {
                MessageBox.Show("Cannot load configuration parameter.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close();
            }
        }

        private void FormChangeUID_FormClosing(object sender, FormClosingEventArgs e)
        {
            timerDoWork.Enabled = false;
            tagInfo = null;
            prodParam  = null;
        }

        private void FormChangeUID_FormClosed(object sender, FormClosedEventArgs e)
        {

        }

        private void btnReadtTagUID_Click(object sender, EventArgs e)
        {
            labelMessage.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            btnReadtTagUID.Enabled = false;
            btnCancelTagUID.Enabled = false;
            btnGetNewTagUID.Enabled = false;
            btnPrint.Enabled = true;
            btnProgramAndRegisterNewTagUID.Enabled = false;
            ClearInformation();
            iFunction = FN_GET_TAG_INFORMATION;
            iStep = STEP_START;
            bDone = false;
            timerDoWork.Enabled = true;
            txtBoxNumber.BackColor = Color.White;
        }

        private void btnCancelTagUID_Click(object sender, EventArgs e)
        {
            if (Convert.ToInt32(txtBoxNumber.Text) > 0)
            {
                if (MessageBox.Show("Do you want to cancel this tag UID from database?", "Are you sure?", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    labelMessage.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
                    labelMessage.ForeColor = Color.Black;
                    string sBoxNumber = txtBoxNumber.Text;
                    string sWorkOrder = txtWorkOrder.Text;
                    Cursor oldCursor = btnCancelTagUID.Cursor;
                    btnCancelTagUID.Cursor = Cursors.WaitCursor;
                    btnCancelTagUID.Enabled = false;
                    btnGetNewTagUID.Enabled = false;
                    switch (iDatabase)
                    {
                        case SERVER_DTABASE:
                            if (registration.ChangeToBadQATagInfoInServerDataBase(sTargetUID, sWorkOrder, sBoxNumber, System.DateTime.Now.ToString()))
                            {
                                iTotalTagInBox = registration.CountTotalTagInBoxInSeverAndLocalDatabase(sWorkOrder, sBoxNumber);
                                if (iTotalTagInBox >= 0)
                                {
                                    if (iTotalTagInBox < Convert.ToInt32(txtWoQuantityPerBox.Text))
                                    {
                                        txtQuantityPerBox.Text = iTotalTagInBox.ToString();
                                        MessageBox.Show("Update tag information is successful.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                        btnGetNewTagUID.Enabled = true;
                                        btnCancelTagUID.Enabled = false;
                                    }
                                    else if (iTotalTagInBox >= Convert.ToInt32(txtWoQuantityPerBox.Text))
                                    {
                                        MessageBox.Show("Box number " + txtBoxNumber.Text + " is full.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                    }
                                }
                            }
                            else
                            {
                                MessageBox.Show("Cannot update tag information in server database.\n" + registration.ErrorMessage, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                btnCancelTagUID.Enabled = true;
                            }
                            break;
                        case LOCAL_DTABASE:
                            if (registration.ChangeToBadQATagInfoInLocalDataBase(sTargetUID, txtWorkOrder.Text, txtBoxNumber.Text, System.DateTime.Now.ToString()))
                            {
                                iTotalTagInBox = registration.CountTotalTagInBoxInSeverAndLocalDatabase(sWorkOrder, sBoxNumber);
                                if (iTotalTagInBox > 0)
                                {
                                    if (iTotalTagInBox < Convert.ToInt32(txtWoQuantityPerBox.Text))
                                    {
                                        txtQuantityPerBox.Text = iTotalTagInBox.ToString();
                                        MessageBox.Show("Update tag information is successful.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                        btnGetNewTagUID.Enabled = true;
                                        btnCancelTagUID.Enabled = false;
                                    }
                                    else if (iTotalTagInBox >= Convert.ToInt32(txtWoQuantityPerBox.Text))
                                    {
                                        MessageBox.Show("Box number " + txtBoxNumber.Text + " is full.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                    }
                                }
                            }
                            else
                            {
                                MessageBox.Show("Cannot update tag information in local database.\n" + registration.ErrorMessage, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                btnCancelTagUID.Enabled = true;
                            }
                            break;
                    }
                    btnCancelTagUID.Cursor = oldCursor;
                }
            }
            else
            {
                MessageBox.Show("This tag is in bad tag.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btnGetNewTagUID_Click(object sender, EventArgs e)
        {
            labelMessage.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            labelMessage.ForeColor = Color.Black;
            labelMessage.Text = "Get new UID ...";
            Cursor oldCursor = btnGetNewTagUID.Cursor;
            btnGetNewTagUID.Cursor = Cursors.WaitCursor;
            btnProgramAndRegisterNewTagUID.Enabled = false;

            string sLastUID_Local = registration.FindLastUIDinLocalDatabase(txtBeginningUID.Text, txtEndingUID.Text);
            string sLastUID_Server = registration.FindLastUIDinServerDatabase(txtBeginningUID.Text, txtEndingUID.Text);
            string sLastUID = null;

            if (sLastUID_Local != null && sLastUID_Server != null)
            {
                if (Utility.CompareUID(sLastUID_Local, sLastUID_Server) > 0)
                {
                    sLastUID = sLastUID_Local;
                }
                else
                {
                    sLastUID = sLastUID_Server;
                }
            }
            else if (sLastUID_Local != null && registration.ErrorMessage == null)
            {
                sLastUID = sLastUID_Local;
            }
            else if (sLastUID_Server != null && registration.ErrorMessage == null)
            {
                sLastUID = sLastUID_Server;
            }

            if (sLastUID != null)
            {
                sNewUID = Utility.IncUID(sLastUID);
                int iRetVal = registration.UID_ExistInServerDatabase(sNewUID);
                if (iRetVal == 0)
                {
                    txtNextUID.Text = sNewUID;
                    btnProgramAndRegisterNewTagUID.Enabled = true;
                    btnGetNewTagUID.Enabled = false;
                    labelMessage.Text = "Get new UID is successful.";
                }
                else if (iRetVal > 0)
                {
                    MessageBox.Show("New UID from local database duplicate in server database.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else if (iRetVal < 0)
                {
                    MessageBox.Show("Cannot get last UID from local database.\n" + registration.ErrorMessage, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                if (registration.ErrorMessage == null)
                {
                    sLastUID = registration.FindLastUIDinServerDatabase(txtBeginningUID.Text, txtEndingUID.Text);
                    if (sLastUID != null)
                    {
                        sNewUID = Utility.IncUID(sLastUID);
                        txtNextUID.Text = sNewUID;
                        btnProgramAndRegisterNewTagUID.Enabled = true;
                        btnGetNewTagUID.Enabled = false;
                        labelMessage.Text = "Get new UID is successful.";
                    }
                    else
                    {
                        if (registration.ErrorMessage == null)
                        {
                            MessageBox.Show("Cannot find the last UID from database.\n", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        else
                        {
                            MessageBox.Show("Cannot find the last UID from server database.\n" + registration.ErrorMessage, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Cannot find the last UID from local database.\n" + registration.ErrorMessage, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            btnGetNewTagUID.Cursor = oldCursor;
        }

        private void btnProgramAndRegisterNewTagUID_Click(object sender, EventArgs e)
        {
            if (sNewUID.Length == 23)
            {
                labelMessage.ForeColor = Color.Black;
                Cursor oldCursor = btnProgramAndRegisterNewTagUID.Cursor;
                btnProgramAndRegisterNewTagUID.Enabled = false;
                btnGetNewTagUID.Enabled = false;
                btnProgramAndRegisterNewTagUID.Cursor = oldCursor;

                int iTagInBox = registration.CountTotalTagInBoxInSeverAndLocalDatabase(txtWorkOrder.Text, txtBoxNumber.Text);

                if (iTagInBox >= 0)
                {
                    if (iTagInBox < Convert.ToInt32(txtWoQuantityPerBox.Text))
                    {
                        btnReadtTagUID.Enabled = false;
                        iFunction = FN_PROGRAM_TAG;
                        iStep = STEP_START;
                        bDone = false;
                        timerDoWork.Enabled = true;
                    }
                }
                else
                {
                    MessageBox.Show("Cannot get quantity in box number " + txtBoxNumber.Text + ".\n" + registration.ErrorMessage, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    btnProgramAndRegisterNewTagUID.Enabled = true;
                }
            }
            else
            {
                MessageBox.Show("Please get new UID.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void timerDoWork_Tick(object sender, EventArgs e)
        {
            timerDoWork.Enabled = false;
            switch (iFunction)
            {
                case FN_GET_TAG_INFORMATION:
                    GetTagInformation();
                    break;
                case FN_PROGRAM_TAG:
                    ProgramTag();
                    break;
            }
            timerDoWork.Enabled = !bDone;
        }

        private void DisplayTagInformation()
        {
            txtUID.Text = tagInfo.sUID;// asTagInfo[0];
            txtWorkOrder.Text = tagInfo.sWorkOrder;
            txtBoxNumber.Text = tagInfo.sBoxNumber;
            txtMachineNumber.Text = tagInfo.sMachineNumber;
            txtTrimFrequency.Text = tagInfo.sTrimFrequency;
            txtTrimValue.Text = tagInfo.sTrimValue;
            txtF1.Text = tagInfo.sF1;
            txtF2.Text = tagInfo.sF2;
            txtF1_RST.Text = tagInfo.sF1_RST;
            txtF2_RST.Text = tagInfo.sF2_RST;
            txtDateTime.Text = tagInfo.sDateTime;
            iTotalTagInBox = registration.CountTotalTagInBoxInSeverAndLocalDatabase(txtWorkOrder.Text, txtBoxNumber.Text);
            txtQuantityPerBox.Text = iTotalTagInBox.ToString();//asTagInfo[4];      
        }

        private void DisplayWorkOrderAndArticleInformation()
        {
            txtWoWorkOrder.Text = prodParam.iWorkOrder.ToString();
            txtArticleNumber.Text = prodParam.sArticleNumber;
            txtCustomerName.Text = prodParam.sCustormerName;
            txtLabelName.Text = prodParam.sProductLabel;
            txtBeginningUID.Text = prodParam.sBeginningUID;
            txtEndingUID.Text = prodParam.sEndingUID;
            txtWoQuantityPerBox.Text = tagInfo.sQuantityPerBox;//asArticleInfo[5];
            txtWoTrimFrequency.Text = prodParam.iTrimmingFrequency.ToString();
            txtTagProgrammerPowerLevel.Text = prodParam.iPowerLevel.ToString();

            iTagType = prodParam.iTagType;
            switch (iTagType)
            {
                case TAG_TYPE_HDX:
                    txtTagType.Text = "HDX";
                    break;
                case TAG_TYPE_HDX_PLUSE:
                    txtTagType.Text = "HDX+";
                    break;
                default:
                    txtTagType.Text = "Unknow";
                    break;
            }
        }

        private void ClearInformation()
        {
            txtUID.Clear();
            txtWorkOrder.Clear();
            txtBoxNumber.Clear();
            txtMachineNumber.Clear();
            txtQuantityPerBox.Clear();
            txtTrimFrequency.Clear();
            txtTrimValue.Clear();
            txtF1.Clear();
            txtF2.Clear();
            txtF1_RST.Clear();
            txtF2_RST.Clear();
            txtDateTime.Clear();

            txtWoWorkOrder.Clear();
            txtArticleNumber.Clear();
            txtCustomerName.Clear();
            txtLabelName.Clear();
            txtBeginningUID.Clear();
            txtEndingUID.Clear();
            txtWoQuantityPerBox.Clear();
            txtTagType.Clear();
            txtWoTrimFrequency.Clear();
            txtTagProgrammerPowerLevel.Clear();

            txtNextUID.Clear();
            sNewUID = null;
            iTagType = -1;

            txtBoxNumber.BackColor = Color.White;
        }

        bool ProgramHDX(string sNewUID)
        {
            bool bSuccess = false;
            string sHexadecimalNewUID = Utility.ConvertUID_FormDecimalToHexadecimal(sNewUID).ToString("X16");
            bool bProgramSuccess = tagProgrammer.WriteHDX(sHexadecimalNewUID, bLock);

            if (bLock)
            {
                if (bProgramSuccess)
                {
                    bSuccess = tagProgrammer.ReadHDX() == sHexadecimalNewUID;
                }
            }
            else
            {
                bSuccess = bProgramSuccess;
            }
            if (bSuccess)
            {
                labelMessage.ForeColor = Color.Black;
                labelMessage.Text = "Program new UID is successful.";
            }
            else
            {
                labelMessage.ForeColor = Color.Red;
                labelMessage.Text = "Program new UID fail.";
            }
            return bSuccess;
        }

        bool ProgramHDX_Plus(string sNewUID, double dTrimingFrequency)
        {
            bool bSuccess = false;
            int iTrimingFrequency = Convert.ToInt32(dTrimingFrequency);
            string sHexadecimalNewUID = Utility.ConvertUID_FormDecimalToHexadecimal(sNewUID).ToString("X16");
            bool bProgramSuccess = tagProgrammer.WriteHDX_Plus(sHexadecimalNewUID, bLock, iTrimingFrequency);

            if (bLock)
            {
                if (bProgramSuccess)
                {
                    bSuccess = tagProgrammer.ReadHDX_Plus() == sHexadecimalNewUID;
                }
            }
            else
            {
                bSuccess = bProgramSuccess;
            }

            if (bSuccess)
            {
                labelMessage.ForeColor = Color.Black;
                labelMessage.Text = "Program new UID is successful.";
            }
            else
            {
                labelMessage.ForeColor = Color.Red;
                labelMessage.Text = "Program new UID fail.";
            }
            return bSuccess;
        }

        private bool RegisterUID(int iDataBase, bool bGood)
        {
            bool bSuccess = false;
            switch (iDatabase)
            {
                case SERVER_DTABASE:
                    if (bGood)
                    {
                        bSuccess = registration.RegisterTagToServerDatabase(
                            sNewUID,
                            txtWorkOrder.Text,
                            txtBoxNumber.Text,
                            txtMachineNumber.Text,
                            tagProgrammer.WriteParameter.dTrimFrequency.ToString("F03"),
                            tagProgrammer.WriteParameter.iTrimValue.ToString(),
                            tagProgrammer.WriteParameter.dF1.ToString("F03"),
                            tagProgrammer.WriteParameter.dF2.ToString("F03"),
                            tagProgrammer.WriteParameter.dF1_RST.ToString("F03"),
                            tagProgrammer.WriteParameter.dF2_RST.ToString("F03"),
                            tagProgrammer.WriteParameter.dTemperature.ToString("F01"),
                            DateTime.Now.ToString(),
                            QA_REJECT
                        );
                    }
                    else
                    {
                        bSuccess = registration.RegisterTagToServerDatabase(
                            sNewUID,
                            txtWorkOrder.Text,
                            FAIL_BOX_PREFIX + txtBoxNumber.Text,
                            txtMachineNumber.Text,
                            String.Empty,
                            String.Empty,
                            String.Empty,
                            String.Empty,
                            String.Empty,
                            String.Empty,
                            String.Empty,
                            DateTime.Now.ToString(),
                            QA_REJECT
                        );
                    }
                    break;

                case LOCAL_DTABASE:
                    if (bGood)
                    {
                        bSuccess = registration.RegisterTagToLocalDatabase(
                            sNewUID,
                            txtWorkOrder.Text,
                            txtBoxNumber.Text,
                            txtMachineNumber.Text,
                            tagProgrammer.WriteParameter.dTrimFrequency.ToString("F03"),
                            tagProgrammer.WriteParameter.iTrimValue.ToString(),
                            tagProgrammer.WriteParameter.dF1.ToString("F03"),
                            tagProgrammer.WriteParameter.dF2.ToString("F03"),
                            tagProgrammer.WriteParameter.dF1_RST.ToString("F03"),
                            tagProgrammer.WriteParameter.dF2_RST.ToString("F03"),
                            tagProgrammer.WriteParameter.dTemperature.ToString("F01"),
                            DateTime.Now.ToString(),
                            QA_REJECT
                        );
                    }
                    else
                    {
                        bSuccess = registration.RegisterTagToLocalDatabase(
                            sNewUID,
                            txtWorkOrder.Text,
                            FAIL_BOX_PREFIX + txtBoxNumber.Text,
                            txtMachineNumber.Text,
                            String.Empty,
                            String.Empty,
                            String.Empty,
                            String.Empty,
                            String.Empty,
                            String.Empty,
                            String.Empty,
                            DateTime.Now.ToString(),
                            QA_REJECT
                        );
                    }
                    break;
            }

            return bSuccess;
        }

        private void GetTagInformation()
        {
            switch (iStep)
            {
                case STEP_START:
                    ClearInformation();
                    iDatabase = 0;
                    bDone = false;
                    sTargetUID = null;
                    iStep = STEP101;
                    break;
                case STEP101:
                    labelMessage.Text = "Reading tag UID...";
                    iStep = STEP102;
                    break;
                case STEP102:
                    sHexUID1st = tagProgrammer.ReadHDX();
                    if (sHexUID1st != null)
                    {
                        sHexUID2nd = tagProgrammer.ReadHDX();
                        if (sHexUID1st == sHexUID2nd)
                        {
                            sTargetUID = Utility.ConvertUID_HexadecimalToDecimal(sHexUID1st);
                            labelMessage.Text = "UID = " + sTargetUID;
                            iStep = STEP103;
                        }
                    }
                    else
                    {
                        labelMessage.Text = "Cannot read tag UID!";
                        iStep = STEP201;
                    }
                    break;

                case STEP103:
                    tagInfo = registration.GetTagInformationFormServerDatabase(sTargetUID);
                    if (tagInfo != null)
                    {
                        DisplayTagInformation();
                        iDatabase = SERVER_DTABASE;
                        iStep = STEP104;
                    }
                    else
                    {
                        if (registration.ErrorMessage == null)
                        {
                            tagInfo = registration.GetTagInformationFormLocalDatabase(sTargetUID);
                            if (tagInfo != null)
                            {
                                DisplayTagInformation();
                                iDatabase = LOCAL_DTABASE;
                                iStep = STEP104;
                            }
                            else
                            {
                                if (registration.ErrorMessage == null)
                                {
                                    MessageBox.Show("No tag information in database.", "No data", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                }
                                else
                                {
                                    MessageBox.Show("Cannot get tag information from server database.\n" + registration.ErrorMessage, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                }
                                iStep = STEP201;
                            }
                        }
                        else
                        {
                            MessageBox.Show("Cannot get tag information from server database.\n" + registration.ErrorMessage, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            iStep = STEP201;
                        }
                    }
                    break;

                case STEP104:
                    prodParam = registration.GetProductionParamFromServerDatabase(txtWorkOrder.Text);
                    if (prodParam != null)
                    {
                        iStep = STEP105;
                    }
                    else
                    {
                        iStep = STEP201;
                    }
                    break;

                case STEP105:
                    DisplayWorkOrderAndArticleInformation();
                    try
                    {
                        if (Convert.ToInt32(txtBoxNumber.Text) > 0)
                        {
                            btnCancelTagUID.Enabled = true;
                        }
                        else
                        {
                            txtBoxNumber.BackColor = Color.Orange;
                        }
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show("Exception error\n" + e.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }

                    iStep = STEP201;
                    break;

                case STEP201:
                    iFunction = 0;
                    btnReadtTagUID.Enabled = true;
                    bDone = true;
                    break;
            }
        }

        private void ProgramTag()
        {
            switch (iStep)
            {
                case STEP_START:
                    bDone = false;
                    if(tagProgrammer.ChangePowerLevel("25"))
                    {
                        iStep = STEP101;
                    }
                    else
                    {
                        iStep = 0;
                        MessageBox.Show("Cannot change tag programmer power level.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    break;

                case STEP101:
                    labelMessage.ForeColor = Color.Blue;
                    labelMessage.Text = "Program new UID. Please wait...";
                    iStep = STEP102;
                    break;

                case STEP102:
                    switch (iTagType)
                    {
                        case TAG_TYPE_HDX:
                            if (ProgramHDX(sNewUID))
                            {
                                labelMessage.ForeColor = Color.Blue;
                                labelMessage.Text = "Register tag UID...";
                                iTotalTagInBox++;
                                txtQuantityPerBox.Text = iTotalTagInBox.ToString();
                                if (RegisterUID(iDatabase, true))
                                {
                                    iStep = STEP103;
                                }
                                else
                                {
                                    labelMessage.ForeColor = Color.Red;
                                    labelMessage.Text = "Register tag UID fail!";
                                    MessageBox.Show("Cannot register tag UID.\n" + registration.ErrorMessage, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    iStep = STEP202;
                                }
                            }
                            else
                            {
                                labelMessage.ForeColor = Color.Red;
                                labelMessage.Text = "Register bad tag UID...";
                                if (RegisterUID(iDatabase, false))
                                {
                                    iStep = STEP104;
                                }
                                else
                                {
                                    labelMessage.Text = "Register tag UID fail!";
                                    MessageBox.Show("Cannot register tag UID.\n" + registration.ErrorMessage, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    iStep = STEP202;
                                }
                            }
                            sNewUID = null;
                            break;
                        case TAG_TYPE_HDX_PLUSE:
                            if (ProgramHDX_Plus(sNewUID, Convert.ToDouble(txtWoTrimFrequency.Text)))
                            {
                                labelMessage.ForeColor = Color.Blue;
                                labelMessage.Text = "Register tag UID...";
                                iTotalTagInBox++;
                                txtQuantityPerBox.Text = iTotalTagInBox.ToString();
                                if (RegisterUID(iDatabase, true))
                                {
                                    iStep = STEP103;
                                }
                                else
                                {
                                    labelMessage.ForeColor = Color.Red;
                                    labelMessage.Text = "Register tag UID fail!";
                                    MessageBox.Show("Cannot register tag UID.\n" + registration.ErrorMessage, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    iStep = STEP202;
                                }
                            }
                            else
                            {
                                labelMessage.ForeColor = Color.Red;
                                labelMessage.Text = "Register bad tag UID...";
                                if (RegisterUID(iDatabase, false))
                                {
                                    iStep = STEP104;
                                }
                                else
                                {
                                    labelMessage.Text = "Register tag UID fail!";
                                    MessageBox.Show("Cannot register tag UID.\n" + registration.ErrorMessage, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    iStep = STEP202;
                                }
                            }
                            sNewUID = null;
                            break;
                    }
                    break;

                case STEP103:
                    iTotalTagInBox = registration.CountTotalTagInBoxInSeverAndLocalDatabase(txtWorkOrder.Text, txtBoxNumber.Text);
                    if (iTotalTagInBox >= 0)
                    {
                        txtQuantityPerBox.Text = iTotalTagInBox.ToString();
                    }
                    labelMessage.ForeColor = Color.White;
                    labelMessage.BackColor = Color.Green;
                    labelMessage.Text = "Program and register tag UID is successful.";
                    iStep = STEP201;
                    break;

                case STEP104:
                    labelMessage.ForeColor = Color.White;
                    labelMessage.BackColor = Color.Red;
                    labelMessage.Text = "Program UID fail!";
                    iStep = STEP202;
                    break;

                case STEP201:
                    iFunction = 0;
                    btnReadtTagUID.Enabled = true;
                    bDone = true;
                    break;

                case STEP202:
                    iFunction = 0;
                    btnGetNewTagUID.Enabled = true;
                    bDone = true;
                    break;
            }
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            if (Convert.ToInt32(txtBoxNumber.Text) > 0)
            {
                Cursor oldCursor = btnPrint.Cursor;
                btnPrint.Cursor = Cursors.WaitCursor;
                btnPrint.Enabled = false;
                int iTagInBox = registration.CountTotalTagInBoxInSeverAndLocalDatabase(txtWorkOrder.Text, txtBoxNumber.Text);
                btnPrint.Cursor = oldCursor;

                if (iTagInBox >= 0)
                {
                    DateTime dateTime = Convert.ToDateTime(txtDateTime.Text);
                    string sDate = String.Format("{0:D2}/{1:D2}/{2:D2}", dateTime.Day, dateTime.Month, dateTime.Year % 100);
                    string sBatch = Printing.GenerateBatch(Convert.ToInt32(txtMachineNumber.Text), dateTime);

                    Printing.PrintLabel(
                        sPrinterName,
                        txtLabelName.Text,
                        txtArticleNumber.Text,
                        sBatch,
                        txtBoxNumber.Text,
                        txtWorkOrder.Text,
                        sDate,
                        iTagInBox.ToString()
                        );
                }
                else
                {
                    MessageBox.Show("Cannot get quantity in box.\n" + registration.ErrorMessage, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                btnPrint.Enabled = true;
            }
            else
            {
                MessageBox.Show("This tag is in bad tag.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
    }
}
