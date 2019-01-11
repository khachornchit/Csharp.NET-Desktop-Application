using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net.NetworkInformation;
using Registrations;
using System.IO;
using System.Xml;
using Utilities;

namespace Machine
{
    public partial class FormMain : Form
    {
        const string CONFIG_FILE_PATH = "config\\config.xml";
        const string CONFIG_DIR = "config\\";
        const string DATA_FILE_PATH = "data\\data.xml";

        private bool LoadConfig()
        {
            bool bSuccess = false;
            if (File.Exists(CONFIG_FILE_PATH))
            {
                try
                {
                    XmlDocument xmlDocument = new XmlDocument();
                    XmlNode xmlNode;
                    xmlDocument.Load(CONFIG_FILE_PATH);
                    xmlNode = xmlDocument.SelectSingleNode("/Configuration/Machine/MachineNumber") as XmlElement;
                    iMachineNumber = Convert.ToInt32(xmlNode.InnerText);
                    //labelMachineNumber.Text = iMachineNumber.ToString();
                    iEndingBoxNumber = iMachineNumber * BOX_NUMBER_MULTIPLIER + BOX_NUMBER_ENDING_CONST;
                    this.Text = String.Format("REGISTRATION {0:D2}", iMachineNumber);

                    bSuccess = true;
                }
                catch (Exception e)
                {
                    bSuccess = false;
                    MessageBox.Show("Load configuration fail!\n" + e.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                bSuccess = false;
                MessageBox.Show("Load configuration fail!\nConfigutratoin file missing.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return bSuccess;
        }

        bool VerifyDateTime()
        {
            bool bOK;
            try
            {
                string sText = Utility.XmlReadParam(DATA_FILE_PATH, "/Data/DateTime");
                DateTime lastDateTime = Convert.ToDateTime(sText);
                DateTime currentDateTime = DateTime.Now;
                int iResult = currentDateTime.CompareTo(lastDateTime);
                if (iResult > 0)
                {
                    bOK = Utility.XmlSaveParam(DATA_FILE_PATH, "/Data/DateTime", currentDateTime.ToString());
                }
                else
                {
                    bOK = false;
                    sText = String.Format("Date time is not correct.\nCurrent date time is not later than last date time.\n\nLast date time is {0}\nCurrent date time is {1}\n\nCall ENGINEER.", lastDateTime, currentDateTime);
                    if (statusBarUser != null && labelWorkOrder != null)
                    {
                        Log.SaveMessage(statusBarUser.Text, labelWorkOrder.Text, sText);
                    }
                    else
                    {
                        Log.SaveMessage(String.Empty, String.Empty, sText);
                    }
                    MessageBox.Show(sText, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                bOK = false;
                if (statusBarUser != null && labelWorkOrder != null)
                {
                    Log.SaveMessage(statusBarUser.Text, labelWorkOrder.Text, ex.Message + ",Cannot verify date time.\nCall ENGINEER.");
                }
                else
                {
                    Log.SaveMessage(String.Empty, String.Empty, ex.Message + ",Cannot verify date time.\nCall ENGINEER.");
                }
                MessageBox.Show(ex.Message + "\nCannot verify date time.\nCall ENGINEER.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return bOK;
        }

        void ClearResult()
        {
            labelTagMeterResult.BackColor = Color.White;
            labelTagMeterStatus.BackColor = Color.White;
            labelTagProgrammerResult.BackColor = Color.White;
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
            txtMachineMessage.Clear();
            txtTagProgrammer.Clear();
            txtTagMeter.Clear();
        }

        private bool RegisterUID(string sUID, bool bGood)
        {
            System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");
            bool bSuccess = false;

            if (bGood)
            {
                bSuccess = registration.RegisterTagToLocalDatabase(
                    sUID,
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
                    DateTime.Now.ToString(),
                    Registration.NON_PARTIAL_BOX
                );

                if (!bSuccess)
                {
                    Stop();
                    MessageBox.Show("Local database error\n" + registration.ErrorMessage + "\nCall ENGINEER.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                bSuccess = registration.RegisterTagToLocalDatabase(
                    sUID,
                    labelWorkOrder.Text,
                    Registration.FAIL_BOX_PREFIX + labelBoxNumber.Text,
                    labelMachineNumber.Text,
                    String.Empty,
                    String.Empty,
                    String.Empty,
                    String.Empty,
                    String.Empty,
                    String.Empty,
                    String.Empty,
                    DateTime.Now.ToString(),
                    Registration.NON_PARTIAL_BOX
                );

                if (!bSuccess)
                {
                    Stop();
                    MessageBox.Show("Local database error\n" + registration.ErrorMessage + "\nCall ENGINEER.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            return bSuccess;
        }

        bool SaveTestingData()
        {
            int iResult = 0;
            if (tagMeter.activationTestResult.iStatus == 0 || tagMeter.activationTestResult.iStatus == 13)
            {
                iResult = 1;
            }

            bool bSuccess = registration.SaveTestingDataToLocalDataBase(
                labelWorkOrder.Text,
                labelBoxNumber.Text,
                tagMeter.activationTestResult.iStatus,
                tagMeter.activationTestResult.iF0_Unload,
                tagMeter.activationTestResult.iF0_Load,
                tagMeter.activationTestResult.iQUnload,
                tagMeter.activationTestResult.iQLoad,
                tagMeter.activationTestResult.iActPower,
                tagMeter.activationTestResult.iModuationDepth,
                tagMeter.activationTestResult.iTemperature,
                tagMeter.activationTestResult.iDeltaFrequencyVsTemp,
                tagMeter.activationTestResult.sAID_HEX,
                tagMeter.activationTestResult.sAID_DEC,
                iResult,
                DateTime.Now
                );

            if(bSuccess)
            {
                if (iResult == 1)
                {
                    prodVar.iQuantityInBox++;
                }
            }
            else
            {
                ctrlSys.Stop();
                MessageBox.Show("Cannot save testing data to database.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return bSuccess;
        }

        private void PrintLabel(int iLayout)
        {
            switch(iLayout)
            {
                case 0:
                    PrintLabel1();
                    break;
                case 1:
                    PrintLabel2();
                    break;
            }
        }

        private void PrintLabel1()
        {
            string sPrinterName = Utility.XmlReadParam(CONFIG_FILE_PATH, "/Configuration/Printer/Name");
            if (sPrinterName != null)
            {
                string sDateTime = String.Format("{0:D2}/{1:D2}/{2:D2}", DateTime.Now.Day, DateTime.Now.Month, DateTime.Now.Year % 100);
                string sBatch = Printing.GenerateBatch(labelMachineNumber.Text, System.DateTime.Now);
                Printing.PrintLabel(sPrinterName, labelProductLabel.Text, labelArticleNumber.Text, sBatch, labelBoxNumber.Text, labelWorkOrder.Text, sDateTime, labelGood.Text);
            }
        }

        private void PrintLabel2()
        {
            string sPrinterName = Utility.XmlReadParam(CONFIG_FILE_PATH, "/Configuration/Printer/Name");
            if (sPrinterName != null)
            {
                SapInformation sapInfo = registration.GetDataFromSAP(labelWorkOrder.Text);
                if (sapInfo != null)
                {
                    Printing.PrintLabel2(sPrinterName, labelArticleNumber.Text, labelBoxNumber.Text, sapInfo.sProductName, labelGood.Text, labelWorkOrder.Text);
                    sapInfo = null;
                }
            }
        }

        private void BoxFinished()
        {
            if (ctrlSys.BoxFinished)
            {
                Stop();
                btnStop.BackColor = this.BackColor;
                btnStart.Enabled = true;
                btnStop.Enabled = false;
                btnPause.Enabled = false;
                ctrlSys.Reset();
                dataGridViewRegistration.Rows.Clear();
                dataGridViewTesting.Rows.Clear();
                prodVar.iQuantityInBox = 0;
                prodVar.iBoxNumber++;
                if (prodVar.iBoxNumber > iEndingBoxNumber)
                {
                    MessageBox.Show("Box number is over " + iEndingBoxNumber, "Stop", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                }
                switch(prodParam.iLabelLayout)
                {
                    case 0:
                        labelBoxNumber.Text = String.Format("{0}", prodVar.iBoxNumber);
                        break;
                    case 1:
                        labelBoxNumber.Text = String.Format("{0:D5}", prodVar.iBoxNumber);
                        break;
                }
            }
        }

        private void IncreaseUID()
        {
            string sUID = labelUID.Text;
            string sEndingUID = labelEndingUID.Text;
            if (sUID == sEndingUID)
            {
                Stop();
                MessageBox.Show("Tag UID is equal Ending UID " + sEndingUID + "\nTag UID is full\nCall ENGINEER.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);                
            }
            else
            {
                sUID = Utility.IncUID(sUID);
                if (Utility.CompareUID(sUID, sEndingUID) > 0) // sUID > sEndingUID
                {
                    Stop();
                    MessageBox.Show("Tag UID is over Ending UID " + sEndingUID + "\nTag UID is oveflow\nCall ENGINEER.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                }
            }
            labelUID.Text = sUID;
        }

        private void ClearProductionScreen()
        {
            labelArticleNumber.Text = String.Empty;
            labelWorkOrder.Text = String.Empty;
            labelBoxNumber.Text = String.Empty;
            labelQuantityPerBox.Text = String.Empty;
            labelProductLabel.Text = String.Empty;
            labelQuantityWorked.Text = String.Empty;
            labelUID.Text = String.Empty;
            labelBeginningUID.Text = String.Empty;
            labelEndingUID.Text = String.Empty;
            labelWorkOrderTarget.Text = String.Empty;
            labelCustomerName.Text = String.Empty;
            labelMachineNumber.Text = String.Empty;
            labelTagType.Text = String.Empty;
            labelGood.Text = String.Empty;
            labelPowerLevel.Text = String.Empty;
            labelTrimmingFrequency.Text = String.Empty;
            labelReject1.Text = labelReject2.Text = labelReject3.Text = labelReject4.Text = String.Empty;
        }

        bool IsServerConnectionOK(string sIP)
        {
            bool bOK = false;
            Ping pingSender = new Ping();
            PingOptions options = new PingOptions();
            options.DontFragment = true;
            PingReply reply = pingSender.Send(sIP, 120);
            if (reply.Status == IPStatus.Success)
            {
                bOK = true;
            }
            return bOK;
        }
    }
}