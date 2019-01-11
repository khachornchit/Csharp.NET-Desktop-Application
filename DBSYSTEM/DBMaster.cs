using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Data.Odbc;
using System.Windows.Forms;
using Utilities;

namespace Registrations
{
    public class TagInformation
    {
        public string sUID;
        public string sWorkOrder;
        public string sBoxNumber;
        public string sQuantityPerBox;
        public string sPartialBox;
        public string sMachineNumber;
        public string sTrimFrequency;
        public string sTrimValue;
        public string sF1;
        public string sF2;
        public string sF1_RST;
        public string sF2_RST;
        public string sTemperature;
        public string sDateTime;
    }

    public class BoxInformation
    {
        public string sBoxNumber;
        public string sWorkOrder;
        public string sQunaittyPerBox;
        public string sDateTime;
    }

    public class SapInformation
    {
        public string sProductName;
        public string sArticleNumber;
        public string sWorkOrderTarget;
        public string sWorkOrderStatus;
        public string sDateTime;
    }

    public partial class Registration : Database
    {
        const string CONFIG_FILE_PATH = "config\\config.xml";
        const string CONFIG_DIR = "config\\";

        #region <-+- Public Methode -+->

        public static string ParseWorkOrderStatusToString(string sWorkOrderStatus)
        {
            string _sWorkOrderStatus = null;
            switch (sWorkOrderStatus)
            {
                case "0":
                    _sWorkOrderStatus = "Not Start";
                    break;
                case "1":
                    _sWorkOrderStatus = "Paper Printed";
                    break;
                case "2":
                    _sWorkOrderStatus = "Started";
                    break;
                case "3":
                    _sWorkOrderStatus = "Finished";
                    break;
            }

            return _sWorkOrderStatus;
        }

        public static int ParseStringToWorkOrderStatus(string sWorkOrderStatus)
        {
            int _sWorkOrderStatus = -1;
            switch (sWorkOrderStatus)
            {
                case "Not Start":
                    _sWorkOrderStatus = 0;
                    break;
                case "Paper Printed":
                    _sWorkOrderStatus = 1;
                    break;
                case "Started":
                    _sWorkOrderStatus = 2;
                    break;
                case "Finished":
                    _sWorkOrderStatus = 3;
                    break;
            }

            return _sWorkOrderStatus;
        }

        public static string ParseProgramOptonToString(int iOption)
        {
            string sOption = null;
            switch (iOption)
            {
                case 0:
                    sOption = "Not Lock";
                    break;
                case 1:
                    sOption = "Lock";
                    break;
                case 2:
                    sOption = "Trim only";
                    break;
            }
            return sOption;
        }

        public bool TransferDatabaseToServer(string sWorkOrder)
        {
            FormTransfer formTransfer = new FormTransfer(sServerConnection, sLocalConnection, sWorkOrder);
            sErrorMessage = null;
            formTransfer.ShowDialog();
            sErrorMessage = formTransfer.ErrorMessage;
            return (formTransfer.TransferSuccess);
        }

        public bool TransferDatabase2ToServer(string sWorkOrder)
        {
            FormTransferTesting formTransfer = new FormTransferTesting(sServerConnection, sLocalConnection, sWorkOrder);
            sErrorMessage = null;
            formTransfer.ShowDialog();
            sErrorMessage = formTransfer.ErrorMessage;
            return (formTransfer.TransferSuccess);
        }

        public int WorkOrderListExitsInServerDatabase(string sWorkOrder)
        {
            string sQuery = "SELECT COUNT(WorkOrder) FROM " + SERVER_WORKORDER_TABLE + " WHERE WorkOrder='" + sWorkOrder + "'";
            return CheckDataExistInDatabase(sServerConnection, sQuery);
        }

        public bool DeleteWorkOrderFromServerDatabase(string sWorkOrder)
        {
            string sQuery = "DELETE FROM " + SERVER_WORKORDER_TABLE + " WHERE WorkOrder" + "='" + sWorkOrder + "'";
            bool bResult = ExecuteNonQuerySQL(sServerConnection, sQuery) == 1;
            return bResult;
        }

        public int ArticleExistInServerDatabase(string sArticleNumber)
        {
            string sQuery = "SELECT COUNT(ArticleNumber) FROM " + SERVER_ARTICLE_TABLE + " WHERE ArticleNumber='" + sArticleNumber + "'";
            return CheckDataExistInDatabase(sServerConnection, sQuery);
        }

        public bool DeleteArticleNumberFromServerDatabase(string sArticleNumber)
        {
            string sQuery = "DELETE FROM " + SERVER_ARTICLE_TABLE + " WHERE ArticleNumber='" + sArticleNumber + "'";
            bool bResult = ExecuteNonQuerySQL(sServerConnection, sQuery) == 1;
            return bResult;
        }

        public int IsArticleBeingUsedInWorkOrderList(string sArticleNumber)
        {
            string sQuery = "IF EXISTS (SELECT [ArticleNumber] FROM " + SERVER_WORKORDER_TABLE + " WHERE ArticleNumber='" + sArticleNumber + "')" +
                             " SELECT 1 AS RetVal ELSE SELECT 0 AS RetVal";
            return CheckDataExistInDatabase(sServerConnection, sQuery);
        }

        public int CountTagInServerDatabase(string sWorkOrder)
        {
            string sQuery = "SELECT COUNT (UID) FROM " + SERVER_REGISTRATION_TABLE + " WHERE WorkOrder=" + sWorkOrder + " AND BoxNumber>0";
            return CheckDataExistInDatabase(sServerConnection, sQuery);
        }

        public int CountTagInLocalDatabase(string sWorkOrder)
        {
            string sQuery = "SELECT COUNT (UID) FROM " + LOCAL_REGISTRATION_TABLE + " WHERE WorkOrder=" + sWorkOrder + " AND BoxNumber>0";
            return CheckDataExistInDatabase(sLocalConnection, sQuery);
        }

        public int CountTagInLocalDatabase(string sWorkOrder, string sBoxNumber)
        {
            string sQuery = "SELECT COUNT (UID) FROM " + LOCAL_REGISTRATION_TABLE + " WHERE WorkOrder=" + sWorkOrder + " AND BoxNumber=" + sBoxNumber;
            return CheckDataExistInDatabase(sLocalConnection, sQuery);
        }

        public int CountTotalTagInBoxInSeverAndLocalDatabase(string sWorkOrder, string sBoxNumber)
        {
            string sTableName = LOCAL_REGISTRATION_TABLE;
            string sQuery = "SELECT COUNT " + "(BoxNumber)" + " FROM " + sTableName + " WHERE BoxNumber=" + sBoxNumber + " AND WorkOrder =" + sWorkOrder;
            string sTagInBoxLocal = GetItemFromDatabase(sLocalConnection, sQuery);
            int iTagTotalTagInBox = 0;
            if (sErrorMessage == null)
            {
                sTableName = SERVER_REGISTRATION_TABLE;
                sQuery = "SELECT COUNT " + "(BoxNumber)" + " FROM " + sTableName + " WHERE BoxNumber=" + sBoxNumber + " AND WorkOrder =" + sWorkOrder;
                string sTagInBoxServer = GetItemFromDatabase(sServerConnection, sQuery);
                if (sErrorMessage == null)
                {
                    if (sTagInBoxLocal != null)
                    {
                        iTagTotalTagInBox = Convert.ToInt32(sTagInBoxLocal);
                    }

                    if (sTagInBoxServer != null)
                    {
                        iTagTotalTagInBox += Convert.ToInt32(sTagInBoxServer);
                    }

                    return iTagTotalTagInBox;
                }
            }
            return -1;
        }
        
        public int CountBadUIDinServerDatabase(string sWorkOrder)
        {
            string sQuery = "SELECT COUNT (*) FROM " + SERVER_REGISTRATION_TABLE + " WHERE WorkOrder='" + sWorkOrder + "' AND BoxNumber<0";
            return CheckDataExistInDatabase(sServerConnection, sQuery);
        }

        public int CountBadUIDinLocalDatabase(string sWorkOrder)
        {
            string sQuery = "SELECT COUNT (*) FROM " + LOCAL_REGISTRATION_TABLE + " WHERE WorkOrder='" + sWorkOrder + "' AND BoxNumber<0";
            return CheckDataExistInDatabase(sLocalConnection, sQuery);
        }

        public int UID_ExistInServerDatabase(string sUID)
        {
            string sQuery = "IF EXISTS (SELECT [UID] FROM " + SERVER_REGISTRATION_TABLE + " WHERE UID='" + sUID + "')" +
                             " SELECT 1 AS RetVal ELSE SELECT 0 AS RetVal";
            return CheckDataExistInDatabase(sServerConnection, sQuery);
        }

        public int UID_ExistInLocalDatabase(string sUID)
        {
            string sQuery = "IF EXISTS (SELECT [UID] FROM " + LOCAL_REGISTRATION_TABLE + " WHERE UID='" + sUID + "')" +
                             " SELECT 1 AS RetVal ELSE SELECT 0 AS RetVal";
            return CheckDataExistInDatabase(sLocalConnection, sQuery);
        }

        public string FindLastUIDinServerDatabase(string sBeginnigUID, string sEndingUID)
        {
            return GetLastUID(sServerConnection, SERVER_REGISTRATION_TABLE, sBeginnigUID, sEndingUID);
        }

        public string FindLastUIDinLocalDatabase(string sBeginnigUID, string sEndingUID)
        {
            return GetLastUID(sLocalConnection, LOCAL_REGISTRATION_TABLE, sBeginnigUID, sEndingUID);
        }

        public TagInformation GetTagInformationFormServerDatabase(string sTagetUID)
        {
            string sQuery = "SELECT dbo.registration11.UID,dbo.registration11.WorkOrder,dbo.registration11.BoxNumber,dbo.registration11_Box.QuantityPerBox," +
                            "dbo.registration11.PartialBox,dbo.registration11.MachineNumber, dbo.registration11.TrimFrequency, dbo.registration11.TrimValue," +
                            "dbo.registration11.F1, dbo.registration11.F2,dbo.registration11.F1_RST,dbo.registration11.F2_RST,dbo.registration11.Temperature,dbo.registration11.DateTime " +
                            "FROM dbo.registration11 INNER JOIN dbo.registration11_Box ON dbo.registration11.BoxNumber = dbo.registration11_Box.BoxNumber " +
                            "WHERE UID='" + sTagetUID + "'";
            string[] asData = GetItemsFromDatabase(sServerConnection, sQuery);
            if (asData != null)
            {
                TagInformation tagInfo = new TagInformation();
                tagInfo.sUID = asData[0];
                tagInfo.sWorkOrder = asData[1];
                tagInfo.sBoxNumber = asData[2];
                tagInfo.sQuantityPerBox = asData[3];
                tagInfo.sPartialBox = asData[4];
                tagInfo.sMachineNumber = asData[5];
                tagInfo.sTrimFrequency = asData[6];
                tagInfo.sTrimValue = asData[7];
                tagInfo.sF1 = asData[8];
                tagInfo.sF2 = asData[9];
                tagInfo.sF1_RST = asData[10];
                tagInfo.sF2_RST = asData[11];
                tagInfo.sTemperature = asData[12];
                tagInfo.sDateTime = asData[13];
                asData = null;
                return tagInfo;
            }
            return null;
        }

        public TagInformation GetTagInformationFormLocalDatabase(string sTagetUID)
        {
            string sQuery = "SELECT * FROM " + LOCAL_REGISTRATION_TABLE + " WHERE UID='" + sTagetUID + "'"; ;
            string[] asData = GetItemsFromDatabase(sLocalConnection, sQuery);
            if (asData != null)
            {
                TagInformation tagInfo = new TagInformation();
                tagInfo.sWorkOrder = asData[1];
                ProductionParameters proParam = GetProductionParamFromServerDatabase(tagInfo.sWorkOrder);
                if (proParam != null)
                {
                    tagInfo.sUID = asData[0];
                    tagInfo.sWorkOrder = asData[1];
                    tagInfo.sBoxNumber = asData[2];
                    tagInfo.sQuantityPerBox = proParam.iQuantityPerBox.ToString();
                    tagInfo.sPartialBox = "0";
                    tagInfo.sMachineNumber = asData[3];
                    tagInfo.sTrimFrequency = asData[4];
                    tagInfo.sTrimValue = asData[5];
                    tagInfo.sF1 = asData[6];
                    tagInfo.sF2 = asData[7];
                    tagInfo.sF1_RST = asData[8];
                    tagInfo.sF2_RST = asData[9];
                    tagInfo.sTemperature = asData[10];
                    tagInfo.sDateTime = asData[11];
                    proParam = null;
                }
                asData = null;
                return tagInfo;
            }
            return null;
        }
        /*
        public string[] GetWorkOrderInformationFormServerDatabase(string sWorkOrder)
        {
            string sQuery = "SELECT * FROM " + SERVER_WORKORDER_LIST_TABLE + " WHERE WorkOrder='" + sWorkOrder + "'";
            return GetItemsFromDatabase(sServerConnection, sQuery);
        }
        */
        public string[] GetArticleInformationFormServerDatabase(string sArticleNumber)
        {
            string sQuery = "SELECT * FROM " + SERVER_ARTICLE_TABLE + " WHERE ArticleNumber='" + sArticleNumber + "'"; ;
            return GetItemsFromDatabase(sServerConnection, sQuery);
        }
        
        #endregion

        #region <-+- Private Methode -+->
        private string GetLastUID(string sODBC, string sTableName, string sBeginningUID, string sEndingUID)
        {
            string sQuery = "SELECT TOP 1 UID FROM " + sTableName + " WHERE UID BETWEEN '" + sBeginningUID + "' AND '" + sEndingUID + "' ORDER BY UID DESC";
            return GetItemFromDatabase(sODBC, sQuery);
        }
        #endregion

        public bool GetTagMeterParameterFromServerDatabase(ref string[,] asTagMeterParameter, string sArticleNumber)
        {
            sErrorMessage = null;
            int iRetValue = 0;
            string sQuery = "SELECT * FROM " + SERVER_TAG_METER_PARAMETER_TABLE + " WHERE ArticleNumber='" + sArticleNumber + "'";

            using (OdbcConnection odbcConnection = new OdbcConnection(sServerConnection))
            {
                try
                {
                    odbcConnection.Open();
                    using (OdbcCommand odbcCommand = new OdbcCommand(sQuery, odbcConnection))
                    {
                        using (OdbcDataReader odbcDataReader = odbcCommand.ExecuteReader())
                        {
                            if (odbcDataReader.HasRows)
                            {
                                using (StreamReader streamReader = new StreamReader(CONFIG_DIR + "\\TagMeterParam.cfg"))
                                {
                                    string sParameter;
                                    string[] asParameterList = null;
                                    sParameter = streamReader.ReadLine();
                                    if (sParameter != null)
                                    {
                                        asParameterList = sParameter.Split(new char[] { ',' });
                                        if (asParameterList != null)
                                        {
                                            while (odbcDataReader.Read())
                                            {
                                                for (int iRow = 0; iRow < asParameterList.Length; iRow++)
                                                {
                                                    asTagMeterParameter[iRow, 0] = asParameterList[iRow];
                                                    asTagMeterParameter[iRow, 1] = odbcDataReader[iRow + 1].ToString();
                                                }
                                                iRetValue++;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    iRetValue = -1;
                    sErrorMessage = e.Message;
                }
            }

            if (iRetValue == 0)
            {
                if (sErrorMessage == null)
                {
                    sErrorMessage = "No tag meter parameter for article '" + sArticleNumber + ",.";
                }
            }
            return iRetValue > 0;
        }

        public BoxInformation GetBoxInformationFormServerDatabase(int iBoxNumber)
        {
            string sQuery = "SELECT * FROM " + SERVER_BOX_TABLE + " WHERE BoxNumber='" + iBoxNumber + "'"; ;
            string[] sData = GetItemsFromDatabase(sServerConnection, sQuery);
            if (sData != null)
            {
                BoxInformation boxInfo = new BoxInformation();
                boxInfo.sBoxNumber = sData[0];
                boxInfo.sWorkOrder = sData[1];
                boxInfo.sQunaittyPerBox = sData[2];
                sData = null;
                return boxInfo;
            }
            return null;
        }

        public bool AddBoxNumberToBoxTable(string sBoxNumber, string sWorkOrder, int iQuantityPerBox, DateTime dataTime)
        {
            string sQuery = "INSERT INTO " + SERVER_BOX_TABLE + " (BoxNumber,WorkOrder,QuantityPerBox,DateTime) " +
                            "VALUES('" + sBoxNumber + "','" + sWorkOrder + "','" + iQuantityPerBox + "','" + dataTime + "')";

            bool bResult = ExecuteNonQuerySQL(sServerConnection, sQuery) == 1;
            if(bResult)
            {
                int iBadTag = CountBadUIDinLocalDatabase(sWorkOrder);
                if(iBadTag >= 0)
                {
                    sQuery = "INSERT INTO " + SERVER_BOX_TABLE + " (BoxNumber,WorkOrder,QuantityPerBox,DateTime) " +
                                    "VALUES('-" + sBoxNumber + "','" + sWorkOrder + "','" + iBadTag + "','" + dataTime + "')";

                    bResult = ExecuteNonQuerySQL(sServerConnection, sQuery) == 1;
                }
            }
            return bResult;
        }

        public SapInformation GetDataFromSAP(string sWorkOrder)
        {
            string sQuery = "select min(OITM.ItemName),[dbo].[@PPSONE_PRDORDERS].U_ArtNo,[dbo].[@PPSONE_PRDORDERS].U_AmouPlan ,[dbo].[@PPSONE_PRDORDERS].U_WrkStat," +
                            "[dbo].[@PPSONE_PRDORDERS].[U_StartDat],[dbo].[@PPSONE_PRDORDERS].[U_startuid],[dbo].[@PPSONE_PRDORDERS].[U_enduid] " +
                            "from OITM LEFT OUTER JOIN [dbo].[@PPSONE_PRDORDERS] ON [dbo].[@PPSONE_PRDORDERS].U_ArtNo=OITM.ItemCode " +
                            "where [dbo].[@PPSONE_PRDORDERS].U_ProdNo='" +
                            sWorkOrder +
                            "'GROUP BY [dbo].[@PPSONE_PRDORDERS].U_ArtNo,[dbo].[@PPSONE_PRDORDERS].U_AmouPlan,[dbo].[@PPSONE_PRDORDERS].U_WrkStat,[dbo].[@PPSONE_PRDORDERS].[U_StartDat],[dbo].[@PPSONE_PRDORDERS].[U_startuid],[dbo].[@PPSONE_PRDORDERS].[U_enduid]";
            sErrorMessage = null;

            SapInformation sapInfo = null;
            // Connect to SAP database server for select 
            using (OdbcConnection sapOdbcConnection = new OdbcConnection(sSapConnection))
            {
                try
                {
                    sapOdbcConnection.Open();
                    using (OdbcCommand odbcCommand = new OdbcCommand(sQuery, sapOdbcConnection))
                    {
                        odbcCommand.CommandTimeout = iCommandTimeout;
                        try
                        {
                            using (OdbcDataReader odbcDataReader = odbcCommand.ExecuteReader())
                            {
                                try
                                {
                                    if (odbcDataReader.HasRows)
                                    {
                                        if (odbcDataReader.Read())
                                        {
                                            sapInfo = new SapInformation();
                                            sapInfo.sProductName = odbcDataReader[0].ToString();
                                            sapInfo.sArticleNumber = odbcDataReader[1].ToString();
                                            sapInfo.sWorkOrderTarget = odbcDataReader[2].ToString();
                                            sapInfo.sWorkOrderTarget = sapInfo.sWorkOrderTarget.Substring(0, sapInfo.sWorkOrderTarget.IndexOf("."));
                                            sapInfo.sWorkOrderStatus = odbcDataReader[3].ToString();
                                            sapInfo.sDateTime = odbcDataReader[4].ToString();
                                        }
                                    }
                                }
                                catch (Exception e)
                                {
                                    sErrorMessage = e.Message;
                                    sapInfo = null;
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            sErrorMessage = e.Message;
                            sapInfo = null;
                        }
                    }
                }
                catch (Exception e)
                {
                    sErrorMessage = e.Message;
                    sapInfo = null;
                }
            }
            return sapInfo;
        }
    }
}