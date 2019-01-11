using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Odbc;
using System.Windows.Forms;
using System.IO;
using Utilities;

namespace Registrations
{
    public class ProductionParameters
    {
        public int iWorkOrder;
        public int iQuantityPerBox;
        public int iWorkOrderTarget;
        public int iTagType;
        public int iLabelLayout;
        public bool bRegister;
        public bool bUseTagMeter;
        public bool bUserTagProgrammer;
        public bool bTesting;
        public int iProgramOption;
        public int iPowerLevel;
        public int iTrimmingFrequency;
        public int iLabelPerBox;
        public string sUID;
        public string sBeginningUID;
        public string sEndingUID;
        public string sArticleNumber;
        public string sProductLabel;
        public string sCustormerName;
        public bool bPartialBoxClosed;
    }

    public struct ProductionVariable
    {
        public int iQuantityWorked;
        public int iQuantityInBox;
        public int iBoxNumber;
    }

    public partial class Registration
    {
        #region <-+- Constance -+->
        public const string BAD_TAG_PREFIX = "-";
        public const string NON_PARTIAL_BOX = "0";
        public const string PARTIAL_BOX = "1";
        public const string QA_REJECT = "100";

        public const string WORKORDER_STATUS_FINISHED = "3";

        public const string FAIL_BOX_PREFIX = "-";
        public const string FAIL_QUANTITY = "0";

        public const int SAP_WORK_ORDER_STATUS_NOT_START = 0;
        public const int SAP_WORK_ORDER_STATUS_PAPER_PRINTED = 1;
        public const int SAP_WORK_ORDER_STATUS_STARTED = 2;
        public const int SAP_WORK_ORDER_STATUS_FINISHED = 3;

        public const string LOCAL_REGISTRATION_TABLE = "dbo.localregistration11";
        public const string LOCAL_TESTING_TABLE = "dbo.localregistration11_Testing";

        public const string SERVER_REGISTRATION_TABLE = "dbo.registration11";
        public const string SERVER_TESTING_TABLE = "dbo.registration11_Testing";
        protected const string SERVER_BOX_TABLE = "dbo.registration11_Box";
        protected const string SERVER_ARTICLE_TABLE = "dbo.registration11_Article";
        protected const string SERVER_PRODUCTTYPE_TABLE = "dbo.registration11_ProductType";
        protected const string SERVER_WORKORDER_TABLE = "dbo.registration11_WorkOrder";
        protected const string SERVER_TAG_METER_PARAMETER_TABLE = "dbo.registration11_TagMeterParameter";

        public const int TAG_TYPE_HDX = 0;
        public const int TAG_TYPE_HDX_PLUSE = 1;
        public const int TAG_TYPE_FDX = 2;
        #endregion

        public static string ParseTagTypeToString(int iTagType)
        {
            string sTagType = null;

            switch (iTagType)
            {
                case TAG_TYPE_HDX:
                    sTagType = "HDX";
                    break;
                case TAG_TYPE_HDX_PLUSE:
                    sTagType = "HDX+";
                    break;
                case TAG_TYPE_FDX:
                    sTagType = "FDX";
                    break;
            }

            return sTagType;
        }

        public static int ParseStringToTagType(string sTagType)
        {
            int iTagType;

            switch (sTagType)
            {
                case "HDX":
                    iTagType = TAG_TYPE_HDX;
                    break;
                case "HDX+":
                    iTagType = TAG_TYPE_HDX_PLUSE;
                    break;
                default:
                    iTagType = -1;
                    break;
            }

            return iTagType;
        }

        public ProductionParameters GetProductionParamFromServerDatabase(string sWorkOrder)
        {
            string sQuery = "SELECT dbo.registration11_WorkOrder.*, dbo.registration11_Article.CustomerName, dbo.registration11_Article.LabelName, dbo.registration11_Article.LabelLayout," +
                            "dbo.registration11_Article.QuantityPerBox, dbo.registration11_Article.Register, dbo.registration11_Article.TagType, dbo.registration11_Article.TagMeterEnable," +
                            "dbo.registration11_Article.TagMeterTesting, dbo.registration11_Article.TagProgrammerEnable, dbo.registration11_Article.TagProgrammerOption," +
                            "dbo.registration11_Article.TagProgrammerPowerLevel, dbo.registration11_Article.TagProgrammerTrimingFrequency, dbo.registration11_Article.BeginningUID," +
                            "dbo.registration11_Article.EndingUID " +
                            "FROM dbo.registration11_WorkOrder INNER JOIN " +
                            "dbo.registration11_Article ON dbo.registration11_WorkOrder.ArticleNumber = dbo.registration11_Article.ArticleNumber " +
                            "WHERE WorkOrder=" + sWorkOrder;
            ProductionParameters prodParam = null;
            using (OdbcConnection odbcConnection = new OdbcConnection(sServerConnection))
            {
                try
                {
                    odbcConnection.Open();
                    using (OdbcCommand odbcCommand = new OdbcCommand(sQuery, odbcConnection))
                    {
                        try
                        {
                            odbcCommand.CommandTimeout = iCommandTimeout;
                            using (OdbcDataReader odbcDataReader = odbcCommand.ExecuteReader())
                            {
                                try
                                {
                                    if (odbcDataReader.HasRows)
                                    {
                                        if (odbcDataReader.Read())
                                        {
                                            prodParam = new ProductionParameters();
                                            prodParam.iWorkOrder = 
                                            prodParam.iQuantityPerBox =
                                            prodParam.iTrimmingFrequency = Convert.ToInt32(Convert.ToDouble(odbcDataReader["TagProgrammerTrimingFrequency"].ToString()));
                                            prodParam.iPowerLevel = Convert.ToInt32(odbcDataReader["TagProgrammerPowerLevel"].ToString());
                                            prodParam.iWorkOrder = Convert.ToInt32(odbcDataReader["WorkOrder"].ToString());
                                            prodParam.iQuantityPerBox = Convert.ToInt32(odbcDataReader["QuantityPerBox"].ToString());
                                            prodParam.iWorkOrderTarget = Convert.ToInt32(odbcDataReader["WorkOrderTarget"].ToString());
                                            prodParam.iTagType = Convert.ToInt32(odbcDataReader["TagType"].ToString());
                                            prodParam.iLabelLayout = Convert.ToInt32(odbcDataReader["LabelLayout"].ToString());
                                            prodParam.bRegister = Convert.ToBoolean(odbcDataReader["Register"].ToString());
                                            prodParam.bUseTagMeter = Convert.ToBoolean(odbcDataReader["TagMeterEnable"].ToString());
                                            prodParam.bUserTagProgrammer = Convert.ToBoolean(odbcDataReader["TagProgrammerEnable"].ToString());
                                            prodParam.bTesting = Convert.ToBoolean(odbcDataReader["TagMeterTesting"].ToString());
                                            prodParam.iProgramOption = Convert.ToInt32(odbcDataReader["TagProgrammerOption"].ToString());        
                                            prodParam.iPowerLevel = Convert.ToInt32(odbcDataReader["TagProgrammerPowerLevel"].ToString());
                                            prodParam.iTrimmingFrequency = Convert.ToInt32(odbcDataReader["TagProgrammerTrimingFrequency"].ToString());
                                            prodParam.sUID = null;
                                            prodParam.sBeginningUID = odbcDataReader["BeginningUID"].ToString();
                                            prodParam.sEndingUID = odbcDataReader["EndingUID"].ToString();
                                            prodParam.sArticleNumber = odbcDataReader["ArticleNumber"].ToString();
                                            prodParam.sProductLabel = odbcDataReader["LabelName"].ToString();
                                            prodParam.sCustormerName = odbcDataReader["CustomerName"].ToString();
                                            prodParam.bPartialBoxClosed = odbcDataReader["PartialBoxClosed"].ToString() == "1";
                                        }
                                    }
                                }
                                catch(Exception e)
                                {
                                    sErrorMessage = e.Message;
                                    prodParam = null;
                                }
                            }
                        }
                        catch(Exception e)
                        {
                            sErrorMessage = e.Message;
                            prodParam = null;
                        }
                    }
                }
                catch (Exception e)
                {
                    sErrorMessage = e.Message;
                    prodParam = null;
                }
            }

            return prodParam;
        }

        public bool AddWorkOrderToWorkOrderTable(string sWorkOrder, string sArticleNumber, string sProductName, string sWorkOrderTarget, int iQuantityWorked, int iDatabaseTransferGoodUID, int iDatabaseTransferBadUID, int iPartialBoxClosed, int iWorkOrderStatus)
        {
            string sQuery = "INSERT INTO " + SERVER_WORKORDER_TABLE + " (WorkOrder,ArticleNumber,ProductName,WorkOrderTarget,QuantityWorked,DataBaseTransferGoodUID,DataBaseTransferBadUID,PartialBoxClosed,WorkOrderStatus,InsertDate,ModifyDate) " +
                            "VALUES('" + sWorkOrder + "','" + sArticleNumber + "','" + sProductName + "','" + sWorkOrderTarget + "','" + iQuantityWorked + "','" + iDatabaseTransferGoodUID + "','" + iDatabaseTransferBadUID + "','" + iPartialBoxClosed + "','" + iWorkOrderStatus + "','" + DateTime.Now + "','" + DateTime.Now + "')";

            bool bResult = ExecuteNonQuerySQL(sServerConnection, sQuery) == 1;
            return bResult;
        }

        public int GetLastBoxNumberFromServerDatabase()
        {
            return GetLastBoxnumberFromDatabase(sServerConnection, SERVER_BOX_TABLE);
        }

        public int GetLastBoxNumberFromLocalDatabase()
        {
            return GetLastBoxnumberFromDatabase(sLocalConnection, LOCAL_REGISTRATION_TABLE);
        }

        public int GetLastBoxNumberFromServerDatabase(string sWorkOrder)
        {
            return GetLastBoxnumberFromDatabase(sServerConnection, SERVER_TESTING_TABLE, sWorkOrder);
        }

        public int GetLastBoxNumberFromLocalDatabse(string sWorkOrder)
        {
            return GetLastBoxnumberFromDatabase(sLocalConnection, LOCAL_TESTING_TABLE, sWorkOrder);
        }

        public bool UpdateQuantityWorkedInWorkOrderTable(string sWorkOrder, int iQuantity)
        {
            string sQuery = "UPDATE " + SERVER_WORKORDER_TABLE + " SET QuantityWorked='" + iQuantity + "' WHERE WorkOrder='" + sWorkOrder + "'";
            bool bResult = ExecuteNonQuerySQL(sServerConnection, sQuery) == 1;
            return bResult;
        }

        public bool UpdateQuantityWorkedInWorkOrderTable(string sWorkOrder, string sQuantity)
        {
            string sQuery = "UPDATE " + SERVER_WORKORDER_TABLE + " SET QuantityWorked='" + sQuantity + "' WHERE WorkOrder='" + sWorkOrder + "'";
            bool bResult = ExecuteNonQuerySQL(sServerConnection, sQuery) == 1;
            return bResult;
        }

        public bool UpdateDatabaseTrsnsferInWorkOrderTable(string sWorkOrder, int iDatabaseTransferGoodUID, int iDatabaseTransferBadUID)
        {
            string sQuery = "UPDATE " + SERVER_WORKORDER_TABLE + " SET DataBaseTransferGoodUID=" + iDatabaseTransferGoodUID + ",DataBaseTransferBadUID=" + iDatabaseTransferBadUID + " WHERE WorkOrder=" + sWorkOrder;
            bool bResult = ExecuteNonQuerySQL(sServerConnection, sQuery) == 1;
            return bResult;
        }

        public bool AddNewArticleToArticleTable(
            string sArticleNumber,
            string sProductType,
            string sCustomerName,
            string sLabelName,
            int iLabelLayout,
            string sQuantityPerBox,
            bool bRegister,
            int iTagType,
            bool bTagMeterEnable,
            bool bTagMeterTesting,
            bool bTagProgrammerEnable,
            int iTagProgrammerOption,
            string sTagProgrammerPowerLevel,
            string sTagProgrammerTrimingFrequency,
            string sBeginningUID,
            string sEndingUID
            )
        {
            int iTrimingFrequency = Convert.ToInt32(Convert.ToDouble(sTagProgrammerTrimingFrequency));
            string sQuery = "INSERT INTO " + SERVER_ARTICLE_TABLE + " (ArticleNumber,ProductType,CustomerName,LabelName,LabelLayout,QuantityPerBox,Register,TagType,TagMeterEnable,TagMeterTesting,TagProgrammerEnable,TagProgrammerOption,TagProgrammerPowerLevel,TagProgrammerTrimingFrequency,BeginningUID,EndingUID,InsertDate,ModifyDate) " +
                            "VALUES('" + sArticleNumber + "','" + sProductType + "','" + sCustomerName + "','" + sLabelName + "','" + iLabelLayout + "','" + sQuantityPerBox + "','" + bRegister + "','" + iTagType + "','" + bTagMeterEnable + "','" + bTagMeterTesting + "','" + bTagProgrammerEnable + "','" + iTagProgrammerOption + "','" + sTagProgrammerPowerLevel + "','" + sTagProgrammerTrimingFrequency + "','" + sBeginningUID + "','" + sEndingUID + "','" + DateTime.Now + "','" + DateTime.Now + "')";

            bool bResult = ExecuteNonQuerySQL(sServerConnection, sQuery) == 1;
            return bResult;
        }

        public bool UpdateArticleInArticleTable(
            string sArticleNumber,
            string sProductType,
            string sCustomerName,
            string sLabelName,
            int iLabelLayout,
            string sQuantityPerBox,
            bool bRegister,
            int iTagType,
            bool bTagMeterEnable,
            bool bTagMeterTesting,
            bool bTagProgrammerEnable,
            int iTagProgrammerOption,
            string sTagProgrammerPowerLevel,
            string sTagProgrammerTrimingFrequency,
            string sBeginningUID,
            string sEndingUID
            )
        {
            string sQuery = "UPDATE " + SERVER_ARTICLE_TABLE + " SET" +
                            " [ArticleNumber]='" + sArticleNumber + "'" +
                            ",[ProductType]='" + sProductType + "'" +
                            ",[CustomerName]='" + sCustomerName + "'" +
                            ",[LabelName]='" + sLabelName + "'" +
                            ",[LabelLayout]='" + iLabelLayout + "'" +
                            ",[QuantityPerBox]='" + sQuantityPerBox + "'" +
                            ",[Register]='" + bRegister + "'" +
                            ",[TagType]='" + iTagType + "'" +
                            ",[TagMeterEnable]='" + bTagMeterEnable + "'" +
                            ",[TagMeterTesting]='" + bTagMeterTesting + "'" +
                            ",[TagProgrammerEnable]='" + bTagProgrammerEnable + "'" +
                            ",[TagProgrammerOption]='" + iTagProgrammerOption + "'" +
                            ",[TagProgrammerPowerLevel]='" + sTagProgrammerPowerLevel + "'" +
                            ",[TagProgrammerTrimingFrequency]='" + sTagProgrammerTrimingFrequency + "'" +
                            ",[BeginningUID]='" + sBeginningUID + "'" +
                            ",[EndingUID]='" + sEndingUID + "'" +
                            ",[ModifyDate]='" + DateTime.Now + "'" +
                            " WHERE [ArticleNumber]='" + sArticleNumber + "'";

            bool bResult = ExecuteNonQuerySQL(sServerConnection, sQuery) == 1;
            return bResult;
        }

        public string[] ImportTagMeterParameter(string sFilePath)
        {
            string[] asTagMeterParameter = null;
            sErrorMessage = null;
            try
            {
                using (StreamReader streamReader = new StreamReader(sFilePath))
                {
                    string sParameter;
                    string[] asParameterList = null;
                    asTagMeterParameter = new string[35];
                    int iRow = 0;
                    while ((sParameter = streamReader.ReadLine()) != null)
                    {
                        asParameterList = sParameter.Split(new char[] { '=' });
                        asTagMeterParameter[iRow] = asParameterList[1].TrimStart(' ');
                        iRow++;
                    }
                }
            }
            catch (Exception e)
            {
                sErrorMessage = e.Message;
                asTagMeterParameter = null;
                #if DEBUG
                Console.WriteLine(sErrorMessage);
                #endif
            }

            return asTagMeterParameter;
        }

        public bool UpdateTagMeterParameterInTagMeterParameterTable(string sArticleNumber, string[] asParameter)
        {
            string sQuery = "UPDATE " + SERVER_TAG_METER_PARAMETER_TABLE + " SET" +
                            " [TF]=" + asParameter[0] +
                            ",[TT]=" + asParameter[1] +
                            ",[FXT]=" + asParameter[2] +
                            ",[AFT]=" + asParameter[3] +
                            ",[PRL]=" + asParameter[4] +
                            ",[PST]=" + asParameter[5] +
                            ",[PSP]=" + asParameter[6] +
                            ",[PFU]=" + asParameter[7] +
                            ",[PFL]=" + asParameter[8] +
                            ",[PACT]=" + asParameter[9] +
                            ",[PCRC]=" + asParameter[10] +
                            ",[PR]=" + asParameter[11] +
                            ",[PD]=" + asParameter[12] +
                            ",[AVG]=" + asParameter[13] +
                            ",[FMIN]=" + asParameter[14] +
                            ",[FMAX]=" + asParameter[15] +
                            ",[FLT]=" + asParameter[16] +
                            ",[TM0]=" + asParameter[17] +
                            ",[TM1]=" + asParameter[18] +
                            ",[TMD]=" + asParameter[19] +
                            ",[TTT]=" + asParameter[20] +
                            ",[TO]=" + asParameter[21] +
                            ",[TC]=" + asParameter[22] +
                            ",[PFUU]=" + asParameter[23] +
                            ",[PFUL]=" + asParameter[24] +
                            ",[PFLU]=" + asParameter[25] +
                            ",[PFLL]=" + asParameter[26] +
                            ",[PQUU]=" + asParameter[27] +
                            ",[PQUL]=" + asParameter[28] +
                            ",[PQLU]=" + asParameter[29] +
                            ",[PQLL]=" + asParameter[30] +
                            ",[PAPU]=" + asParameter[31] +
                            ",[PAPL]=" + asParameter[32] +
                            ",[PCPU]=" + asParameter[33] +
                            ",[PCPL]=" + asParameter[34] +
                            " WHERE [ArticleNumber]='" + sArticleNumber + "'";

            bool bResult = ExecuteNonQuerySQL(sServerConnection, sQuery) == 1;
            return bResult;
        }

        public bool ChangeToBadQATagInfoInServerDataBase(string sUID, string sWorkOrder, string sBoxNumber, string sDateTime)
        {
            return ChageToBadQATagInfo(sServerConnection, SERVER_REGISTRATION_TABLE, sUID, sWorkOrder, sBoxNumber, sDateTime);
        }

        public bool ChangeToBadQATagInfoInLocalDataBase(string sUID, string sWorkOrder, string sBoxNumber, string sDateTime)
        {
            return ChageToBadQATagInfo(sLocalConnection, LOCAL_REGISTRATION_TABLE, sUID, sWorkOrder, sBoxNumber, sDateTime);
        }

        public bool AddTagMeterParameterToServerDatabase(string sArticleNumber)
        {
            string sQuery = "INSERT INTO " + SERVER_TAG_METER_PARAMETER_TABLE + " (ArticleNumber) " +
                            "VALUES('" + sArticleNumber + "')";

            bool bResult = ExecuteNonQuerySQL(sServerConnection, sQuery) == 1;
            return bResult;
        }

        public bool DeleteTagMeterParameterFromTagMeterParameterTable(string sArticleNumber)
        {
            string sQuery = "DELETE FROM " + SERVER_TAG_METER_PARAMETER_TABLE + " WHERE ArticleNumber='" + sArticleNumber + "'";
            bool bResult = ExecuteNonQuerySQL(sServerConnection, sQuery) == 1;
            return bResult;
        }

        public int TagMeterArticleExitsInTagMeterParameterTable(string sArticleNumber)
        {
            string sQuery = "SELECT COUNT(ArticleNumber) FROM " + SERVER_TAG_METER_PARAMETER_TABLE + " WHERE ArticleNumber='" + sArticleNumber + "'";
            return CheckDataExistInDatabase(sServerConnection, sQuery);
        }

        public bool RetrieveArticleTable(DataGridView dataGridView)
        {
            int iRetValue = 0;
            string sQuery = "SELECT * FROM " + SERVER_ARTICLE_TABLE + " ORDER BY CustomerName";
            sErrorMessage = null;

            using (OdbcConnection odbcConnection = new OdbcConnection(sServerConnection))
            {
                try
                {
                    odbcConnection.Open();
                    using (OdbcCommand odbcCommand = new OdbcCommand(sQuery, odbcConnection))
                    {
                        using (OdbcDataReader odbcDataReader = odbcCommand.ExecuteReader())
                        {
                            dataGridView.Rows.Clear();
                            if (odbcDataReader.HasRows)
                            {
                                while (odbcDataReader.Read())
                                {
                                    dataGridView.Rows.Add(
                                        odbcDataReader[0].ToString(),//[ArticleNumber]
                                        odbcDataReader[1].ToString(),//[ProductType]
                                        ParseTagTypeToString(Convert.ToInt32(odbcDataReader[7].ToString())),//TagType
                                        odbcDataReader[2].ToString(),//[CustomerName]
                                        odbcDataReader[3].ToString(),//[LabelName]
                                        odbcDataReader[4].ToString(),// == "0" ? "BOX" : "CARTON",//[LabelLayout]
                                        odbcDataReader[5].ToString(),//[QuantityPerBox]
                                        odbcDataReader[6].ToString() == "True" ? "Register" : "Not Register",//[Register]
                                        odbcDataReader[8].ToString() == "True" ? "Use" : "No Use",//[TagMeterEnable]
                                        odbcDataReader[9].ToString() == "True" ? "Test" : "Not Test",//[TagMeterTesting]
                                        odbcDataReader[10].ToString() == "True" ? "Use" : "No Use",//[TagProgrammerEnable]
                                        odbcDataReader[11].ToString(),//[TagProgrammerOption]
                                        odbcDataReader[12].ToString(),//[TagProgrammerPowerLevel]
                                        odbcDataReader[13].ToString(),//[TagProgrammerTrimingFrequency]
                                        odbcDataReader[14].ToString(),//[BeginningUID]
                                        odbcDataReader[15].ToString()//,[EndingUID]                                        
                                    );
                                    iRetValue++;
                                }
                            }
                            dataGridView.ClearSelection();
                        }
                    }
                }
                catch (Exception e)
                {
                    iRetValue = -1;
                    sErrorMessage = e.Message;
                }
            }

            return iRetValue >= 0;
        }

        public bool RetrieveProductTypeFromServerDatabase(DataGridView dataGridView)
        {
            int iRetValue = 0;
            string sQuery = "SELECT * FROM " + SERVER_PRODUCTTYPE_TABLE;
            sErrorMessage = null;

            using (OdbcConnection odbcConnection = new OdbcConnection(sServerConnection))
            {
                try
                {
                    odbcConnection.Open();
                    using (OdbcCommand odbcCommand = new OdbcCommand(sQuery, odbcConnection))
                    {
                        using (OdbcDataReader odbcDataReader = odbcCommand.ExecuteReader())
                        {
                            dataGridView.Rows.Clear();
                            if (odbcDataReader.HasRows)
                            {
                                while (odbcDataReader.Read())
                                {
                                    dataGridView.Rows.Add(
                                        odbcDataReader[0].ToString(),
                                        ParseTagTypeToString(Convert.ToInt32(odbcDataReader[1].ToString())),//TagType
                                        odbcDataReader[2].ToString() == "True" ? "Test" : "Not Test",
                                        odbcDataReader[3].ToString() == "True" ? "Trim" : "Not Trim",
                                        odbcDataReader[4].ToString() == "True" ? "Program" : "Not Program",
                                        odbcDataReader[5].ToString() == "True" ? "Lock" : "Not Lock",
                                        odbcDataReader[6].ToString() == "True" ? "Register" : "Not Register",
                                        odbcDataReader[7].ToString(),                                       
                                        odbcDataReader[8].ToString(),                                       
                                        odbcDataReader[9].ToString(),                                       
                                        odbcDataReader[10].ToString(),                                       
                                        odbcDataReader[11].ToString()                                     
                                    );
                                    iRetValue++;
                                }
                            }
                            dataGridView.ClearSelection();
                        }
                    }
                }
                catch (Exception e)
                {
                    sErrorMessage = e.Message;
                }
            }

            return iRetValue >= 0;
        }
        
        public bool RetrieveWorkOrderTable(DataGridView dataGridView, bool bHideFinishedWorkOrder, int iUserID)
        {
            int iRetValue = 0;
            string sQuery = "SELECT dbo.registration11_WorkOrder.WorkOrder, dbo.registration11_WorkOrder.ArticleNumber, dbo.registration11_Article.ProductType," +
                            "dbo.registration11_Article.TagType, dbo.registration11_Article.CustomerName, dbo.registration11_WorkOrder.ProductName, dbo.registration11_Article.LabelName," +
                            "dbo.registration11_Article.LabelLayout, dbo.registration11_Article.QuantityPerBox, dbo.registration11_WorkOrder.WorkOrderTarget," +
                            "dbo.registration11_WorkOrder.QuantityWorked, dbo.registration11_WorkOrder.DataBaseTransferGoodUID," +
                            "dbo.registration11_WorkOrder.DataBaseTransferBadUID, dbo.registration11_WorkOrder.PartialBoxClosed," +
                            "dbo.registration11_WorkOrder.WorkOrderStatus, dbo.registration11_Article.Register, dbo.registration11_Article.TagMeterEnable," +
                            "dbo.registration11_Article.TagMeterTesting, dbo.registration11_Article.TagProgrammerEnable, dbo.registration11_Article.TagProgrammerOption," +
                            "dbo.registration11_Article.TagProgrammerPowerLevel, dbo.registration11_Article.TagProgrammerTrimingFrequency, dbo.registration11_Article.BeginningUID," +
                            "dbo.registration11_Article.EndingUID " +
                            "FROM dbo.registration11_WorkOrder INNER JOIN " +
                            "dbo.registration11_Article ON dbo.registration11_WorkOrder.ArticleNumber = dbo.registration11_Article.ArticleNumber";
            
            if (bHideFinishedWorkOrder)
            {
                sQuery += " WHERE WorkOrderStatus <> 3";
            }
            else
            {
                if (iUserID == Login.USER_ID_QA)
                {
                    sQuery += " WHERE LEN(WorkOrder) < 5";
                }
                else
                {
                    sQuery += " WHERE LEN(WorkOrder) = 5";
                }
            }
            sErrorMessage = null;

            using (OdbcConnection odbcConnection = new OdbcConnection(sServerConnection))
            {
                try
                {
                    odbcConnection.Open();
                    using (OdbcCommand odbcCommand = new OdbcCommand(sQuery, odbcConnection))
                    {
                        using (OdbcDataReader odbcDataReader = odbcCommand.ExecuteReader())
                        {
                            dataGridView.Rows.Clear();
                            if (odbcDataReader.HasRows)
                            {
                                while (odbcDataReader.Read())
                                {
                                    dataGridView.Rows.Add(
                                        odbcDataReader[0].ToString(),
                                        odbcDataReader[1].ToString(),
                                        odbcDataReader[2].ToString(),
                                        ParseTagTypeToString(Convert.ToInt32(odbcDataReader[3].ToString())),
                                        odbcDataReader[4].ToString(),
                                        odbcDataReader[5].ToString(),
                                        odbcDataReader[6].ToString(),
                                        odbcDataReader[7].ToString(),
                                        odbcDataReader[8].ToString(),
                                        odbcDataReader[9].ToString(),
                                        odbcDataReader[10].ToString(),
                                        odbcDataReader[11].ToString(),
                                        odbcDataReader[12].ToString(),
                                        odbcDataReader[13].ToString() == "1" ? "Close" : "Not Close",// Partial Box Closed
                                        ParseWorkOrderStatusToString(odbcDataReader[14].ToString()),// Work Order Status
                                        odbcDataReader[15].ToString() == "True" ? "Register" : "Not Register",// Register
                                        odbcDataReader[16].ToString() == "True" ? "Use" : "Not Use",// Use Tag Meter
                                        odbcDataReader[17].ToString() == "True" ? "Test" : "Not Test",// Testing
                                        odbcDataReader[18].ToString() == "True" ? "Use" : "Not Use",// Use Tag Programmer
                                        ParseProgramOptonToString(Convert.ToInt32(odbcDataReader[19].ToString())),// Program Option
                                        odbcDataReader[20].ToString(),
                                        odbcDataReader[21].ToString(),
                                        odbcDataReader[22].ToString(),
                                        odbcDataReader[23].ToString()
                                        );
                                    iRetValue++;
                                }
                                dataGridView.ClearSelection();
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    iRetValue = -1;
                    sErrorMessage = e.Message;
                }
                return iRetValue >= 0;
            }
        }

        public bool RetrieveTagMeterParameterTable(DataGridView dataGridView)
        {
            sErrorMessage = null;
            int iRetValue = 0;
            string sQuery = "SELECT * FROM " + SERVER_TAG_METER_PARAMETER_TABLE + " ORDER BY ArticleNumber";

            using (OdbcConnection odbcConnection = new OdbcConnection(sServerConnection))
            {
                try
                {
                    odbcConnection.Open();
                    using (OdbcCommand odbcCommand = new OdbcCommand(sQuery, odbcConnection))
                    {
                        using (OdbcDataReader odbcDataReader = odbcCommand.ExecuteReader())
                        {
                            dataGridView.Rows.Clear();
                            if (odbcDataReader.HasRows)
                            {
                                while (odbcDataReader.Read())
                                {
                                    dataGridView.Rows.Add(
                                        odbcDataReader[0].ToString(),
                                        odbcDataReader[1].ToString(),
                                        odbcDataReader[2].ToString(),
                                        odbcDataReader[3].ToString(),
                                        odbcDataReader[4].ToString(),
                                        odbcDataReader[5].ToString(),
                                        odbcDataReader[6].ToString(),
                                        odbcDataReader[7].ToString(),
                                        odbcDataReader[8].ToString(),
                                        odbcDataReader[9].ToString(),
                                        odbcDataReader[10].ToString(),
                                        odbcDataReader[11].ToString(),
                                        odbcDataReader[12].ToString(),
                                        odbcDataReader[13].ToString(),
                                        odbcDataReader[14].ToString(),
                                        odbcDataReader[15].ToString(),
                                        odbcDataReader[16].ToString(),
                                        odbcDataReader[17].ToString(),
                                        odbcDataReader[18].ToString(),
                                        odbcDataReader[19].ToString(),
                                        odbcDataReader[20].ToString(),
                                        odbcDataReader[21].ToString(),
                                        odbcDataReader[22].ToString(),
                                        odbcDataReader[23].ToString(),
                                        odbcDataReader[24].ToString(),
                                        odbcDataReader[25].ToString(),
                                        odbcDataReader[26].ToString(),
                                        odbcDataReader[27].ToString(),
                                        odbcDataReader[28].ToString(),
                                        odbcDataReader[29].ToString(),
                                        odbcDataReader[30].ToString(),
                                        odbcDataReader[31].ToString(),
                                        odbcDataReader[32].ToString(),
                                        odbcDataReader[33].ToString(),
                                        odbcDataReader[34].ToString(),
                                        odbcDataReader[35].ToString()
                                        );
                                    iRetValue++;
                                }
                            }
                            dataGridView.ClearSelection();
                        }
                    }
                }
                catch (Exception e)
                {
                    iRetValue = -1;
                    sErrorMessage = e.Message;
                }
            }

            return iRetValue >= 0;
        }

        public bool RetrieveTagMeterParameter(DataGridView dataGridView, string sArticleNumber)
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
                            dataGridView.Rows.Clear();
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
                                                for (int iRow = 1; iRow < odbcDataReader.FieldCount; iRow++)
                                                {
                                                    dataGridView.Rows.Add(asParameterList[iRow - 1], odbcDataReader[iRow]);
                                                }
                                                iRetValue++;
                                            }
                                        }
                                    }
                                }
                            }
                            dataGridView.ClearSelection();
                        }
                    }
                }
                catch (Exception e)
                {
                    iRetValue = -1;
                    sErrorMessage = e.Message;
                }
            }

            return iRetValue >= 0;
        }

        public bool ImportTagMeterParameter(DataGridView dataGridView, string sFilePath)
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

        public bool RetrieveRegistrationDataFromLocalDatabase(DataGridView dataGridView, string sWorkOrder, string sBoxNumber)
        {
            int iRetValue = 0;
            string sQuery = "SELECT * FROM " + LOCAL_REGISTRATION_TABLE + " WHERE BoxNumber=" + sBoxNumber;
            if (sWorkOrder != null)
            {
                sQuery += " AND WorkOrder=" + sWorkOrder;
            }

            sQuery += " AND BoxNumber>0";

            using (OdbcConnection odbcConnection = new OdbcConnection(sLocalConnection))
            {
                try
                {
                    odbcConnection.Open();
                    using (OdbcCommand odbcCommand = new OdbcCommand(sQuery, odbcConnection))
                    {
                        using (OdbcDataReader odbcDataReader = odbcCommand.ExecuteReader())
                        {
                            dataGridView.Rows.Clear();
                            if (odbcDataReader.HasRows)
                            {
                                int iRow = 0;
                                while (odbcDataReader.Read())
                                {
                                    iRow++;
                                    dataGridView.Rows.Add(
                                        iRow.ToString(),
                                        odbcDataReader[0].ToString(),   // UID
                                        odbcDataReader[1].ToString(),   // WorkOrder
                                        odbcDataReader[2].ToString(),   // BoxNumber
                                        odbcDataReader[3].ToString(),   // MachineNumber
                                        //odbcDataReader[4].ToString(),   // QuantityPerBox
                                        Convert.ToDouble(odbcDataReader[4].ToString()).ToString("F03"),   // TrimFrequency
                                        odbcDataReader[5].ToString(),   // TrimValue
                                        Convert.ToDouble(odbcDataReader[6].ToString()).ToString("F03"),   // F1
                                        Convert.ToDouble(odbcDataReader[7].ToString()).ToString("F03"),   // F2
                                        Convert.ToDouble(odbcDataReader[8].ToString()).ToString("F03"),   // F1_RST
                                        Convert.ToDouble(odbcDataReader[9].ToString()).ToString("F03"),   // F2_RST
                                        Convert.ToDouble(odbcDataReader[10].ToString()).ToString("F01"),  // Temperature
                                        odbcDataReader[11].ToString()   // DatTime
                                        );
                                    iRetValue++;
                                }
                                if(iRow > 0)
                                {
                                    dataGridView.CurrentCell = dataGridView.Rows[dataGridView.RowCount - 1].Cells[0];
                                }
                                dataGridView.ClearSelection();
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    iRetValue = -1;
                    sErrorMessage = e.Message;
                    #if DEBUG
                    Console.WriteLine(sErrorMessage);
                    #endif
                }
            }

            return iRetValue >= 0;
        }

        public bool RetrieveTestingDataFromLocalDatabase(DataGridView dataGridView, string sWorkOrder, string sBoxNumber)
        {
            int iRetVal = 0;
            string sQuery = "SELECT * FROM " + LOCAL_TESTING_TABLE + " WHERE BoxNumber='" + sBoxNumber + "'";
            if (sWorkOrder != null)
            {
                sQuery += " AND WorkOrder='" + sWorkOrder + "'";
            }
            sQuery += " ORDER BY DateTime";
            //sQuery += " AND Result = 1" + " ORDER BY DateTime";
            sErrorMessage = null;
            using (OdbcConnection odbcConnection = new OdbcConnection(sLocalConnection))
            {
                try
                {
                    odbcConnection.Open();
                    using (OdbcCommand odbcCommand = new OdbcCommand(sQuery, odbcConnection))
                    {
                        using (OdbcDataReader odbcDataReader = odbcCommand.ExecuteReader())
                        {
                            dataGridView.Rows.Clear();
                            if (odbcDataReader.HasRows)
                            {
                                string sTemperature;
                                double dTemperature;
                                int iRow = 0;
                                while (odbcDataReader.Read())
                                {
                                    iRow++;
                                    dTemperature = Convert.ToDouble(odbcDataReader[9].ToString()) / 10;
                                    sTemperature = String.Format("{0}", dTemperature);
                                    dataGridView.Rows.Add(
                                        iRow.ToString(),
                                        odbcDataReader[0].ToString(),
                                        String.Format("{0:D5}", odbcDataReader[1]),
                                        odbcDataReader[2].ToString(),
                                        odbcDataReader[3].ToString(),
                                        odbcDataReader[4].ToString(),
                                        odbcDataReader[5].ToString(),
                                        odbcDataReader[6].ToString(),
                                        odbcDataReader[7].ToString(),
                                        odbcDataReader[8].ToString(),
                                        sTemperature,//odbcDataReader[9].ToString(),
                                        odbcDataReader[10].ToString(),
                                        odbcDataReader[11].ToString(),
                                        odbcDataReader[12].ToString(),
                                        odbcDataReader[14].ToString()
                                        );
                                    iRetVal++;
                                }
                                if (iRow > 0)
                                {
                                    dataGridView.CurrentCell = dataGridView.Rows[dataGridView.RowCount - 1].Cells[0];
                                }
                                dataGridView.ClearSelection();
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    iRetVal = -1;
                    sErrorMessage = e.Message;
                    #if DEBUG
                    Console.WriteLine(sErrorMessage);
                    #endif
                }
            }
            return iRetVal >= 0;
        }

        public int CountTestingTagInServerDatabase(string sWorkOrder)
        {
            string sQuery = "SELECT COUNT (*) FROM " + SERVER_TESTING_TABLE + " WHERE WorkOrder='" + sWorkOrder + "' AND Result=1";
            return CheckDataExistInDatabase(sServerConnection, sQuery);
        }

        public int CountTestingTagInLocalDatabase(string sWorkOrder)
        {
            string sQuery = "SELECT COUNT (*) FROM " + LOCAL_TESTING_TABLE + " WHERE WorkOrder='" + sWorkOrder + "' AND Result=1";
            return CheckDataExistInDatabase(sLocalConnection, sQuery);
        }

        const int TAG_METER_RESULT_OK = 0;
        const int TAG_METER_RESULT_HW_ERROR = 1;
        const int TAG_METER_RESULT_FULL = 2;
        const int TAG_METER_RESULT_FULU = 3;
        const int TAG_METER_RESULT_QULL = 4;
        const int TAG_METER_RESULT_QULU = 5;
        const int TAG_METER_RESULT_FULP = 6;
        const int TAG_METER_RESULT_FLL = 7;
        const int TAG_METER_RESULT_FLU = 8;
        const int TAG_METER_RESULT_QLL = 9;
        const int TAG_METER_RESULT_QLU = 10;
        const int TAG_METER_RESULT_FLP = 11;
        const int TAG_METER_RESULT_CRC_12 = 12;
        const int TAG_METER_RESULT_CRC_13 = 13;

        public int CountReject1TestingTagInLocalDatabase(string sWorkOrder)
        {
            string sQuery = "SELECT COUNT (*) FROM " + LOCAL_TESTING_TABLE + " WHERE WorkOrder='" + sWorkOrder + "' AND Status=" + TAG_METER_RESULT_FULL;
            return CheckDataExistInDatabase(sLocalConnection, sQuery);
        }

        public int CountReject2TestingTagInLocalDatabase(string sWorkOrder)
        {
            string sQuery = "SELECT COUNT (*) FROM " + LOCAL_TESTING_TABLE + " WHERE WorkOrder='" + sWorkOrder + "' AND Status=" + TAG_METER_RESULT_FULU;
            return CheckDataExistInDatabase(sLocalConnection, sQuery);
        }

        public int CountReject3TestingTagInLocalDatabase(string sWorkOrder)
        {
            string sQuery = "SELECT COUNT (*) FROM " + LOCAL_TESTING_TABLE + " WHERE WorkOrder='" + sWorkOrder + "' AND ( Status=" + TAG_METER_RESULT_FULP + " OR Status=" + TAG_METER_RESULT_FLP + ")";
            return CheckDataExistInDatabase(sLocalConnection, sQuery);
        }

        public int CountReject4TestingTagInLocalDatabase(string sWorkOrder)
        {
            string sQuery = "SELECT COUNT (*) FROM " + LOCAL_TESTING_TABLE + " WHERE WorkOrder=" + sWorkOrder +
                " AND Status<>" + TAG_METER_RESULT_OK +
                " AND Status<>" + TAG_METER_RESULT_FULL +
                " AND Status<>" + TAG_METER_RESULT_FULU +
                " AND Status<>" + TAG_METER_RESULT_FULP +
                " AND Status<>" + TAG_METER_RESULT_FLP;

            return CheckDataExistInDatabase(sLocalConnection, sQuery);
        }

        public string[] GetProductTypeInformationFormServerDatabase(string sProductType)
        {
            string sQuery = "SELECT * FROM " + SERVER_PRODUCTTYPE_TABLE + " WHERE ProductType='" + sProductType + "'"; ;
            return GetItemsFromDatabase(sServerConnection, sQuery);
        }

        public bool SaveTestingDataToLocalDataBase(string sWorkOrder, string sBoxNumber, int iStatus, int iF0_Unload, int iF0_Load, int iQ_Unload, int iQ_Load, int iACT_Power, int iModulationDepth, int iTemperature, int iDeltaFreqVsTemp, string sAID_HEX, string sAID_DEC, int iResult, DateTime dateTime)
        {
            return SaveTestingDataToDatabase(sLocalConnection, LOCAL_TESTING_TABLE, sWorkOrder, sBoxNumber, iStatus, iF0_Unload, iF0_Load, iQ_Unload, iQ_Load, iACT_Power, iModulationDepth, iTemperature, iDeltaFreqVsTemp, sAID_HEX, sAID_DEC, iResult, dateTime);
        }

        public bool SeveTestingDataToServerDataBase(string sWorkOrder, string sBoxNumber, int iStatus, int iF0_Unload, int iF0_Load, int iQ_Unload, int iQ_Load, int iACT_Power, int iModulationDepth, int iTemperature, int iDeltaFreqVsTemp, string sAID_HEX, string sAID_DEC, int iResult, DateTime dateTime)
        {
            return SaveTestingDataToDatabase(sServerConnection, SERVER_TESTING_TABLE, sWorkOrder, sBoxNumber, iStatus, iF0_Unload, iF0_Load, iQ_Unload, iQ_Load, iACT_Power, iModulationDepth, iTemperature, iDeltaFreqVsTemp, sAID_HEX, sAID_DEC, iResult, dateTime);
        }

        /*
        public bool UpdateQuantityPerBoxInLocalDatadase(string sBoxNumber, string sQuantityPerBox)
        {
            return UpdateQuantityPerBox(sLocalConnection, LOCAL_REGISTRATION_TABLE, sBoxNumber, sQuantityPerBox);
        }
        */
        public bool UpdateWorkOrderStatusInWorkOrderTable(string sWorkOrder, string sWorkOrderStatus)
        {
            string sQuery = "UPDATE " + SERVER_WORKORDER_TABLE + " SET WorkOrderStatus='" + sWorkOrderStatus + "' WHERE WorkOrder='" + sWorkOrder + "'";
            bool bResult = ExecuteNonQuerySQL(sServerConnection, sQuery) == 1;
            return bResult;
        }

        public bool UpdatePartialBoxClosedStatusInWorkOrderTable(string sWorkOrder, int iPartialBoxClosed)
        {
            string sQuery = "UPDATE " + SERVER_WORKORDER_TABLE + " SET PartialBoxClosed='" + iPartialBoxClosed + "' WHERE WorkOrder='" + sWorkOrder + "'";
            bool bResult = ExecuteNonQuerySQL(sServerConnection, sQuery) == 1;
            return bResult;
        }

        public bool RegisterTagToLocalDatabase(string sTagUID, string sWorkOrder, string sBoxNumber, string sMachineNo, string sTrimFrequency, string sTrimValue, string sF1, string sF2, string sF1_RST, string sF2_RST, string sTemperature, string sDateTime, string sPartialBox)
        {
            return Register(sLocalConnection, LOCAL_REGISTRATION_TABLE, sTagUID, sWorkOrder, sBoxNumber, sMachineNo, sTrimFrequency, sTrimValue, sF1, sF2, sF1_RST, sF2_RST, sTemperature, sDateTime, sPartialBox);
        }

        public bool RegisterTagToServerDatabase(string sTagUID, string sWorkOrder, string sBoxNumber, string sMachineNo, string sTrimFrequency, string sTrimValue, string sF1, string sF2, string sF1_RST, string sF2_RST, string sTemperature, string sDateTime, string sPartialBox)
        {
            return Register(sServerConnection, SERVER_REGISTRATION_TABLE, sTagUID, sWorkOrder, sBoxNumber, sMachineNo, sTrimFrequency, sTrimValue, sF1, sF2, sF1_RST, sF2_RST, sTemperature, sDateTime, sPartialBox);
        }

        #region <-+- Private Methode -+->
        private int GetLastBoxnumberFromDatabase(string sODBC, string sTable)
        {
            int iLastBoxNumber = -1;
            string sQuery = "SELECT TOP 1 BoxNumber FROM " + sTable + " ORDER BY BoxNumber DESC";
            string sLastBoxNumber = GetItemFromDatabase(sODBC, sQuery);
            if (sLastBoxNumber != null)
            {
                iLastBoxNumber = Convert.ToInt32(sLastBoxNumber);
            }
            else if (sErrorMessage == null)
            {
                iLastBoxNumber = 0;
            }
            return iLastBoxNumber;
        }

        private int GetLastBoxnumberFromDatabase(string sODBC, string sTable, string sWorkOrder)
        {
            int iLastBoxNumber = -1;
            string sQuery = "SELECT TOP 1 BoxNumber FROM " + sTable + " WHERE WorkOrder=" + sWorkOrder + " ORDER BY BoxNumber DESC";
            string sLastBoxNumber = GetItemFromDatabase(sODBC, sQuery);
            if (sLastBoxNumber != null)
            {
                iLastBoxNumber = Convert.ToInt32(sLastBoxNumber);
            }
            else if (sErrorMessage == null)
            {
                iLastBoxNumber = 0;
            }
            return iLastBoxNumber;
        }

        private bool ChageToBadQATagInfo(string sODBC, string sTableName, string sUID, string sWorkOrder, string sBoxNumber, string sDateTime)
        {
            string sQuery = "UPDATE " + sTableName + " SET BoxNumber='" + FAIL_BOX_PREFIX + sBoxNumber + "'" + ",PartialBox=" + QA_REJECT +
                            " WHERE UID='" + sUID + "' AND WorkOrder='" + sWorkOrder + "' AND BoxNumber='" + sBoxNumber + "'";

            bool bResult = ExecuteNonQuerySQL(sODBC, sQuery) == 1;
            return bResult;
        }

        private bool SaveTestingDataToDatabase(string sODBC, string sTableName, string sWorkOrder, string sBoxNumber, int iStatus, int iF0_Unload, int iF0_Load, int iQ_Unload, int iQ_Load, int iACT_Power, int iModulationDepth, int iTemperature, int iDeltaFreqVsTemp, string sAID_HEX, string sAID_DEC, int iResult, DateTime dateTime)
        {
            string sQuery = "INSERT INTO " + sTableName + " (WorkOrder,BoxNumber,Status,F0_Unload,F0_Load,Q_Unload,Q_Load,ACT_Power,ModulationDepth,Temperature,DeltaFreqVsTemp,AID_HEX,AID_DEC,Result,DateTime)" +
                            " VALUES('" + sWorkOrder + "','" + sBoxNumber + "','" + iStatus + "','" + iF0_Unload + "','" + iF0_Load + "','" + iQ_Unload + "','" + iQ_Load + "','" + iACT_Power + "','" + iModulationDepth + "','" + iTemperature + "','" + iDeltaFreqVsTemp + "','" + sAID_HEX + "','" + sAID_DEC + "','" + iResult + "','" + dateTime + "')";

            bool bResult = ExecuteNonQuerySQL(sODBC, sQuery) == 1;
            return bResult;
        }

        /*
        private bool UpdateQuantityPerBox(string sODBC, string sTableName, string sBoxNumber, string sQuantity)
        {
            string sQuery = "UPDATE " + sTableName + " SET QuantityPerBox=" + sQuantity + ",PartialBox=" + PARTIAL_BOX +
                            " WHERE BoxNumber=" + sBoxNumber;

            bool bResult = ExecuteNonQuerySQL(sODBC, sQuery) >= 0;
            return bResult;
        }
        */
        private bool Register(string sODBC, string sTableName, string sUID, string sWorkOrder, string sBoxNumber, string sMachineNo, string sTrimFrequency, string sTrimValue, string sF1, string sF2, string sF1_RST, string sF2_RST, string sTemperature, string sDateTime, string sPartialBox)
        {
            string sQuery = "INSERT INTO " + sTableName + " ([UID],[WorkOrder],[BoxNumber],[MachineNumber],[TrimFrequency],[TrimValue],[F1],[F2],[F1_RST],[F2_RST],[Temperature],[DateTime],[PartialBox])" +
                            " VALUES('" + sUID + "','" + sWorkOrder + "','" + sBoxNumber + "','" + sMachineNo + "','" + sTrimFrequency + "','" + sTrimValue + "','" + sF1 + "','" + sF2 + "','" + sF1_RST + "','" + sF2_RST + "','" + sTemperature + "','" + sDateTime + "','" + sPartialBox + "')";

            bool bResult = ExecuteNonQuerySQL(sODBC, sQuery) == 1;
            return bResult;
        }       
        #endregion
    }
}
