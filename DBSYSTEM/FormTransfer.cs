using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using System.Data.SqlClient;
using System.Data.Odbc;

namespace Registrations
{
    public partial class FormTransfer : Form
    {
        const int OPTION_COUNT_ALL = 1;
        const int OPTION_COUNT_GOOD_ONLY = 2;
        const int OPTION_COUNT_BAD_ONLY = 3;

        const string SERVER_REGISTRATION_TABLE = Registration.SERVER_REGISTRATION_TABLE;
        const string LOCAL_REGISTRATION_TABLE = Registration.LOCAL_REGISTRATION_TABLE;

        OdbcConnection serverOdbcConnection = null;
        OdbcCommand serverOdbcCommand = null;

        OdbcConnection localOdbcConnection = null;
        OdbcCommand localOdbcCommand = null;
        OdbcDataReader localOdbcDataReader = null;

        string sServerConnection;
        string sLocalConnection;
        string sWorkOrder;

        string sServerQuery;
        string sLocalQuery;
        string sErrorMessage;
        string sTagUID;
        
        int iStep;
        int iTotalRow, iRow, iRetVal;
        int iLoopPercent;
        int iPercentComplete;

        bool bTransferSuccess;

        public bool TransferSuccess { get { return bTransferSuccess; } }
        public int TotalTransfered { get { return iRow; } }
        public string ErrorMessage { get { return sErrorMessage; } }

        public FormTransfer(string sServerConnection, string sLocalConnection, string sWorkOrder)
        {
            InitializeComponent();
            this.sServerConnection = sServerConnection;
            this.sLocalConnection = sLocalConnection;
            this.sWorkOrder = sWorkOrder;
            serverOdbcCommand = new OdbcCommand();
            localOdbcCommand = new OdbcCommand();
        }

        private void FormTransfer_Load(object sender, EventArgs e)
        {
            iRow = 0;
            iStep = 0;
            bTransferSuccess = false;
            btnOK.Enabled = false;
        }

        private void FormTransfer_Shown(object sender, EventArgs e)
        {
            timerTransfer.Enabled = true;
        }

        private void FormTransfer_FormClosing(object sender, FormClosingEventArgs e)
        {
            sErrorMessage = null;
            timerTransfer.Enabled = false;
            try
            {
                localOdbcConnection.Close();
                serverOdbcConnection.Close();
            }
            catch (Exception ex)
            {
                sErrorMessage = ex.Message;
            }
        }

        bool ConnectToServerDatabase()
        {
            bool bSuccess = false;
            sErrorMessage = null;
            try
            {
                serverOdbcConnection = new OdbcConnection(sServerConnection);
                serverOdbcConnection.Open();
                serverOdbcCommand.Connection = serverOdbcConnection;
                bSuccess = true;
            }
            catch (OdbcException e)
            {
                sErrorMessage = e.Message;
            }
            catch (Exception e)
            {
                sErrorMessage = e.Message;
            }
            return bSuccess;
        }

        bool ConnectToLocalDatabase()
        {
            bool bSuccess = false;
            sErrorMessage = null;
            try
            {
                localOdbcConnection = new OdbcConnection(sLocalConnection);
                localOdbcConnection.Open();
                localOdbcCommand.Connection = localOdbcConnection;
                bSuccess = true;
            }
            catch (OdbcException e)
            {
                sErrorMessage = e.Message;
            }
            catch (Exception e)
            {
                sErrorMessage = e.Message;
            }
            return bSuccess;
        }

        int GetRowCount(int iOption)
        {
            int iRowCount = 0;
            sErrorMessage = null;
            sLocalQuery = "SELECT COUNT(*) FROM " + LOCAL_REGISTRATION_TABLE + " WHERE WorkOrder='" + sWorkOrder + "'";
            switch (iOption)
            {
                case OPTION_COUNT_GOOD_ONLY:
                    sLocalQuery += " AND BoxNumber>0";
                    break;
                case OPTION_COUNT_BAD_ONLY:
                    sLocalQuery += " AND BoxNumber<0";
                    break;
            }

            try
            {
                localOdbcCommand.CommandText = sLocalQuery;
                localOdbcDataReader = localOdbcCommand.ExecuteReader();

                if(localOdbcDataReader.HasRows)
                {
                    localOdbcDataReader.Read();
                    iRowCount = Convert.ToInt32(localOdbcDataReader[0].ToString());
                    localOdbcDataReader.Close();
                }
            }
            catch (OdbcException e)
            {
                sErrorMessage = e.Message;
                iRowCount = -1;
            }
            catch (Exception e)
            {
                sErrorMessage = e.Message;
                iRowCount = -2;
            }
            return iRowCount;
        }

        bool TransferToServer()
        {
            bool bSuccess = false;
            sErrorMessage = null;
            sLocalQuery = "SELECT TOP 1 * FROM " + LOCAL_REGISTRATION_TABLE + " WHERE WorkOrder='" + sWorkOrder + "'";
           
            try
            {
                localOdbcCommand.CommandText = sLocalQuery;
                localOdbcDataReader = localOdbcCommand.ExecuteReader();
                if (localOdbcDataReader.HasRows)
                {
                    if (localOdbcDataReader.Read())
                    {
                        sServerQuery = "INSERT INTO " + SERVER_REGISTRATION_TABLE + " ([UID],[WorkOrder],[BoxNumber],[MachineNumber],[TrimFrequency],[TrimValue],[F1],[F2],[F1_RST],[F2_RST],[Temperature],[DateTime],[PartialBox]) " +
                                        "VALUES('" +
                                        localOdbcDataReader[0] + "','" + localOdbcDataReader[1] + "','" + localOdbcDataReader[2] + "','" +
                                        localOdbcDataReader[3] + "','" + localOdbcDataReader[4] + "','" + localOdbcDataReader[5] + "','" +
                                        localOdbcDataReader[6] + "','" + localOdbcDataReader[7] + "','" + localOdbcDataReader[8] + "','" +
                                        localOdbcDataReader[9] + "','" + localOdbcDataReader[10] + "','" + localOdbcDataReader[11] + "','" +
                                        localOdbcDataReader[12] + "')";
                        sTagUID = localOdbcDataReader[0].ToString();
                        localOdbcDataReader.Close();
                        serverOdbcCommand.CommandText = sServerQuery;
                        iRetVal = serverOdbcCommand.ExecuteNonQuery();
                        if (iRetVal == 1)
                        {
                            sLocalQuery = "DELETE FROM " + LOCAL_REGISTRATION_TABLE + " WHERE UID='" + sTagUID + "' AND WorkOrder='" + sWorkOrder + "'";
                            localOdbcCommand.CommandText = sLocalQuery;
                            iRetVal = localOdbcCommand.ExecuteNonQuery();
                            bSuccess = true;
                        }
                    }
                }
                else
                {
                    localOdbcDataReader.Close();
                }                
            }
            catch (OdbcException e)
            {
                sErrorMessage = e.Message;
            }
            catch (Exception e)
            {
                sErrorMessage = e.Message;
            }
            return bSuccess;
        }
       
        private void btnOK_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void timerTransfer_Tick(object sender, EventArgs e)
        {
            timerTransfer.Enabled = false;
            const int STEP0 = 0;
            const int STEP1 = 1;
            const int STEP2 = 2;
            const int STEP3 = 3;
            const int STEP4 = 4;

            switch (iStep)
            {
                case STEP0:
                    labelStatus.Text = "Connect to Local Database";
                    if (ConnectToLocalDatabase())
                    {
                        iTotalRow = GetRowCount(OPTION_COUNT_ALL);
                        int iGood = GetRowCount(OPTION_COUNT_GOOD_ONLY);
                        int iBad = GetRowCount(OPTION_COUNT_BAD_ONLY);
                        if (iTotalRow > 0)
                        {
                            labelTotal.Text = String.Format("Total {0} (Good={1}, Bad={2})", iTotalRow, iGood, iBad);
                            labelTransfer.Text = "Transfer 0";
                            progressBarTransfer.Maximum = iTotalRow;
                            iStep = STEP1;
                            iPercentComplete = 0;
                            iLoopPercent = 1;

                            timerTransfer.Interval = 1;
                            timerTransfer.Enabled = true;
                        }
                        else
                        {
                            labelStatus.Text = "No data base transfer";
                            btnOK.ForeColor = Color.Red;
                            btnOK.Enabled = true;
                        }
                    }
                    else
                    {
                        labelStatus.Text = "Cannot connect to Local Database";
                        btnOK.Enabled = true;
                    }
                    break;

                case STEP1:
                    labelStatus.Text = "Connect to Server Database";
                    if (ConnectToServerDatabase())
                    {
                        iStep = STEP2;
                        timerTransfer.Enabled = true;
                    }
                    else
                    {
                        labelStatus.Text = "Canaot connect to Server Database";
                        btnOK.ForeColor = Color.Red;
                        btnOK.Enabled = true;
                    }
                    break;

                case STEP2:
                    labelStatus.Text = "Transfering work order " + sWorkOrder;
                    do
                    {
                        if (TransferToServer())
                        {
                            iRow++;
                            iPercentComplete = iRow * 1000 / iTotalRow; // XX.X
                            progressBarTransfer.PerformStep();
                            labelTransfer.Text = String.Format("Transfer {0} ", iRow);
                            if (iRow == iTotalRow)
                            {
                                iStep = STEP3;
                                timerTransfer.Enabled = true;
                                return;
                            }
                        }
                        else
                        {
                            labelStatus.Text = "Error during transfer";
                            MessageBox.Show(sErrorMessage, "Error");
                            localOdbcConnection.Close();
                            serverOdbcConnection.Close();
                            btnOK.ForeColor = Color.Red;
                            btnOK.Enabled = true;
                            return;
                        }
                    } while (iPercentComplete < iLoopPercent && iRow < iTotalRow);

                    iLoopPercent++;
                    labelPercentComplete.Text = String.Format("Complete {0:F1} %", iPercentComplete / 10.0);
                    timerTransfer.Enabled = true;
                    break;

                case STEP3:
                    labelPercentComplete.Text = "Complete 100 %";
                    localOdbcConnection.Close();
                    serverOdbcConnection.Close();
                    timerTransfer.Interval = 500;
                    iStep = STEP4;
                    timerTransfer.Enabled = true;
                    break;

                case STEP4:
                    if (iRow > 0)
                    {
                        labelPercentComplete.Text = "Total " + iRow + " rows transfer already";
                    }
                    labelStatus.Text = "Finish transfer database";
                    bTransferSuccess = true;
                    btnOK.Enabled = true;
                    this.Close();
                    break;
            }
        }
    }
}
