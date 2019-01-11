#undef DEBUG
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Diagnostics;
using System.Windows.Forms;
using Utilities;

namespace Testers.TagProgrammer
{
    #region <-+- Public Struct -+->
    public struct WriteParam
    {
        public double dTrimFrequency;
        public int iTrimValue;
        public double dF1;
        public double dF2;
        public double dF1_RST;
        public double dF2_RST;
        public double dTemperature;
    }

    #endregion

    #region <-+- Tag Programmer Class -+->
    public class TagProgrammer : Tester
    {
        #region <-+- Private Constance -+->
        const string CONFIG_FILE_PATH = "config\\config.xml";

        private const string CMD_GET_TESTER_VERSION = ".V";
        private const string CMD_READ_RFID = ".RFID";
        private const string CMD_READ_HDX_PLUS = ".R";
        private const string CMD_WRITE_HDX_PLUS = ".W+";
        private const string CMD_WRITE_HDX_PLUS_LOCK = ".WL+";
        private const string CMD_RESET_HDX_PLUS = ".RST+";
        private const string CMD_SET_POWER_LEVEL = ".TXPW";

        private const string CMD_READ_HDX = ".R";
        private const string CMD_WRITE_HDX = ".W";
        private const string CMD_WRITE_HDX_LOCK = ".WL";

        private const string CMD_READ_FDX = ".RFDXB";
        private const string CMD_WRITE_FDX = ".PEM";
        private const string CMD_WRITE_LOCK_FDX = ".LEM";

        private const int TAG_PROGRAMMER_RETRY = 250;//500
        #endregion

        #region <-+- Private Variable -+->
        Stopwatch swMs;
        bool bTrim;
        string sDataReceived;
        string sSendCommand;
        string sSendMessage;
        string sUID;
        int iTagProgrammerResult;
        bool bCommandResponse;
        bool bCompleteRecieveData;

        WriteParam writeParam = new WriteParam();
        #endregion

        #region <-+- Constructor -+>
        public TagProgrammer()
            : base(PROGRAMMER, 1)
        {
            swMs = new Stopwatch();
        }
        #endregion

        #region <-+- Public Mathode -+->

        public bool GetVersion()
        {
            SendCommand(CMD_GET_TESTER_VERSION, null);
            return bCompleteRecieveData;
        }

        public string ReadHDX_Plus()
        {
            string sUID = null;
            SendCommand(CMD_READ_HDX_PLUS, null);
            if (bCompleteRecieveData)
            {
                try
                {
                    string[] asDataReceived = sDataReceived.Split(new char[] { '\n' });
                    string[] asData = asDataReceived[2].Split(new char[] { ':' });
                    if (asData[0] == "#Tag Found")
                    {
                        sUID = asData[1].Substring(1, asData[1].Length - 2);
                    }
                }
                catch (Exception e)
                {
                    sErrorMessage = e.Message;
                    sUID = null;
                }
            }
            return sUID;
        }

        public bool WriteHDX_Plus(string sUID, bool bLock, int iTrimingFrequency)
        {
            this.sUID = sUID;
            bool bSuccess = false;
            bTrim = iTrimingFrequency > 0;

            if (bLock)
            {
                SendCommand(CMD_WRITE_HDX_PLUS_LOCK, Convert.ToString(iTrimingFrequency) + " " + sUID);
            }
            else
            {
                SendCommand(CMD_WRITE_HDX_PLUS, Convert.ToString(iTrimingFrequency) + " " + sUID);
            }

            if (bCompleteRecieveData)
            {
                try
                {
                    string[] asDataReceived = sDataReceived.Split(new char[] { '\n' });
                    string[] asResMessage = new string[2];
                    string[] asData;
                    bool bResponse = false;
                    for (int iIndex = 0, iMsgIndex = 0; iIndex < asDataReceived.Length; iIndex ++)
                    {
                        if(asDataReceived[iIndex].IndexOf("$") != -1)
                        {
                            asResMessage[iMsgIndex++] = asDataReceived[iIndex];
                        }
                        if(iMsgIndex == 2)
                        {
                            bResponse = true;
                            break;
                        }
                    }

                    asDataReceived = null;

                    if(bResponse)
                    {
                        asData = asResMessage[1].Split(new char[] { ' ' });
                        bSuccess = asData[0] == "$0" && (asData[1] == sUID + "\r");
                        asData = asResMessage[0].Split(new char[] { ' ' });
                        if (bSuccess)
                        {
                            iTagProgrammerResult = 0;
                        }
                        else
                        {
                            string sResult = asData[0].TrimStart(new char[] { '$' });
                            iTagProgrammerResult = Convert.ToInt32(sResult);
                        }
                        writeParam.dTrimFrequency = Convert.ToDouble(asData[1]) / 1000.0;
                        writeParam.iTrimValue = Convert.ToInt32(asData[2]);
                        writeParam.dF1 = Convert.ToDouble(asData[3]) / 1000.0;
                        writeParam.dF2 = Convert.ToDouble(asData[4]) / 1000.0;
                        writeParam.dF1_RST = Convert.ToDouble(asData[5]) / 1000.0;
                        writeParam.dF2_RST = Convert.ToDouble(asData[6]) / 1000.0;
                        writeParam.dTemperature = Convert.ToDouble(asData[7]) / 10.0;
                    }
                }
                catch (Exception e)
                {
                    sErrorMessage = e.Message;
                }
            }

            return bSuccess;
        }

        public bool ResetHDX_Plus()
        {
            bool bSuccess = false;

            SendCommand(CMD_RESET_HDX_PLUS, null);

            if (bCompleteRecieveData)
            {
                try
                {
                    string[] asDataReceived = sDataReceived.Split(new char[] { '\n' });
                    for (int iIndex = 0; iIndex < asDataReceived.Length; iIndex++)
                    {
                        if (asDataReceived[iIndex].IndexOf('$') != -1)
                        {
                            string[] asData = asDataReceived[iIndex].Split(new char[] { ' ' });
                            if (asData[0] == "$1")
                            {
                                bSuccess = true;
                            }
                            break;
                        }
                    }
                }
                catch (Exception e)
                {
                    sErrorMessage = e.Message;
                    sUID = null;
                }
            }

            return bSuccess;
        }

        public bool WriteHDX(string sUID, bool bLock)
        {
            this.sUID = sUID;
            bool bSuccess = false;

            if (bLock)
            {
                SendCommand(CMD_WRITE_HDX_LOCK, sUID);
            }
            else
            {
                SendCommand(CMD_WRITE_HDX, sUID);
            }

            if (bCompleteRecieveData)
            {
                try
                {
                    string[] asDataReceived = sDataReceived.Split(new char[] { '\n' });
                    string[] asData;
                    string sResMessage = null;
                    bool bResponse = false;
                    for (int iIndex = 0; iIndex < asDataReceived.Length; iIndex++)
                    {
                        if (asDataReceived[iIndex].IndexOf("$") != -1)
                        {
                            sResMessage = asDataReceived[iIndex];
                            bResponse = true;
                            break;
                        }
                    }

                    asDataReceived = null;

                    if (bResponse)
                    {
                        asData = sResMessage.Split(new char[] { ' ' });
                        bSuccess = asData[0] == "$0" && (asData[4] == sUID + "\r");
                        if (bSuccess)
                        {
                            iTagProgrammerResult = 0;
                        }
                        else
                        {
                            string sResult = asData[0].TrimStart(new char[] { '$' });
                            iTagProgrammerResult = Convert.ToInt32(sResult);
                        }

                        writeParam.dTrimFrequency = 0;
                        writeParam.iTrimValue = 0;
                        writeParam.dF1 = Convert.ToDouble(asData[2]) / 1000.0;
                        writeParam.dF2 = Convert.ToDouble(asData[3]) / 1000.0;
                        writeParam.dF1_RST = 0;
                        writeParam.dF2_RST = 0;
                        writeParam.dTemperature = Convert.ToDouble(asData[1]) / 10.0;

                    }
                }
                catch (Exception e)
                {
                    sErrorMessage = e.Message;
                }
            }

            return bSuccess;
        }

        public string ReadHDX()
        {
            string sUID = null;
            SendCommand(CMD_READ_HDX, null);
            if (bCompleteRecieveData)
            {
                try
                {
                    string[] asDataReceived = sDataReceived.Split(new char[] { '\n' });
                    for (int iIndex = 0; iIndex < asDataReceived.Length; iIndex++)
                    {
                        if (asDataReceived[iIndex].IndexOf("#Tag Found") != -1)
                        {
                            string[] asData = asDataReceived[iIndex].Split(new char[] { ':' });
                            if (asData[0] == "#Tag Found")
                            {
                                sUID = asData[1].Substring(1, asData[1].Length - 2);
                            }
                            break;
                        }
                    }
                }
                catch (Exception e)
                {
                    sErrorMessage = e.Message;
                    sUID = null;
                }
            }
            return sUID;
        }

        public bool ChangePowerLevel(string sPowerLevel)
        {
            bool bSuccess = false;
            SendCommand(CMD_SET_POWER_LEVEL, sPowerLevel);

            if (bCompleteRecieveData)
            {
                try
                {
                    string[] asDataReceived = sDataReceived.Split(new char[] { '\n' });
                    for (int iIndex = 0; iIndex < asDataReceived.Length; iIndex++)
                    {
                        if (asDataReceived[iIndex].IndexOf("#Ok") != -1)
                        {
                            string[] asData = asDataReceived[iIndex].Split(new char[] { ' ' });
                            bSuccess = asData[1] == sPowerLevel + "\r";
                            break;
                        }
                    }
                }
                catch (Exception e)
                {
                    sErrorMessage = e.Message;
                }
            }
            return bSuccess;
        }

        public string ReadFDX()
        {
            // Read FDX stanard ISO tag.
            string sUID = null; // In Hex format
            SendCommand(CMD_READ_FDX, null);

            if (bCompleteRecieveData)
            {
                try
                {
                    string[] asDataReceived = sDataReceived.Split(new char[] { '\n' });
                    for(int iIndex = 0; iIndex < asDataReceived.Length; iIndex ++)
                    {
                        if(asDataReceived[iIndex].IndexOf("#Tag Found") != -1)
                        {
                            string[] asData = asDataReceived[iIndex].Split(new char[] { ':' });
                            if (asData[0] == "#Tag Found")
                            {
                                sUID = asData[1].Substring(1, asData[1].Length - 2);
                            }
                            break;
                        }
                    }
                }
                catch (Exception e)
                {
                    sErrorMessage = e.Message;
                    sUID = null;
                }
            }
            return sUID;
        }

        public bool WriteFDX(string sUID, bool bLock)
        {
            this.sUID = sUID;
            if (bLock)
            {
                SendCommand(CMD_WRITE_LOCK_FDX, sUID);
            }
            else
            {
                SendCommand(CMD_WRITE_FDX, sUID);
            }

            string sRetUID = null;
            bool bSuccess = false;
            if (bCompleteRecieveData)
            {
                try
                {
                    string[] asDataReceived = sDataReceived.Split(new char[] { '\n' });
                    string sResMessage = null;
                    bool bResponse = false;
                    for (int iIndex = 0; iIndex < asDataReceived.Length; iIndex++)
                    {
                        if (asDataReceived[iIndex].IndexOf("$") != -1)
                        {
                            sResMessage = asDataReceived[iIndex];
                            bResponse = true;
                            break;
                        }
                    }

                    asDataReceived = null;

                    if (bResponse)
                    {
                        string[] asData = sResMessage.Split(new char[] { ' ' });
                        if (asData[0] == "$0")
                        {
                            sRetUID = asData[1].Substring(0, asData[1].Length - 1);
                            bSuccess = sRetUID == sUID;
                            iTagProgrammerResult = 0;
                        }
                        else
                        {
                            string sResult = asData[0].TrimStart(new char[] { '$' });
                            iTagProgrammerResult = Convert.ToInt32(sResult);
                        }
                    }
                }
                catch (Exception e)
                {
                    iTagProgrammerResult = -1;
                    sErrorMessage = e.Message;
                    bSuccess = false;
                }
            }
            return bSuccess;
        }

        #endregion

        #region <-+- Private Methode -+->
        const int WAIT_TIME = 1000;
        private void SendCommand(string sCommand, string sCommandData)
        {
            iTagProgrammerResult = -1;
            bCommandResponse = false;
            bCompleteRecieveData = false;
            sErrorMessage = null;
            sDataReceived = null;
            sSendCommand = sCommand;
            sSendMessage = sCommand;

            if (sCommandData != null)
            {
                sSendMessage += " " + sCommandData + "\r";
            }
            else
            {
                sSendMessage += "\r";
            }

            if (Send(sSendMessage))
            {
                swMs.Reset();
                swMs.Start();
                while (!bCompleteRecieveData && !bTimeout)
                {
                    Thread.Sleep(5);
                    if (!bTimeout)
                    {
                        if (swMs.ElapsedMilliseconds >= WAIT_TIME)
                        {
                            swMs.Stop();
                            sErrorMessage = "Not response form the Tag Programmer.";
                            break;
                        }
                    }
                }
            }
        }

        protected override void DataReceived()
        {
            string sSubMessage;
            try
            {
                sDataReceived += ReadAll();
                if (!bCommandResponse)
                {
                    if (sDataReceived.IndexOf(sSendMessage) != -1)
                    {
                        bCommandResponse = true;
                    }
                }

                if (bCommandResponse)
                {
                    switch (sSendCommand)
                    {
                        case CMD_GET_TESTER_VERSION:
                            if (sDataReceived.Length > sSendCommand.Length)
                            {
                                sSubMessage = sDataReceived.Substring(sSendMessage.Length, sDataReceived.Length - sSendMessage.Length);
                                if (sSubMessage.IndexOf('#') != -1)
                                {
                                    if (sSubMessage[sSubMessage.Length - 1] == '\n')
                                    {
                                        swMs.Stop();
                                        bCompleteRecieveData = true;
                                    }
                                }
                            }
                            break;

                        case CMD_READ_RFID:
                            if (sDataReceived.Length > sSendCommand.Length)
                            {
                                sSubMessage = sDataReceived.Substring(sSendMessage.Length, sDataReceived.Length - sSendMessage.Length);
                                if (sSubMessage.IndexOf(':') != -1)
                                {
                                    if (sSubMessage[sSubMessage.Length - 1] == '\n')
                                    {
                                        swMs.Stop();
                                        bCompleteRecieveData = true;
                                    }
                                }
                            }
                            break;

                        case CMD_SET_POWER_LEVEL:
                            if (sDataReceived.Length > sSendCommand.Length)
                            {
                                sSubMessage = sDataReceived.Substring(sSendMessage.Length, sDataReceived.Length - sSendMessage.Length);
                                if (sSubMessage.IndexOf("#Ok:") != -1)
                                {
                                    if (sSubMessage[sSubMessage.Length - 1] == '\n')
                                    {
                                        swMs.Stop();
                                        bCompleteRecieveData = true;
                                    }
                                }
                            }

                            break;

                        case CMD_RESET_HDX_PLUS:
                            if (sDataReceived.Length > sSendCommand.Length)
                            {
                                sSubMessage = sDataReceived.Substring(sSendMessage.Length, sDataReceived.Length - sSendMessage.Length);
                                if (sSubMessage.IndexOf('$') != -1)
                                {
                                    if (sSubMessage[sSubMessage.Length - 1] == '\n')
                                    {
                                        swMs.Stop();
                                        bCompleteRecieveData = true;
                                    }
                                }
                            }
                            break;

                        case CMD_READ_HDX_PLUS:
                            if (sDataReceived.Length > sSendCommand.Length)
                            {
                                sSubMessage = sDataReceived.Substring(sSendMessage.Length, sDataReceived.Length - sSendMessage.Length);
                                if (sSubMessage.IndexOf('$') != -1)
                                {
                                    if (sSubMessage[sSubMessage.Length - 1] == '\n')
                                    {
                                        swMs.Stop();
                                        bCompleteRecieveData = true;
                                    }
                                }
                            }
                            break;

                        case CMD_WRITE_HDX:
                        case CMD_WRITE_HDX_LOCK:
                            if (sDataReceived.Length > sSendCommand.Length)
                            {
                                sSubMessage = sDataReceived.Substring(sSendMessage.Length, sDataReceived.Length - sSendMessage.Length);
                                if (sSubMessage.IndexOf(sUID) != -1)
                                {
                                    if (sSubMessage[sSubMessage.Length - 1] == '\n')
                                    {
                                        swMs.Stop();
                                        bCompleteRecieveData = true;
                                    }
                                }
                            }
                            break;

                        case CMD_WRITE_HDX_PLUS:
                        case CMD_WRITE_HDX_PLUS_LOCK:
                            if (sDataReceived.Length > sSendCommand.Length)
                            {
                                sSubMessage = sDataReceived.Substring(sSendMessage.Length, sDataReceived.Length - sSendMessage.Length);
                                if (sSubMessage.IndexOf(sUID) != -1)
                                {
                                    if (sSubMessage[sSubMessage.Length - 1] == '\n')
                                    {
                                        swMs.Stop();
                                        bCompleteRecieveData = true;
                                    }
                                }
                            }
                            break;

                        case CMD_READ_FDX:
                            if (sDataReceived.Length > sSendCommand.Length)
                            {
                                sSubMessage = sDataReceived.Substring(sSendMessage.Length, sDataReceived.Length - sSendMessage.Length);
                                if (sSubMessage.IndexOf('$') != -1)
                                {
                                    if (sSubMessage[sSubMessage.Length - 1] == '\n')
                                    {
                                        swMs.Stop();
                                        bCompleteRecieveData = true;
                                    }
                                }
                            }
                            break;

                        case CMD_WRITE_FDX:
                        case CMD_WRITE_LOCK_FDX:
                            if (sDataReceived.Length > sSendCommand.Length)
                            {
                                sSubMessage = sDataReceived.Substring(sSendMessage.Length, sDataReceived.Length - sSendMessage.Length);
                                if (sSubMessage.IndexOf('$') != -1)
                                {
                                    if (sSubMessage.IndexOf(sUID) != -1)
                                    {
                                        if (sSubMessage[sSubMessage.Length - 1] == '\n')
                                        {
                                            swMs.Stop();
                                            bCompleteRecieveData = true;
                                        }
                                    }
                                }
                            }
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                sErrorMessage = ex.Message;
                bError = true;
            }
        }
        #endregion

        #region <-+- Public Acessor -+->
        public bool Initial
        {
            get { return bInitial; }
        }

        public int TagProgrammerResult
        {
            get { return iTagProgrammerResult; }
        }

        public string ErrorMessage
        {
            get { return sErrorMessage; }
        }

        public WriteParam WriteParameter
        {
            get { return writeParam; }
        }

        public string Data
        {
            get { return sDataReceived; }
        }
        #endregion
    }
    #endregion
}
