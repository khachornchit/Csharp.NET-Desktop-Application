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
    public partial class FormWorkOrder : Form
    {
        const string QA_WORK_ORDER = "0";
        ProductionParameters _prodParam;
        Registration registration;
        ProductionVariable _prodVar;
        bool bWorkOrderSelected;        
        int iUserID;

        public ProductionVariable prodVar
        {
            get { return _prodVar; }
        }

        public ProductionParameters prodParam
        {
            get { return _prodParam; }
        }

        public FormWorkOrder(int iUserID, Registration registration)
        {
            this.iUserID = iUserID;
            this.registration = registration;
            InitializeComponent();
        }

        private void FormWorkOrder_Load(object sender, EventArgs e)
        {
            btnChangBoxNumber.Visible = btnTransferDatabase.Visible = iUserID >= Login.ROLE_ENGINER;
            btnAddWorkOrder.Enabled = btnDeleteWorkOrder.Enabled = btnGetWorkOrder.Enabled = btnSelectWorkOrder.Enabled = iUserID >= Login.ROLE_WORK_ORDER && iUserID != Login.USER_ID_QA;
            bWorkOrderSelected = false;
        }

        private void FormWorkOrder_Shown(object sender, EventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;
            timerInitial.Enabled = true;
        }

        private void timerInitial_Tick(object sender, EventArgs e)
        {
            timerInitial.Enabled = false;
            registration.RetrieveWorkOrderTable(dataGridViewWorkOrder, chkHideFinishedWorkOrder.Checked, iUserID);
            UpdateWorkOrderList();
            registration.RetrieveWorkOrderTable(dataGridViewWorkOrder, chkHideFinishedWorkOrder.Checked, iUserID);
            this.Cursor = Cursors.Arrow;
        }

        private void FormWorkOrder_FormClosed(object sender, FormClosedEventArgs e)
        {
        }

        public bool WorkOrderSelected
        {
            get { return bWorkOrderSelected; }
        }

        private void txtWorkOrder_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar);
        }

        private void btnGetWorkOrder_Click(object sender, EventArgs e)
        {
            if (iUserID == Login.USER_ID_QA)
            {
                return;
            }
            btnGetWorkOrder.Cursor = Cursors.WaitCursor;
            if (txtWorkOrder.Text.Length == FormMain.WORKORDER_DIGIT)
            {
                if (GetSAP_Data(txtWorkOrder.Text))
                {
                    btnAddWorkOrder.Enabled = true;
                }
            }
            btnGetWorkOrder.Cursor = Cursors.Arrow;
        }

        private void btnAddWorkOrder_Click(object sender, EventArgs e)
        {
            if(iUserID == Login.USER_ID_QA)
            {
                return;
            }
            string sWorkOrder = txtWorkOrder.Text;
            int iRetVal = registration.WorkOrderListExitsInServerDatabase(sWorkOrder);
            if (iRetVal == 0)
            {
                string[] asArticleInfo = registration.GetArticleInformationFormServerDatabase(labelArticleNumber.Text);
                if (asArticleInfo != null)
                {
                    if (asArticleInfo[6] == "True")
                    {
                        int iTagInLocalDataBase = registration.CountTagInLocalDatabase(sWorkOrder);
                        if (iTagInLocalDataBase >= 0)
                        {
                            int iTagInServerDataBase = registration.CountTagInServerDatabase(sWorkOrder);
                            if (iTagInServerDataBase >= 0)
                            {
                                int iTotalTagInWorkOrder = iTagInLocalDataBase + iTagInServerDataBase;
                                int iBagUIDInServerDataBase = registration.CountBadUIDinServerDatabase(sWorkOrder);
                                if (registration.AddWorkOrderToWorkOrderTable(sWorkOrder, labelArticleNumber.Text, labelProductName.Text, labelWorkOrderTarget.Text, iTotalTagInWorkOrder, iTagInServerDataBase, iBagUIDInServerDataBase, 0, Registration.ParseStringToWorkOrderStatus(labelWorkOrderStatus.Text)))
                                {
                                    registration.RetrieveWorkOrderTable(dataGridViewWorkOrder, chkHideFinishedWorkOrder.Checked, iUserID);
                                    ClearWorkOrderField();
                                    MessageBox.Show("Add new workorder is successful.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                }
                                else
                                {
                                    MessageBox.Show("Cannot add workorder.\n" + registration.ErrorMessage, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                }
                            }
                            else
                            {
                                MessageBox.Show("Cannot update work order " + txtWorkOrder.Text + ".\n" + registration.ErrorMessage, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                        else
                        {
                            MessageBox.Show("Cannot update work order " + txtWorkOrder.Text + ".\n" + registration.ErrorMessage, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    else
                    {
                        int iTagInLocalDataBase = registration.CountTestingTagInLocalDatabase(sWorkOrder);
                        if (iTagInLocalDataBase >= 0)
                        {
                            int iTagInServerDataBase = registration.CountTestingTagInServerDatabase(sWorkOrder);
                            if (iTagInServerDataBase >= 0)
                            {
                                int iTotalTagInWorkOrder = iTagInLocalDataBase + iTagInServerDataBase;
                                if (registration.AddWorkOrderToWorkOrderTable(sWorkOrder, labelArticleNumber.Text, labelProductName.Text, labelWorkOrderTarget.Text, iTotalTagInWorkOrder, iTagInServerDataBase, 0, 0, Registration.ParseStringToWorkOrderStatus(labelWorkOrderStatus.Text)))
                                {
                                    registration.RetrieveWorkOrderTable(dataGridViewWorkOrder, chkHideFinishedWorkOrder.Checked, iUserID);
                                    ClearWorkOrderField();
                                    MessageBox.Show("Add new workorder is successful.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                }
                                else
                                {
                                    MessageBox.Show("Cannot add workorder.\n" + registration.ErrorMessage, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                }
                            }
                            else
                            {
                                MessageBox.Show("Cannot update work order " + txtWorkOrder.Text + ".\n" + registration.ErrorMessage, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                        else
                        {
                            MessageBox.Show("Cannot update work order " + txtWorkOrder.Text + ".\n" + registration.ErrorMessage, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
            else if (iRetVal > 0)
            {
                MessageBox.Show("Duplicate workorder.", "Duplicate", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else if (iRetVal < 0)
            {
                MessageBox.Show("Cannot add workorder.\n" + registration.ErrorMessage, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnDeleteWorkOrder_Click(object sender, EventArgs e)
        {
            if (iUserID == Login.USER_ID_QA)
            {
                return;
            }
            if (dataGridViewWorkOrder.RowCount > 0)
            {
                string sWorkOrder = dataGridViewWorkOrder.Rows[dataGridViewWorkOrder.CurrentCell.RowIndex].Cells[0].Value.ToString();
                if (MessageBox.Show("Do you want to delete work order " + sWorkOrder + "?", "Are you sure?", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    Log.SaveMessage(sWorkOrder, "Do you want to delete work order " + sWorkOrder + "?, YES");
                    if (registration.DeleteWorkOrderFromServerDatabase(sWorkOrder))
                    {
                        dataGridViewWorkOrder.Rows.RemoveAt(dataGridViewWorkOrder.CurrentCell.RowIndex);
                        ClearWorkOrderField();
                        MessageBox.Show("Delete work order " + sWorkOrder + " is successful.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        Log.SaveMessage(sWorkOrder, "Cannot delete workorder " + sWorkOrder + "," + registration.ErrorMessage + ",Call ENGINEER.");
                        MessageBox.Show("Cannot delete workorder " + sWorkOrder + "\n" + registration.ErrorMessage + "\nCall ENGINEER.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void btnSelectWorkOrder_Click(object sender, EventArgs e)
        {
            bWorkOrderSelected = false;
            if (dataGridViewWorkOrder.RowCount > 0)
            {
                string sWorkOrder = txtWorkOrder.Text;
                if(iUserID != Login.USER_ID_QA)
                {
                    if (sWorkOrder.Length < FormMain.WORKORDER_DIGIT)
                    {
                        if (sWorkOrder.Length == 0)
                        {
                            MessageBox.Show("Please select work order.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                        else if (sWorkOrder.Length == 1)
                        {
                            MessageBox.Show("This is work order for QA.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                    }
                    else
                    {
                        int iRetVal = registration.WorkOrderListExitsInServerDatabase(sWorkOrder);
                        if (iRetVal == 1)
                        {
                            if (GetWorkOrderData(sWorkOrder))
                            {
                                if (registration.UpdateQuantityWorkedInWorkOrderTable(sWorkOrder, _prodVar.iQuantityWorked))
                                {
                                    bWorkOrderSelected = true;
                                }
                                this.Close();
                            }
                        }
                        else if (iRetVal == 0)
                        {
                            MessageBox.Show("Work order " + sWorkOrder + " is not in work order list database.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                    }
                }
                else
                {
                    if (sWorkOrder != QA_WORK_ORDER)
                    {
                        MessageBox.Show("This work order cannot use for QA initialize.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                    else
                    {
                        int iRetVal = registration.WorkOrderListExitsInServerDatabase(sWorkOrder);
                        if (iRetVal == 1)
                        {
                            if (GetWorkOrderData(sWorkOrder))
                            {
                                bWorkOrderSelected = true;
                                this.Close();
                            }
                        }
                        else if (iRetVal == 0)
                        {
                            MessageBox.Show("Work order " + sWorkOrder + " is not in work order list database.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                    }
                }
            }
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            registration.RetrieveWorkOrderTable(dataGridViewWorkOrder, chkHideFinishedWorkOrder.Checked, iUserID);
            SapInformation sapInfo;
            string sWorkOrderStatus, sWorkOrder;
            for (int iRow = 0; iRow < dataGridViewWorkOrder.Rows.Count; iRow++)
            {
                sWorkOrderStatus = dataGridViewWorkOrder.Rows[iRow].Cells["dgvWorkOrderStatus"].Value.ToString();
                sWorkOrder = dataGridViewWorkOrder.Rows[iRow].Cells["dgvWorkOrder"].Value.ToString();
                if (sWorkOrderStatus != "Finished" || sWorkOrder == txtWorkOrder.Text)
                {
                    sapInfo = registration.GetDataFromSAP(sWorkOrder);
                    if (sapInfo != null)
                    {
                        registration.UpdateWorkOrderStatusInWorkOrderTable(sWorkOrder, sapInfo.sWorkOrderStatus);
                        if (dataGridViewWorkOrder.Rows[iRow].Cells["dgvRegister"].Value.ToString() == "Register")
                        {
                            int iTagInLocalDataBase = registration.CountTagInLocalDatabase(sWorkOrder);
                            if (iTagInLocalDataBase >= 0)
                            {
                                int iTagInServerDataBase = registration.CountTagInServerDatabase(sWorkOrder);
                                if (iTagInServerDataBase >= 0)
                                {
                                    int iTotalTagInWorkOrder = iTagInLocalDataBase + iTagInServerDataBase;

                                    registration.UpdateQuantityWorkedInWorkOrderTable(sWorkOrder, iTotalTagInWorkOrder);
                                }
                                else
                                {
                                    MessageBox.Show("Cannot update work order " + sWorkOrder + ".\n" + registration.ErrorMessage, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                }
                            }
                            else
                            {
                                MessageBox.Show("Cannot update work order " + sWorkOrder + ".\n" + registration.ErrorMessage, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                        else
                        {
                            int iGoodTestInLocalDataBase = registration.CountTestingTagInLocalDatabase(sWorkOrder);
                            if (iGoodTestInLocalDataBase >= 0)
                            {
                                int iGoodTestInServerDataBase = registration.CountTestingTagInServerDatabase(sWorkOrder);
                                if (iGoodTestInServerDataBase >= 0)
                                {
                                    int iTotalTagInWorkOrder = iGoodTestInLocalDataBase + iGoodTestInServerDataBase;
                                    registration.UpdateQuantityWorkedInWorkOrderTable(sWorkOrder, iTotalTagInWorkOrder);
                                }
                                else
                                {
                                    MessageBox.Show("Cannot update work order " + sWorkOrder + ".\n" + registration.ErrorMessage, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                }
                            }
                            else
                            {
                                MessageBox.Show("Cannot update work order " + sWorkOrder + ".\n" + registration.ErrorMessage, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                        sapInfo = null;
                    }
                }
            }
            registration.RetrieveWorkOrderTable(dataGridViewWorkOrder, chkHideFinishedWorkOrder.Checked, iUserID);
        }

        private void btnChangBoxNumber_Click(object sender, EventArgs e)
        {
            FormChangeBoxNumber formChangeBoxNumber = new FormChangeBoxNumber();
            formChangeBoxNumber.ShowDialog();
        }

        private void btnTransferDatabase_Click(object sender, EventArgs e)
        {
            if (dataGridViewWorkOrder.RowCount > 0)
            {
                string sWorkOrder = dataGridViewWorkOrder.Rows[dataGridViewWorkOrder.CurrentCell.RowIndex].Cells[0].Value.ToString();
                if (sWorkOrder.Length < FormMain.WORKORDER_DIGIT)
                {
                    MessageBox.Show("Please select work order.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else
                {
                    if (MessageBox.Show("Do you want to transfer work order " + sWorkOrder + " data to server database?", "Are you sure?", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        registration.TransferDatabaseToServer(sWorkOrder);
                    }
                }
            }
        }

        private void chkHideFinishedWorkOrder_Click(object sender, EventArgs e)
        {
            dataGridViewWorkOrder.Rows.Clear();
            registration.RetrieveWorkOrderTable(dataGridViewWorkOrder, chkHideFinishedWorkOrder.Checked, iUserID);
        }

        private void dataGridViewWorkOrder_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex > -1)
            {
                ClearWorkOrderField();
                btnSelectWorkOrder.Enabled = iUserID >= Login.ROLE_WORK_ORDER;
                btnDeleteWorkOrder.Enabled = iUserID >= Login.ROLE_WORK_ORDER && iUserID != Login.USER_ID_QA;
                txtWorkOrder.Text = dataGridViewWorkOrder.Rows[e.RowIndex].Cells["dgvWorkOrder"].Value.ToString();
                labelProductName.Text = dataGridViewWorkOrder.Rows[e.RowIndex].Cells["dgvProductName"].Value.ToString();
                labelArticleNumber.Text = dataGridViewWorkOrder.Rows[e.RowIndex].Cells["dgvArticle"].Value.ToString();
                txtQuantityPerBox.Text = dataGridViewWorkOrder.Rows[e.RowIndex].Cells["dgvQuantityPerBox"].Value.ToString();
                labelWorkOrderTarget.Text = dataGridViewWorkOrder.Rows[e.RowIndex].Cells["dgvWorkOrderTarget"].Value.ToString();
                labelWorkOrderStatus.Text = dataGridViewWorkOrder.Rows[e.RowIndex].Cells["dgvWorkOrderStatus"].Value.ToString();
            }
        }

        private void ClearWorkOrderField()
        {
            txtWorkOrder.Clear();
            labelArticleNumber.Text = String.Empty;
            labelProductName.Text = String.Empty;
            labelWorkOrderStatus.Text = String.Empty;
            labelWorkOrderTarget.Text = String.Empty;
            txtQuantityPerBox.Text = String.Empty;
            btnAddWorkOrder.Enabled = false;
            btnDeleteWorkOrder.Enabled = false;
        }

        private bool GetWorkOrderData(string sWorkOrder)
        {
            bool bSuccess = false;
            _prodParam = registration.GetProductionParamFromServerDatabase(sWorkOrder);
            if (_prodParam != null)
            {
                if (!_prodParam.bPartialBoxClosed)//&& Convert.ToInt32(asDataForProduction[3]) > Convert.ToInt32(asDataForProduction[4]))
                {
                    if (_prodParam.bRegister)// Register UID
                    {
                        string sLastUID = FindLastUID(_prodParam.sBeginningUID, _prodParam.sEndingUID);

                        if (sLastUID != null)
                        {
                            string sUID = FindNextUID(_prodParam.sBeginningUID, sLastUID);
                            if (sUID != null)
                            {
                                int iTagInServer = registration.CountTagInServerDatabase(sWorkOrder);
                                if (iTagInServer >= 0)
                                {
                                    int iTagInBox = registration.CountTagInLocalDatabase(sWorkOrder);
                                    if (iTagInBox >= 0)
                                    {
                                        _prodVar.iQuantityWorked = iTagInServer + iTagInBox;
                                        if (IsUID_Enough(sWorkOrder, sUID, _prodParam.sEndingUID, _prodParam.iWorkOrderTarget - _prodVar.iQuantityWorked))
                                        {
                                            int iBoxNumber;
                                            if (iTagInBox > 0)
                                            {
                                                iBoxNumber = registration.GetLastBoxNumberFromLocalDatabase();
                                            }
                                            else
                                            {
                                                iBoxNumber = registration.GetLastBoxNumberFromServerDatabase();
                                                if (iBoxNumber > 0)
                                                {
                                                    iBoxNumber += 1;
                                                }
                                            }

                                            if (iBoxNumber >= 0)
                                            {
                                                _prodVar.iQuantityInBox = iTagInBox;
                                                _prodVar.iBoxNumber = iBoxNumber;
                                                _prodParam.sUID = sUID;
                                                bSuccess = true;
                                            }
                                            else
                                            {
                                                MessageBox.Show("Cannot get last box nuber form database.\n" + registration.ErrorMessage, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                            }
                                        }
                                        else
                                        {
                                            MessageBox.Show("Tag UID is not enough", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                        }
                                    }
                                    else
                                    {
                                        MessageBox.Show("Cannot check current box in local database.\n" + registration.ErrorMessage, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        int iTagInBox = registration.CountTestingTagInLocalDatabase(sWorkOrder);
                        if (iTagInBox >= 0)
                        {
                            int iBoxNumber;
                            if (iTagInBox > 0)
                            {
                                iBoxNumber = registration.GetLastBoxNumberFromLocalDatabse(sWorkOrder);
                            }
                            else
                            {
                                iBoxNumber = registration.GetLastBoxNumberFromServerDatabase(sWorkOrder);
                                if (iBoxNumber > 0)
                                {
                                    iBoxNumber += 1;
                                }
                            }

                            if (iBoxNumber >= 0)
                            {
                                int iTagInServer = registration.CountTestingTagInServerDatabase(sWorkOrder);
                                if (iTagInServer >= 0)
                                {
                                    _prodVar.iQuantityWorked = iTagInServer + iTagInBox;
                                    _prodVar.iQuantityInBox = iTagInBox;
                                    _prodVar.iBoxNumber = iBoxNumber;
                                    bSuccess = true;
                                }
                            }
                            else
                            {
                                MessageBox.Show("Cannot get last box nuber form database.\n" + registration.ErrorMessage, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                        else
                        {
                            MessageBox.Show("Cannot check current box in local database.\n" + registration.ErrorMessage, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
                else
                {
                    MessageBox.Show(String.Format("Work order {0} closed.\nTarget {1} >= {2}.", sWorkOrder, _prodParam.iWorkOrderTarget, _prodVar.iQuantityWorked, "Stop", MessageBoxButtons.OK, MessageBoxIcon.Stop));
                }
            }
            else if (registration.ErrorMessage != null)
            {
                MessageBox.Show("Cannot find workorder " + sWorkOrder + ".\n" + registration.ErrorMessage, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return bSuccess;
        }

        private string FindLastUID(string sBeginningUID, string sEndingUID)
        {
            string sLastUID = null;
            int iRetVal = 0;

            iRetVal = registration.UID_ExistInLocalDatabase(sEndingUID);
            if (iRetVal >= 1)
            {
                MessageBox.Show("Last UID from local database is " + sEndingUID + ".\nEnding UID is " + sEndingUID + ".\nUID is full.\nCall ENGINEER.", "UID is full", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                return null;
            }
            else if (iRetVal == 0)
            {
                iRetVal = registration.UID_ExistInServerDatabase(sEndingUID);
                if (iRetVal >= 1)
                {
                    MessageBox.Show("Last UID from local database is " + sEndingUID + ".\nEnding UID is " + sEndingUID + ".\nUID is full.\nCall ENGINEER.", "UID is full", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    return null;
                }
                else if (iRetVal < 0)
                {
                    MessageBox.Show("Cannot find last UID in server datbase.\n" + registration.ErrorMessage + "\nCall ENGINEER.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return null;
                }
            }
            else if (iRetVal < 0)
            {
                MessageBox.Show("Cannot find last UID in local datbase.\n" + registration.ErrorMessage + "\nCall ENGINEER.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }


            string sLastUID_Local = registration.FindLastUIDinLocalDatabase(sBeginningUID, sEndingUID);
            if (registration.ErrorMessage != null)
            {
                MessageBox.Show("Cannot find last UID from server database.\nServer dabase error.\n" + registration.ErrorMessage + "\nCall ENGINEER.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }

            string sLastUID_Server = registration.FindLastUIDinServerDatabase(sBeginningUID, sEndingUID);
            if (registration.ErrorMessage != null)
            {
                MessageBox.Show("Cannot find last UID from local database.\nLocal dabase error.\n" + registration.ErrorMessage + "\nCall ENGINEER.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }

            sLastUID = String.Empty;

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

            return sLastUID;
        }

        private string FindNextUID(string sBeginningUID, string sLastUID)
        {
            string sUID = null;
            if (sLastUID == String.Empty)
            {
                int iRetVal = registration.UID_ExistInLocalDatabase(sBeginningUID);
                if (iRetVal == 0)
                {
                    iRetVal = registration.UID_ExistInServerDatabase(sBeginningUID);
                    if (iRetVal == 0)
                    {
                        sUID = sBeginningUID;
                    }
                }
            }
            else
            {
                sUID = Utility.IncUID(sLastUID);
            }
            return sUID;
        }

        private bool IsUID_Enough(string sWorkOrder, string sUID, string sEndingUID, int iProductionTarget)
        {
            bool bEnough = false;
            long lEndingUID = Convert.ToInt64(sEndingUID.Substring(sEndingUID.Length - 12, 12));
            long lUID = Convert.ToInt64(sUID.Substring(sUID.Length - 12, 12));

            int iTagInServerDataBase = registration.CountTagInServerDatabase(sWorkOrder);
            if (iTagInServerDataBase > -1)
            {
                int iTagInLocalDataBase = registration.CountTagInLocalDatabase(sWorkOrder);
                if (iTagInLocalDataBase > -1)
                {
                    int iBadUIDinServerDataBase = registration.CountBadUIDinServerDatabase(sWorkOrder);
                    int iBadUIDinLocalDataBase = registration.CountBadUIDinLocalDatabase(sWorkOrder);
                    if (registration.UpdateQuantityWorkedInWorkOrderTable(sWorkOrder, iTagInServerDataBase + iTagInLocalDataBase))
                    {
                        if (registration.UpdateDatabaseTrsnsferInWorkOrderTable(sWorkOrder, iTagInServerDataBase, iBadUIDinServerDataBase))
                        {
                            int iTotalUIDused = iTagInServerDataBase + iTagInLocalDataBase + iBadUIDinServerDataBase + iBadUIDinLocalDataBase;
                            bEnough = (lEndingUID - lUID + 1 - iTotalUIDused) >= iProductionTarget - iTagInServerDataBase - iTagInLocalDataBase;
                            if (!bEnough)
                            {
                                MessageBox.Show(String.Format("UID is not enough for production {0}.", iProductionTarget - iTagInServerDataBase - iTagInLocalDataBase), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                        else
                        {
                            MessageBox.Show("Cannot update database trnasfer in server database.\n" + registration.ErrorMessage, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    else
                    {
                        MessageBox.Show("Cannot update quantity worked in server database.\n" + registration.ErrorMessage, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    MessageBox.Show("Cannot check quantity worked in local database.\n" + registration.ErrorMessage, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("Cannot check quantity worked in server database.\n" + registration.ErrorMessage, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return bEnough;
        }

        private bool GetSAP_Data(string sWorkOrder)
        {
            bool bSuccess = false;
            SapInformation sapInfo = registration.GetDataFromSAP(sWorkOrder);
            if (sapInfo != null)
            {
                if (sapInfo.sArticleNumber != null) 
                {
                    string[] asArticleInfo = registration.GetArticleInformationFormServerDatabase(sapInfo.sArticleNumber);
                    if (asArticleInfo != null)
                    {
                        labelProductName.Text = sapInfo.sProductName;
                        labelArticleNumber.Text = sapInfo.sArticleNumber;
                        labelWorkOrderStatus.Text = Registration.ParseWorkOrderStatusToString(sapInfo.sWorkOrderStatus);
                        labelWorkOrderTarget.Text = sapInfo.sWorkOrderTarget;
                        txtQuantityPerBox.Text = asArticleInfo[5];
                        btnAddWorkOrder.Enabled = true;
                        asArticleInfo = null;
                        bSuccess = true;
                    }
                    else
                    {
                        MessageBox.Show("Found work order " + sWorkOrder + " in SAP database.\nCan not find article number " + sapInfo.sArticleNumber + " in product type database.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    MessageBox.Show("Can not find work order  " + sWorkOrder + " in SAP database.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                sapInfo = null;
            }
            else
            {
                if (registration.ErrorMessage == null)
                {
                    MessageBox.Show("No work order " + sWorkOrder + " in SAP database.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    MessageBox.Show("SAP Error.\n" + registration.ErrorMessage + "\nCall ENGINEER.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            return bSuccess;
        }

        private void UpdateWorkOrderList()
        {
            string sWorkOrder;
            SapInformation sapInfo;
            int iTagInServer, iTagInLocal, iBadTagInSever, iBadTagInLocal;
            for (int iRow = 0; iRow < dataGridViewWorkOrder.RowCount; iRow++)
            {
                if (dataGridViewWorkOrder.Rows[iRow].Cells["dgvWorkOrderStatus"].Value.ToString() != "Finished")
                {
                    sWorkOrder = dataGridViewWorkOrder.Rows[iRow].Cells["dgvWorkOrder"].Value.ToString();
                    sapInfo = registration.GetDataFromSAP(sWorkOrder);
                    if (sapInfo != null)
                    {
                        if (registration.UpdateWorkOrderStatusInWorkOrderTable(sWorkOrder, sapInfo.sWorkOrderStatus))
                        {
                            if (dataGridViewWorkOrder.Rows[iRow].Cells["dgvRegister"].Value.ToString() == "Register")
                            {
                                iTagInServer = registration.CountTagInServerDatabase(sWorkOrder);
                                if (iTagInServer >= 0)
                                {
                                    iTagInLocal = registration.CountTagInLocalDatabase(sWorkOrder);
                                    if (iTagInLocal >= 0)
                                    {
                                        if (!registration.UpdateQuantityWorkedInWorkOrderTable(sWorkOrder, iTagInServer + iTagInLocal))
                                        {
                                            MessageBox.Show("Cannot update quantity worked in server database.\n" + registration.ErrorMessage, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                        }
                                        else
                                        {
                                            iBadTagInSever = registration.CountBadUIDinServerDatabase(sWorkOrder);
                                            if (iBadTagInSever >= 0)
                                            {
                                                iBadTagInLocal = registration.CountBadUIDinLocalDatabase(sWorkOrder);
                                                if (iBadTagInLocal >= 0)
                                                {
                                                    if (!registration.UpdateDatabaseTrsnsferInWorkOrderTable(sWorkOrder, iTagInServer + iTagInLocal, iBadTagInSever + iBadTagInLocal))
                                                    {
                                                        MessageBox.Show("Cannot update Bad UID quantity in server database.\n" + registration.ErrorMessage, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        MessageBox.Show("Cannot get quantity worked in local database.\n" + registration.ErrorMessage, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    }
                                }
                                else
                                {
                                    MessageBox.Show("Cannot get quantity worked in server database.\n" + registration.ErrorMessage, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                }
                            }
                            else
                            {
                                iTagInServer = registration.CountTestingTagInServerDatabase(sWorkOrder);
                                if (iTagInServer >= 0)
                                {
                                    iTagInLocal = registration.CountTestingTagInLocalDatabase(sWorkOrder);
                                    if (iTagInLocal >= 0)
                                    {
                                        if (!registration.UpdateQuantityWorkedInWorkOrderTable(sWorkOrder, iTagInServer + iTagInLocal))
                                        {
                                            MessageBox.Show("Cannot update quantity worked in server database.\n" + registration.ErrorMessage, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                        }
                                    }
                                    else
                                    {
                                        MessageBox.Show("Cannot get quantity worked in local database.\n" + registration.ErrorMessage, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    }
                                }
                                else
                                {
                                    MessageBox.Show("Cannot get quantity worked in server database.\n" + registration.ErrorMessage, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                }
                            }
                        }
                        else
                        {
                            MessageBox.Show("Cannot update work order status in server database.\n" + registration.ErrorMessage, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        sapInfo = null;
                    }
                    else
                    {
                        if (registration.ErrorMessage != null)
                        {
                            MessageBox.Show("Cannot get work order status from SAP database.\n" + registration.ErrorMessage, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        else
                        {
                            MessageBox.Show("No work order " + sWorkOrder + " in SAP database.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
        }
    }
}
