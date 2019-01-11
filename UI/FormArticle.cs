using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Testers.TagMeter;
using Registrations;
using Utilities;

namespace Machine
{
    public partial class FormArticle : Form
    {
        public const int UID_DIGIT = FormMain.UID_DIGIT;
        public const int WORKORDER_DIGIT = FormMain.WORKORDER_DIGIT;
        public const int ARTICLE_DIGIT = FormMain.ARTICLE_DIGIT;

        Registration registration;

        public FormArticle(Registration registration)
        {
            this.registration = registration;
            InitializeComponent();
        }

        private void FormArticle_Load(object sender, EventArgs e)
        {
            registration.RetrieveArticleTable(dataGridViewArticle);
            registration.RetrieveTagMeterParameterTable(dataGridViewTagMeter);
        }

        private void FormArticle_Shown(object sender, EventArgs e)
        {
        }

        private void FormArticle_FormClosed(object sender, FormClosedEventArgs e)
        {
        }

        private void txtQuantityPerBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar);
        }

        private void txtBeginningUID_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar);
        }

        private void txtEndingUID_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar);
        }

        private void txtTagProgrammerTrimingFrequency_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar);
            //if (e.Handled)
            //{
            //    if (e.KeyChar == '.')
            //    {
            //        e.Handled = false;
            //    }
            //}
        }

        private void txtTagProgrammerPowerLevel_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar);
        }

        private void btnAddArticle_Click(object sender, EventArgs e)
        {
            if (IsArticleDataOK())
            {
                if (registration.AddNewArticleToArticleTable(                    
                    txtArticleNumber.Text,
                    cmbProductType.Text,//Items[cmbProductType.SelectedIndex].ToString(),
                    txtCustomerName.Text,
                    txtLabelName.Text,
                    cmbLabelLayout.SelectedIndex,
                    txtQuantityPerBox.Text,
                    cmbRegister.SelectedIndex == 1,
                    cmbTagType.SelectedIndex,
                    cmbTagMeter.SelectedIndex == 1,
                    cmbTesting.SelectedIndex == 1,
                    cmbTagProgrammer.SelectedIndex == 1,
                    cmbProgramOption.SelectedIndex,                    
                    txtTagProgrammerPowerLevel.Text,
                    txtTagProgrammerTrimingFrequency.Text,
                    txtBeginningUID.Text,
                    txtEndingUID.Text
                    ))
                {
                    registration.RetrieveArticleTable(dataGridViewArticle);
                    for (int iRow = 0; iRow < dataGridViewArticle.Rows.Count; iRow++)
                    {
                        if (dataGridViewArticle.Rows[iRow].Cells[0].Value.ToString() == txtArticleNumber.Text)
                        {
                            dataGridViewArticle.CurrentCell = dataGridViewArticle.Rows[iRow].Cells[0];
                            break;
                        }
                    }
                    MessageBox.Show("Add new article number is successful.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    ClearArticleField();
                }
                else
                {
                    MessageBox.Show("Cannot add article number.\n" + registration.ErrorMessage, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnUpdateArticle_Click(object sender, EventArgs e)
        {
            if (IsArticleDataOK())
            {
                int iRetVal = registration.ArticleExistInServerDatabase(txtArticleNumber.Text);
                if (iRetVal == 0)
                {
                    MessageBox.Show("Cannot find article number " + txtArticleNumber.Text, "Unknow", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                else if (iRetVal < 0)
                {
                    MessageBox.Show("Cannot add article number.\n" + registration.ErrorMessage, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                    if (registration.UpdateArticleInArticleTable(
                    txtArticleNumber.Text,
                    cmbProductType.Text, //cmbProductType.Items[cmbProductType.SelectedIndex].ToString(),
                    txtCustomerName.Text,
                    txtLabelName.Text,
                    cmbLabelLayout.SelectedIndex,
                    txtQuantityPerBox.Text,
                    cmbRegister.SelectedIndex == 1,
                    cmbTagType.SelectedIndex,
                    cmbTagMeter.SelectedIndex == 1,
                    cmbTesting.SelectedIndex == 1,
                    cmbTagProgrammer.SelectedIndex == 1,
                    cmbProgramOption.SelectedIndex,
                    txtTagProgrammerPowerLevel.Text,
                    txtTagProgrammerTrimingFrequency.Text,
                    txtBeginningUID.Text,
                    txtEndingUID.Text
                    ))
                {
                    registration.RetrieveArticleTable(dataGridViewArticle);
                    for (int iRow = 0; iRow < dataGridViewArticle.Rows.Count; iRow++)
                    {
                        if (dataGridViewArticle.Rows[iRow].Cells[0].Value.ToString() == txtArticleNumber.Text)
                        {
                            dataGridViewArticle.CurrentCell = dataGridViewArticle.Rows[iRow].Cells[0];
                            break;
                        }
                    }
                    MessageBox.Show("Update article number is successful.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    ClearArticleField();
                }
                else
                {
                    MessageBox.Show("Cannot add article number.\n" + registration.ErrorMessage, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnDeleteArticle_Click(object sender, EventArgs e)
        {
            string sArticleNumber;
            if (txtArticleNumber.Text.Length == 0)
            {
                sArticleNumber = dataGridViewArticle.Rows[dataGridViewArticle.Rows.Count - 1].Cells[0].Value.ToString();
            }
            else
            {
                sArticleNumber = txtArticleNumber.Text;
            }

            if (sArticleNumber.Length < ARTICLE_DIGIT)
            {
                if (sArticleNumber.Length == ARTICLE_DIGIT - 1)
                {
                    sArticleNumber = txtArticleNumber.Text.Substring(0, 3) + " " + txtArticleNumber.Text.Substring(3, txtArticleNumber.Text.Length - 3);
                    txtArticleNumber.Text = sArticleNumber;
                }
                else
                {
                    MessageBox.Show("Please enter 12 digit article number.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }

            if (registration.IsArticleBeingUsedInWorkOrderList(sArticleNumber) == 0)
            {
                if (registration.DeleteArticleNumberFromServerDatabase(sArticleNumber))
                {
                    dataGridViewArticle.Rows.RemoveAt(dataGridViewArticle.CurrentCell.RowIndex);
                    MessageBox.Show("Delete aticle number " + sArticleNumber + " is successful.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    ClearArticleField();
                }
                else
                {
                    MessageBox.Show("Cannot delete article number " + sArticleNumber + "\n" + registration.ErrorMessage, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("Cannot delete.\nThis article number is being used in work order list", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnArticleTagMeterAdd_Click(object sender, EventArgs e)
        {
            string sArticleNumber = txtTagMeterArticle.Text;
            if (sArticleNumber.Length == ARTICLE_DIGIT)
            {
                int iRetVal = registration.TagMeterArticleExitsInTagMeterParameterTable(sArticleNumber);
                if (iRetVal == 0)
                {
                    if (registration.AddTagMeterParameterToServerDatabase(sArticleNumber))
                    {
                        if (registration.RetrieveTagMeterParameterTable(dataGridViewTagMeter))
                        {
                            MessageBox.Show("Add new Tag Meter Article number is successful.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                }
                else if(iRetVal > 0)
                {
                    MessageBox.Show("Duplicate Atricle Number.", "Stop", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                }
                else
                {
                    MessageBox.Show("Database error.\n" + registration.ErrorMessage, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnArticleTagMeterParameterFile_Click(object sender, EventArgs e)
        {
            string sArticleNumber = txtTagMeterArticle.Text;
            if (sArticleNumber.Length == ARTICLE_DIGIT)
            {
                int iRetVal = registration.TagMeterArticleExitsInTagMeterParameterTable(sArticleNumber);
                if (iRetVal == 1)
                {
                    OpenFileDialog openFileDialog = new OpenFileDialog();
                    openFileDialog.Filter = "tpr files (*.tpr)|*.tpr|All files (*.*)|*.*";
                    openFileDialog.FilterIndex = 1;
                    openFileDialog.RestoreDirectory = true;

                    if (openFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        if (registration.UpdateTagMeterParameterInTagMeterParameterTable(txtTagMeterArticle.Text, registration.ImportTagMeterParameter(openFileDialog.FileName)))//formTagMeterParameter.Parameter))
                        {
                            if (registration.RetrieveTagMeterParameterTable(dataGridViewTagMeter))
                            {
                                MessageBox.Show("Import tag meter parameter is successful", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                txtTagMeterArticle.Clear();
                            }
                        }
                    }
                }
                else if (iRetVal == 0)
                {
                    MessageBox.Show("Article number " + sArticleNumber + " is not in server database.", "Stop", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                }
            }
        }

        private void btnEditTagMeterParameter_Click(object sender, EventArgs e)
        {
            string sArticleNumber = txtTagMeterArticle.Text;
            if (sArticleNumber.Length == ARTICLE_DIGIT)
            {
                int iRetVal = registration.TagMeterArticleExitsInTagMeterParameterTable(sArticleNumber);
                if (iRetVal == 1)
                {
                    FormTagMeterParameter formTagMeterParameter = new FormTagMeterParameter(dataGridViewTagMeter);
                    formTagMeterParameter.ShowDialog();
                    if (formTagMeterParameter.Save)
                    {
                        if (registration.UpdateTagMeterParameterInTagMeterParameterTable(txtTagMeterArticle.Text, formTagMeterParameter.Parameter))
                        {
                            if (registration.RetrieveTagMeterParameterTable(dataGridViewTagMeter))
                            {
                                MessageBox.Show("Update tag meter parameter is successful", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                txtTagMeterArticle.Clear();
                            }
                        }
                    }
                }
                else if (iRetVal == 0)
                {
                    MessageBox.Show("Article number " + sArticleNumber + " is not in server database.", "Stop", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                }
            }
        }

        private void btnArticleTagMeterDelete_Click(object sender, EventArgs e)
        {
            string sArticleNumber = txtTagMeterArticle.Text;
            if (sArticleNumber.Length == ARTICLE_DIGIT)
            {
                if (registration.TagMeterArticleExitsInTagMeterParameterTable(sArticleNumber) == 1)
                {
                    if (registration.TagMeterArticleExitsInTagMeterParameterTable(sArticleNumber) == 0)
                    {
                        if (registration.DeleteTagMeterParameterFromTagMeterParameterTable(sArticleNumber))
                        {
                            if (registration.RetrieveTagMeterParameterTable(dataGridViewTagMeter))
                            {
                                MessageBox.Show("Add new Tag Meter Article number is successful.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                        }
                    }
                    else
                    {
                        MessageBox.Show("Cannot delete.\nThis tag meter article is being used in article list.", "Stop", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    }
                }
            }
            else
            {
                MessageBox.Show("Article number " + sArticleNumber + " is not in server database.", "Stop", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            }
        }

        bool IsArticleDataOK()
        {
            if (txtArticleNumber.Text.Length < ARTICLE_DIGIT)
            {
                if (txtArticleNumber.Text.Length == ARTICLE_DIGIT - 1)
                {
                    string sArticleNumber = txtArticleNumber.Text.Substring(0, 3) + " " + txtArticleNumber.Text.Substring(3, txtArticleNumber.Text.Length - 3);
                    txtArticleNumber.Text = sArticleNumber;
                }
                else
                {
                    MessageBox.Show("Please enter ariticle number.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }
            }

            if(cmbProductType.Text.Length == 0)
            {
                MessageBox.Show("Please product type.", "Warning!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            if (txtCustomerName.Text.Length == 0)
            {
                MessageBox.Show("Please enter Label Name.", "Warning!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (txtLabelName.Text.Length == 0)
            {
                MessageBox.Show("Please enter Label Name.", "Warning!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (txtQuantityPerBox.Text.Length == 0)
            {
                MessageBox.Show("Please enter quantity per box.", "Warning!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (cmbLabelLayout.SelectedIndex < 0)
            {
                MessageBox.Show("Please select label layout", "Warning!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (cmbTesting.SelectedIndex < 0)
            {
                MessageBox.Show("Please select testing.", "Warning!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            if (txtTagProgrammerTrimingFrequency.Text.Length > 0)
            {
                try
                {
                    double dlTrimmingFrequency = Convert.ToDouble(txtTagProgrammerTrimingFrequency.Text);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Triming Frequency.\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    txtTagProgrammerTrimingFrequency.Text = String.Empty;
                    return false;
                }
            }
            else
            {
                txtTagProgrammerTrimingFrequency.Text = "0";
            }

            if (txtTagProgrammerPowerLevel.Text.Length > 0)
            {
                try
                {
                    double dPowerLevel = Convert.ToDouble(txtTagProgrammerPowerLevel.Text);
                    if (Convert.ToInt32(dPowerLevel * 10) > 1000)
                    {
                        MessageBox.Show("Please enter power level between 0-100%", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        txtTagProgrammerPowerLevel.Text = String.Empty;
                        return false;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Power Level.\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    txtTagProgrammerPowerLevel.Text = String.Empty;
                    return false;
                }
            }
            else
            {
                txtTagProgrammerPowerLevel.Text = "0";
            }

            txtBeginningUID.Text = txtBeginningUID.Text.ToUpper();
            txtEndingUID.Text = txtEndingUID.Text.ToUpper();
            return true;
        }

        void ClearArticleField()
        {
            txtArticleNumber.Clear();
            txtCustomerName.Clear();
            txtLabelName.Clear();
            txtBeginningUID.Clear();
            txtEndingUID.Clear();
            txtTagProgrammerTrimingFrequency.Clear();
            txtQuantityPerBox.Clear();
            txtTagProgrammerPowerLevel.Clear();
            cmbTagType.SelectedIndex = -1;
            cmbLabelLayout.SelectedIndex = -1;
            cmbRegister.SelectedIndex = -1;
            cmbTagMeter.SelectedIndex = -1;
            cmbTesting.SelectedIndex = -1;
            cmbTagProgrammer.SelectedIndex = -1;
            cmbProgramOption.SelectedIndex = -1;
            cmbProductType.SelectedIndex = -1;
            cmbProductType.Text = String.Empty;
        }

        private void dataGridViewArticle_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex > -1)
            {
                txtArticleNumber.Text = dataGridViewArticle.Rows[e.RowIndex].Cells["dgvArticleArticleNumber"].Value.ToString();
                txtCustomerName.Text = dataGridViewArticle.Rows[e.RowIndex].Cells["dgvArticleCustomerName"].Value.ToString();
                txtLabelName.Text = dataGridViewArticle.Rows[e.RowIndex].Cells["dgvArticleLabel"].Value.ToString();
                txtQuantityPerBox.Text = dataGridViewArticle.Rows[e.RowIndex].Cells["dgvArticleQuantityPerBox"].Value.ToString();
                switch (dataGridViewArticle.Rows[e.RowIndex].Cells["dgvArticleTagType"].Value.ToString())
                {
                    case "HDX": cmbTagType.SelectedIndex = 0;
                        break;
                    case "HDX+": cmbTagType.SelectedIndex = 1;
                        break;
                    case "FDX": cmbTagType.SelectedIndex = 2;
                        break;
                }
                cmbRegister.SelectedIndex = dataGridViewArticle.Rows[e.RowIndex].Cells["dgvArticleRegister"].Value.ToString() == "Register" ? 1 : 0;
                cmbTagMeter.SelectedIndex = dataGridViewArticle.Rows[e.RowIndex].Cells["dgvArticleTagMeterEnable"].Value.ToString() == "Use" ? 1 : 0;
                cmbTesting.SelectedIndex = dataGridViewArticle.Rows[e.RowIndex].Cells["dgvArticleTagMeterSkipTest"].Value.ToString() == "Test" ? 1 : 0;
                cmbTagProgrammer.SelectedIndex = dataGridViewArticle.Rows[e.RowIndex].Cells["dgvArticleTagProgrammerEnable"].Value.ToString() == "Use" ? 1 : 0;
                cmbProgramOption.SelectedIndex = Convert.ToInt32(dataGridViewArticle.Rows[e.RowIndex].Cells["dgvArticleTagProgrammerLock"].Value.ToString());
                txtTagProgrammerPowerLevel.Text = dataGridViewArticle.Rows[e.RowIndex].Cells["dgvArticleTagProgrammerPowerLevel"].Value.ToString();
                txtTagProgrammerTrimingFrequency.Text = dataGridViewArticle.Rows[e.RowIndex].Cells["dgvArticleTagProgrammerTrimingFrequency"].Value.ToString();
                cmbLabelLayout.SelectedIndex = dataGridViewArticle.Rows[e.RowIndex].Cells["dgvArticleLabelLayout"].Value.ToString() == "0" ? 0 : 1;

                if (dataGridViewArticle.Rows[e.RowIndex].Cells["dgvArticleBeginningUID"].Value != null)
                {
                    txtBeginningUID.Text = dataGridViewArticle.Rows[e.RowIndex].Cells["dgvArticleBeginningUID"].Value.ToString();
                }
                if (dataGridViewArticle.Rows[e.RowIndex].Cells["dgvArticleEndingUID"].Value != null)
                {
                    txtEndingUID.Text = dataGridViewArticle.Rows[e.RowIndex].Cells["dgvArticleEndingUID"].Value.ToString();
                }

                cmbProductType.SelectedIndex = -1;
                for(int iIndex = 0; iIndex < cmbProductType.Items.Count; iIndex++)
                {
                    if(cmbProductType.Items[iIndex].ToString() == dataGridViewArticle.Rows[e.RowIndex].Cells["dgvArticleProductType"].Value.ToString())
                    {
                        cmbProductType.SelectedIndex = iIndex;
                        break;
                    }
                }

                if(cmbProductType.SelectedIndex == -1)
                {
                    cmbProductType.Text = dataGridViewArticle.Rows[e.RowIndex].Cells["dgvArticleProductType"].Value.ToString();
                }
            }
        }

        private void dataGridViewTagMeter_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex > -1)
            {
                txtTagMeterArticle.Clear();
                txtTagMeterArticle.Text = dataGridViewTagMeter.Rows[e.RowIndex].Cells["dgvTagMeterAticle"].Value.ToString();
            }
        }

        private void cmbProductType_SelectedIndexChanged(object sender, EventArgs e)
        {
            /*
            string[] asProductTypeInfo = registration.GetProductTypeInformationFormServerDatabase(cmbProductType.Text);
            if(asProductTypeInfo != null)
            {
                cmbTagType.SelectedIndex = Convert.ToInt32(asProductTypeInfo[1]);
                cmbTagMeter.SelectedIndex = Convert.ToInt32(Convert.ToBoolean(asProductTypeInfo[2]));
                cmbTesting.SelectedIndex = cmbTagMeter.SelectedIndex;
                if (Convert.ToBoolean(asProductTypeInfo[3]))
                {
                    //txtTagProgrammerTrimingFrequency.Enabled = true;
                }
                else
                {
                    //txtTagProgrammerTrimingFrequency.Enabled = false;
                    txtTagProgrammerTrimingFrequency.Clear();
                }
                cmbTagProgrammer.SelectedIndex = Convert.ToInt32(Convert.ToBoolean(asProductTypeInfo[4]));
                //if (Convert.ToBoolean(asProductTypeInfo[3]) && !Convert.ToBoolean(asProductTypeInfo[4]))
                //{
                //    //cmbProgramOption.SelectedIndex = 2;
                //}
                //else
                //{
                //    //cmbProgramOption.SelectedIndex = Convert.ToInt32(Convert.ToBoolean(asProductTypeInfo[5]));
                //}

                if (Convert.ToBoolean(asProductTypeInfo[6]))
                {
                    //txtBeginningUID.Enabled = txtEndingUID.Enabled = true;
                    cmbLabelLayout.SelectedIndex = 0;
                }
                else
                {
                    //txtBeginningUID.Enabled = txtEndingUID.Enabled = false;
                    txtBeginningUID.Clear();
                    txtEndingUID.Clear();
                    cmbLabelLayout.SelectedIndex = 1;
                }
                cmbRegister.SelectedIndex = Convert.ToInt32(Convert.ToBoolean(asProductTypeInfo[6]));
            }
            */
        }
    }
}
