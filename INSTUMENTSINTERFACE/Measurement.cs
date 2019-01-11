#undef DEBUG
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Windows.Forms;
using Utilities;

namespace Testers.TagMeter
{
    #region <-+- Public Struct -+->
    public struct ActivationTestResult
    {
        public int iStatus;
        public int iF0_Unload;
        public int iF0_Load;
        public int iQLoad;
        public int iQUnload;
        public int iActPower;
        public int iModuationDepth;
        public string sAID_HEX;
        public string sAID_DEC;
        public int iTemperature;
        public int iDeltaFrequencyVsTemp;
    }

    public struct ResonanceTestResult
    {
        public bool bStatus;
        public int iHField;
        public int iF0;
        public int iQFactor;
        public int iTemperature;
        public int iDeltaFrequencyVsTemp;
    }

    public struct FQShiftTestResult
    {
        public int iF0_Unload;
        public int iF0_Load;
        public int iDeltaFrequency;
        public int iQLoad;
        public int iQUnload;
        public int iTemperature;
    }

    #endregion

    #region <-+- TagMeter Class -+->
    public class TagMeter : Tester
    {
        struct TagMeterResultCommnicationResult
        {
            public bool bCommandSuccess;
            public bool bCommandResponsed;
            public bool bProtocolTimeout;
            public bool bSerialPortReadTimeout;
            public bool bSerialPortException;
        }

        #region <-+- Private Constance -+->
        const string CONFIG_FILE_PATH = "config\\config.xml";

        const string CMD_GET_TESTER_VERSION = ".V";
        const string CMD_TAG_METER_ALIVE = ".K";
        const string CMD_SHOWC = ".SHOWC";
        const string CMD_CAL = ".CAL";
        const string CMD_GETC = ".GETC";
        const string CMD_RES = ".RES";
        const string CMD_MIS = ".MIS";
        const string CMD_ACT = ".ACT";
        const string CMD_GCAL = ".GCAL";
        const string CMD_SAVEC = ".SAVEC";
        const string CMD_LOADC = ".LOADC";

        const int CALIBRATION_TIMEOUT = 10000;
        const int COMMAND_TIMEOUT = 2000;

        #endregion

        #region <-+- Private Object -+->
        ActivationTestResult _ActivationTestResult;
        Stopwatch swMs;
        #endregion

        #region <-+- Private Variable -+->
        TagMeterResultCommnicationResult commResult;

        bool bCompleateRecieveData;
        bool bFinished;

        string sDataReceived;
        string sSendCommand;
        #endregion

        #region <-+- Constructor -+->
        public TagMeter()
            : base(METER, 1)
        {
            swMs = new Stopwatch();
        }
        #endregion

        #region <-+- Public Methode -+->
        public bool GetVersion()
        {
            return SendCommand2(CMD_GET_TESTER_VERSION);
        }

        public bool IsAlive()
        {
            if (SendCommand2(CMD_TAG_METER_ALIVE))
            {
                return commResult.bCommandSuccess;
            }
            return false;
        }

        public bool Calibration()
        {
            bool bSuccess = false;
            ClearResult();
            sSendCommand = CMD_CAL;
           
            if (Send(sSendCommand + "\r"))
            {
                swMs.Reset();
                swMs.Start();
                while (!bCompleateRecieveData && !commResult.bSerialPortReadTimeout)
                {
                    System.Threading.Thread.Sleep(5);
                    if (swMs.ElapsedMilliseconds >= CALIBRATION_TIMEOUT)
                    {
                        sErrorMessage = "Not response from the Tag Meter.";
                        commResult.bProtocolTimeout = true;
                    }
                }
                bSuccess = commResult.bCommandSuccess;
            }
            swMs.Stop();
            return bSuccess;
        }

        public string[,] GetFlashParam()
        {
            string[,] asTagMeterParameters = new string[35, 2];
            string[] asTagMeterParameter;
            string[] asTagMeterParam;

            if(SendCommand2(CMD_GETC))
            {
                try
                {
                    asTagMeterParameter = sDataReceived.Split(new char[] { '\n' });
                    for(int iIndex = 1; iIndex < asTagMeterParameter.Length - 1; iIndex ++ )
                    {
                        asTagMeterParameter[iIndex] = asTagMeterParameter[iIndex].TrimEnd(new char[] {'\r'});
                        asTagMeterParam = asTagMeterParameter[iIndex].Split(new char[] {'\t'});
                        asTagMeterParameters[iIndex - 1, 0] = asTagMeterParam[0];
                        asTagMeterParam[1] = asTagMeterParam[1].TrimStart(new char[] { '=', ' ' });
                        asTagMeterParameters[iIndex - 1, 1] = asTagMeterParam[1];
                    }
                }
                catch(Exception e)
                {
                    sErrorMessage = e.Message;
                    asTagMeterParameters = null;
                }
            }
            else
            {
                asTagMeterParameters = null;
            }
            return asTagMeterParameters;
        }

        public bool SaveConfigToFlash()
        {
            bool bSuccess = false;
            if(SendCommand2(CMD_SAVEC))
            {
                bSuccess = commResult.bCommandSuccess;
            }
            return bSuccess;
        }

        public bool LoadConfigFormFlash()
        {
            bool bSuccess = false;
            if (SendCommand2(CMD_LOADC))
            {
                bSuccess = commResult.bCommandSuccess;
            }
            return bSuccess;

        }
        public bool Resonance()
        {
            return SendCommand(CMD_RES);
        }

        public bool CurveEstimation()
        {
            return SendCommand(CMD_MIS);
        }

        public bool ActivationTest()
        {
            return SendCommand(CMD_ACT);
        }

        public bool GCal()
        {
            return SendCommand(CMD_GCAL);
        }

        public bool ShowActualParameterInRAM()
        {
            return SendCommand3(CMD_SHOWC);
        }

        public bool LoadParameter(string[,] asTagMeterParameter)
        {
            bool bSuccess = false;
            string sCommand;
            for (int iRow = 0; iRow < 35; iRow++)
            {
                sCommand = "." + asTagMeterParameter[iRow,0] + " " + asTagMeterParameter[iRow,1];
                if (iRow == 0)
                {
                    bSuccess = SendCommand3(sCommand);
                }
                else
                {
                    bSuccess &= SendCommand3(sCommand);
                }
            }
            return bSuccess;
        }

        #endregion

        #region <-+- Private Methode -+->

        void ClearResult()
        {
            bFinished = false;
            bCompleateRecieveData = false;
            commResult.bCommandSuccess = false;
            commResult.bCommandResponsed = false;
            commResult.bSerialPortReadTimeout = false;
            commResult.bSerialPortException = false;
            commResult.bProtocolTimeout = false;
            sErrorMessage = null;
            sDataReceived = String.Empty;
        }

        private bool SendCommand(string sCommand)
        {
            ClearResult();
            sSendCommand = sCommand;

            _ActivationTestResult.iStatus = -1;
            return Send(sCommand + "\r");
        }

        private bool SendCommand2(string sCommand)
        {
            bool bSuccess = false;
            ClearResult();
            sSendCommand = sCommand;

            if (Send(sSendCommand + "\r"))
            {
                swMs.Reset();
                swMs.Start();
                while (!bCompleateRecieveData && !commResult.bSerialPortReadTimeout)
                {
                    System.Threading.Thread.Sleep(5);
                    if(swMs.ElapsedMilliseconds >= COMMAND_TIMEOUT)
                    {
                        sErrorMessage = "Not response from the Tag Meter.";
                        commResult.bProtocolTimeout = true;
                        break;
                    }
                }
                bSuccess = bCompleateRecieveData && commResult.bCommandSuccess;
            }
            swMs.Stop();
            return bSuccess;
        }

        private bool SendCommand3(string sCommand)
        {
            bool bSuccess = false;
            ClearResult();
            sSendCommand = sCommand;

            if (Send(sSendCommand + "\r"))
            {
                swMs.Reset();
                swMs.Start();
                while (!bCompleateRecieveData && !commResult.bSerialPortReadTimeout)
                {
                    System.Threading.Thread.Sleep(5);
                    if (swMs.ElapsedMilliseconds >= COMMAND_TIMEOUT)
                    {
                        sErrorMessage = "Not response from the Tag Meter.";
                        commResult.bProtocolTimeout = true;
                        break;
                    }
                }
                bSuccess = bCompleateRecieveData && commResult.bCommandSuccess;
            }
            swMs.Stop();
            return bSuccess;
        }

        bool _bCompleateRecieveData;
        protected override void DataReceived()
        {
            try
            {
                sDataReceived += ReadAll();

                if (!commResult.bCommandResponsed)
                {
                    if (sDataReceived.IndexOf(sSendCommand) != -1)
                    {
                        commResult.bCommandResponsed = true;
                    }
                }

                switch (sSendCommand)
                {
                    case CMD_GET_TESTER_VERSION:
                    case CMD_MIS:
                        if (!bCompleateRecieveData)
                        {
                            if (commResult.bCommandResponsed)
                            {
                                if (sDataReceived.IndexOf("#") != -1)
                                {
                                    _bCompleateRecieveData = sDataReceived[sDataReceived.Length - 1] == '\n';
                                    if (_bCompleateRecieveData)
                                    {
                                        swMs.Stop();
                                        commResult.bCommandSuccess = true;
                                        bCompleateRecieveData = true;
                                    }
                                }
                            }
                        }
                        break;

                    case CMD_CAL:
                        if (!bCompleateRecieveData)
                        {
                            if (commResult.bCommandResponsed)
                            {
                                if (sDataReceived.IndexOf("$") != -1)
                                {
                                    _bCompleateRecieveData = sDataReceived[sDataReceived.Length - 1] == '\n';
                                    if (_bCompleateRecieveData)
                                    {
                                        swMs.Stop();
                                        if (sSendCommand == sDataReceived.Substring(0, sSendCommand.Length))
                                        {
                                            commResult.bCommandSuccess = sDataReceived.IndexOf("$0") != -1;
                                        }
                                        bCompleateRecieveData = true;
                                    }
                                }
                            }
                        }
                        break;

                    case CMD_ACT:
                        if (!bCompleateRecieveData)
                        {
                            if (commResult.bCommandResponsed)
                            {
                                if (sDataReceived.IndexOf("$") != -1)
                                {
                                    _bCompleateRecieveData = sDataReceived[sDataReceived.Length - 1] == '\n';
                                    if (_bCompleateRecieveData)
                                    {
                                        swMs.Stop();
                                        commResult.bCommandResponsed = sDataReceived.IndexOf("$") != -1;
                                        //commResult.bCommandSuccess = sDataReceived.IndexOf("$0") != -1;
                                        ProcessResult();
                                        bCompleateRecieveData = true;
                                    }
                                }
                            }
                        }
                        break;

                    case CMD_TAG_METER_ALIVE:
                        if (!bCompleateRecieveData)
                        {
                            if (commResult.bCommandResponsed)
                            {
                                if (sDataReceived.IndexOf("Ok") != -1)
                                {
                                    _bCompleateRecieveData = sDataReceived[sDataReceived.Length - 1] == '\n';
                                    if (_bCompleateRecieveData)
                                    {
                                        swMs.Stop();
                                        commResult.bCommandSuccess = true;
                                        bCompleateRecieveData = true;
                                    }
                                }
                            }
                        }
                        break;
                    case CMD_GETC:
                    case CMD_SHOWC:
                        if (!bCompleateRecieveData)
                        {
                            if (commResult.bCommandResponsed)
                            {
                                if (sDataReceived.IndexOf("PCPL") != -1)
                                {
                                    _bCompleateRecieveData = sDataReceived[sDataReceived.Length - 1] == '\n';
                                    if (_bCompleateRecieveData)
                                    {
                                        swMs.Stop();
                                        commResult.bCommandSuccess = true;
                                        bCompleateRecieveData = true;
                                    }
                                }
                            }
                        }
                        break;
                    case CMD_SAVEC:
                    case CMD_LOADC:
                        if (!bCompleateRecieveData)
                        {
                            if (commResult.bCommandResponsed)
                            {
                                if (sDataReceived.IndexOf("completed") != -1)
                                {
                                    _bCompleateRecieveData = sDataReceived[sDataReceived.Length - 1] == '\n';
                                    if (_bCompleateRecieveData)
                                    {
                                        swMs.Stop();
                                        commResult.bCommandSuccess = true;
                                        bCompleateRecieveData = true;
                                    }
                                }
                            }
                        }
                        break;
                    case CMD_GCAL:
                        throw new NotImplementedException();
                    default:
                        {
                            string[] asCmd = sSendCommand.Split(new char[] { ' ' });
                            switch (asCmd[0])
                            {
                                case ".TF":
                                case ".TT":
                                case ".FXT":
                                case ".AFT":
                                case ".PRL":
                                case ".PST":
                                case ".PSP":
                                case ".PFU":
                                case ".PFL":
                                case ".PACT":
                                case ".PCRC":
                                case ".PD":
                                case ".PR":
                                case ".AVG":
                                case ".FMAX":
                                case ".FLT":
                                case ".TM0":
                                case ".TM1":
                                case ".TMD":
                                case ".TTT":
                                case ".TO":
                                case ".TC":
                                case ".PFUU":
                                case ".PFUL":
                                case ".PFLU":
                                case ".PFLL":
                                case ".PQUU":
                                case ".PQUL":
                                case ".PQLU":
                                case ".PQLL":
                                case ".PAPU":
                                case ".PAPL":
                                case ".PCPU":
                                case ".PCPL":
                                case ".FMIN":
                                    if (!bCompleateRecieveData)
                                    {
                                        if (commResult.bCommandResponsed)
                                        {
                                            if (sDataReceived.IndexOf("#") != -1)
                                            {
                                                _bCompleateRecieveData = sDataReceived[sDataReceived.Length - 1] == '\n';
                                                if (_bCompleateRecieveData)
                                                {
                                                    swMs.Stop();
                                                    commResult.bCommandSuccess = sDataReceived.IndexOf("Ok") != -1;
                                                    bCompleateRecieveData = true;
                                                }
                                            }
                                        }
                                    }
                                    break;
                            }
                        }
                        break;
                }

                #if DEBUG
                Console.WriteLine(System.DateTime.Now + " Tag Meter : serialPort_DataReceived -> " + iLineCount + " : " + sDataReceived);
                #endif
            }
            catch (TimeoutException ex)
            {
                sErrorMessage = ex.Message;
                commResult.bSerialPortException = true;
                commResult.bSerialPortReadTimeout = true;
                if (sSendCommand == CMD_ACT)
                {
                    _ActivationTestResult.iStatus = -10000;
                }
                bFinished = true;
            }
            catch (Exception ex)
            {
                sErrorMessage = ex.Message;
                commResult.bSerialPortException = true;
                if (sSendCommand == CMD_ACT)
                {
                    _ActivationTestResult.iStatus = -10002;
                }
                bFinished = true;
            }
        }

        void ProcessResult()
        {
            if (commResult.bCommandResponsed)
            {
                string[] asResult;
                string sResult;
                int iIndex;

                try
                {
                    switch (sSendCommand)
                    {
                        case CMD_CAL:
                            // Not implement yet
                            break;
                        case CMD_RES:
                            // Not implement yet
                            break;
                        case CMD_ACT:
                            iIndex = sDataReceived.IndexOf("$");
                            sResult = sDataReceived.Substring(iIndex, sDataReceived.Length - iIndex);
                            asResult = sResult.Split(new char[] { ' ', '\r' });
                            _ActivationTestResult.iStatus = Convert.ToInt32(asResult[0].Substring(1, asResult[0].Length - 1));
                            _ActivationTestResult.iF0_Unload = Convert.ToInt32(asResult[1]);
                            _ActivationTestResult.iF0_Load = Convert.ToInt32(asResult[2]);
                            _ActivationTestResult.iQUnload = Convert.ToInt32(asResult[3]);
                            _ActivationTestResult.iQLoad = Convert.ToInt32(asResult[4]);
                            _ActivationTestResult.iActPower = Convert.ToInt32(asResult[5]);
                            _ActivationTestResult.iModuationDepth = Convert.ToInt32(asResult[6]);
                            _ActivationTestResult.sAID_HEX = asResult[7];
                            _ActivationTestResult.sAID_DEC = asResult[8];
                            _ActivationTestResult.iTemperature = Convert.ToInt32(asResult[9]);
                            _ActivationTestResult.iDeltaFrequencyVsTemp = Convert.ToInt32(asResult[10]);
                            commResult.bCommandSuccess = true;
                            break;
                        case CMD_GCAL:
                            break;
                    }
                }
                catch (Exception ex)
                {
                    sErrorMessage = ex.Message;
                    commResult.bCommandSuccess = false;
                    if (sSendCommand == CMD_ACT)
                    {
                        _ActivationTestResult.iStatus = -10000;
                    }

                }

                asResult = null;
            }
            bFinished = true;
        }
        #endregion

        #region <-+- Public Accessor -+->
        public bool Initial
        {
            get { return bInitial; }
        }

        public bool Finished
        {
            get { return bFinished; }
        }

        public string ErrorMessage
        {
            get { return sErrorMessage; }
        }

        public ActivationTestResult activationTestResult
        {
            get { return _ActivationTestResult; }
        }

        public string Data
        {
            get { return sDataReceived; }
        }
        #endregion
    }
    #endregion
}
