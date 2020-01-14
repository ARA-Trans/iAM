using AppliedResearchAssociates.PciDistress;
using DatabaseManager;
using DataObjects;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace RoadCare3
{
    public partial class FormPCIBulkLoad : BaseForm
    {
        int m_nNextPCIID;
        private bool m_bIsLinear;
        private bool m_bError;
        TextWriter m_twPCI;
        TextWriter m_twPCIDetail;
        Hashtable m_hashMethodDistress;//Key - method, value is hash of distresses and numbers

        DataAdapter dataAdapter;
        BindingSource binding;
        DataTable table;


        /// <summary>
        /// Initialize the bulk loader with either Linear or Section referencing
        /// </summary>
        /// <param name="bIsLinear"></param>
        public FormPCIBulkLoad(bool bIsLinear)
        {
            LoadPCIMethods();
            m_bIsLinear = bIsLinear;
            InitializeComponent();
            this.TabText = "PCI Bulk Load";
            m_nNextPCIID = RoadCareGlobalOperations.GlobalDatabaseOperations.GetNextPCIID();
        }

        private void LoadPCIMethods()
        {
            m_hashMethodDistress = RoadCareGlobalOperations.GlobalDatabaseOperations.GetPCIDistresses();
        }

        private void FormPCIBulkLoad_Load(object sender, EventArgs e)
        {
            if (dataAdapter != null) dataAdapter.Dispose();// Free up the resources
            if (binding != null) binding.Dispose();
            if (table != null) table.Dispose();

            try
            {
                string strQuery;
                if (m_bIsLinear)
                {
                    strQuery = "SELECT * FROM PCI_VALIDATION_LINEAR ORDER BY ROUTES, DIRECTION, BEGIN_STATION, END_STATION, DATE_";
                }
                else
                {
                    strQuery = "SELECT * FROM PCI_VALIDATION ORDER BY FACILITY, SECTION, DATE_, SAMPLE_";
                }
                dataAdapter = new DataAdapter(strQuery);

                // Populate a new data table and bind it to the BindingSource.
                table = new DataTable();
                table.Locale = System.Globalization.CultureInfo.InvariantCulture;
                dataAdapter.Fill(table);
                binding = new BindingSource();
                binding.DataSource = table;
                dgvPCIBulkLoad.DataSource = binding;
                bindingNavigatorPCIValidation.BindingSource = binding;
                dgvPCIBulkLoad.ClipboardCopyMode = DataGridViewClipboardCopyMode.EnableAlwaysIncludeHeaderText;
                if (m_bIsLinear)
                {
                    Validation.LoadLRS();
                }
                else
                {
                    Validation.LoadSRS();
                }
            }
            catch (Exception exception)
            {
                Global.WriteOutput("Error: Building current attribute view. SQL message is " + exception.Message);
            }

            foreach (DataGridViewColumn column in dgvPCIBulkLoad.Columns)
            {
                column.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            }
            
        }

        private void toolStripButtonValidate_Click(object sender, EventArgs e)
        {
            m_bError = false;
            if (m_bIsLinear) ValidateLinear();
            else ValidateSection();

        }

        public void ValidateLinear()
        {
            string strMyDocumentsFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            strMyDocumentsFolder += "\\RoadCare Projects\\Temp";
            Directory.CreateDirectory(strMyDocumentsFolder);

            string strOutFile = strMyDocumentsFolder + "\\PCIValidationErrors.txt";
            TextWriter tw = new StreamWriter(strOutFile);

            FormManager.GetOutputWindow().ClearWindow();
            foreach (DataGridViewRow row in dgvPCIBulkLoad.Rows)
            {
                string strError = "";
                if (!Validation.IsValidRow(row))
                {
                    if (row.Index != dgvPCIBulkLoad.Rows.Count - 1)
                    {
                        SetRowColor(row, Color.LightCoral);
                        tw.WriteLine("Entire row must be entered before Bulk Loading. Row:" + row.Index.ToString() + ", " +
                        row.Cells["ROUTES"].Value + ", " +
                        row.Cells["BEGIN_STATION"].Value + ", " +
                        row.Cells["END_STATION"].Value + ", " +
                        row.Cells["SAMPLE_"].Value + ", " +
                        row.Cells["DATE_"].Value);
                        m_bError = true;
                    }
                    continue;
                }
                LRSObjectPCI lrsObjectPCI;

                try
                {
                    lrsObjectPCI = new LRSObjectPCI(row.Cells["ROUTES"].Value.ToString(), double.Parse(row.Cells["BEGIN_STATION"].Value.ToString()), double.Parse(row.Cells["END_STATION"].Value.ToString()), row.Cells["DIRECTION"].Value.ToString(), row.Cells["SAMPLE_"].Value.ToString(), DateTime.Parse(row.Cells["DATE_"].Value.ToString()), row.Cells["METHOD_"].Value.ToString(), row.Cells["TYPE_"].Value.ToString(), double.Parse(row.Cells["AREA"].Value.ToString()), row.Cells["DISTRESS"].Value.ToString(), row.Cells["SEVERITY"].Value.ToString(), double.Parse(row.Cells["AMOUNT"].Value.ToString()));
                }
                catch //(Exception except)
                {
                    tw.WriteLine("Error validating PCI Object" + row.Index.ToString() + ", " +
                        row.Cells["ROUTES"].Value + ", " +
                        row.Cells["BEGIN_STATION"].Value + ", " +
                        row.Cells["END_STATION"].Value + ", " +
                        row.Cells["SAMPLE_"].Value + ", " +
                        row.Cells["DATE_"].Value);
                    m_bError = true;
                    continue;
                }

                if (!Validation.ValidateLinear((LRSObject)lrsObjectPCI, out strError))
                {
                    tw.WriteLine(strError + ", " +
                        row.Cells["ROUTES"].Value + ", " +
                        row.Cells["BEGIN_STATION"].Value + ", " +
                        row.Cells["END_STATION"].Value + ", " +
                        row.Cells["SAMPLE_"].Value + ", " +
                        row.Cells["DATE_"].Value);
                    m_bError = true;
                    continue;
                }

                

                if (!Validation.ValidatePCIDetail(lrsObjectPCI, out strError))
                {
                    tw.WriteLine(strError + ", " +
                        row.Cells["ROUTES"].Value + ", " +
                        row.Cells["BEGIN_STATION"].Value + ", " +
                        row.Cells["END_STATION"].Value + ", " +
                        row.Cells["SAMPLE_"].Value + ", " +
                        row.Cells["DATE_"].Value);
                    m_bError = true;
                    continue;
                }
            }
            if (m_bError)
            {
                Global.WriteOutput("Errors encountered during validation, please check error log file before continuing.");
            }
            else
            {
                Global.WriteOutput("Validation of PCI Distress complete. No errors or warnings.");
            }
            tw.Close();
        }

        public void ValidateSection()
        {
            string strMyDocumentsFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            strMyDocumentsFolder += "\\RoadCare Projects\\Temp";
            Directory.CreateDirectory(strMyDocumentsFolder);

            string strOutFile = strMyDocumentsFolder + "\\PCIValidationErrors.txt";
            TextWriter tw = new StreamWriter(strOutFile);

            string strError = "";
            FormManager.GetOutputWindow().ClearWindow();
            SRSObjectPCI srsObjectPCI = new SRSObjectPCI();
            foreach (DataGridViewRow row in dgvPCIBulkLoad.Rows)
            {
                if (row.Cells["SAMPLE_"].Value == null) row.Cells["SAMPLE_"].Value = "";
                if (!Validation.IsValidRow(row))
                {
                    if (row.Index != dgvPCIBulkLoad.Rows.Count - 1)
                    {
                        tw.WriteLine("Entire row must be entered before Bulk Loading. Row:" + row.Index.ToString() + ", " +
                        row.Cells["FACILITY"].Value + ", " +
                        row.Cells["SECTION"].Value + ", " +
                        row.Cells["DATE_"].Value + ", " +
                        row.Cells["SAMPLE_"].Value);
                        m_bError = true;
                    }
                    continue;
                }
                

                try
                {
                    srsObjectPCI.Facility = row.Cells["FACILITY"].Value.ToString();
                    srsObjectPCI.Section = row.Cells["SECTION"].Value.ToString();
                    srsObjectPCI.Sample = row.Cells["SAMPLE_"].Value.ToString();
                    srsObjectPCI.Date = Convert.ToDateTime(row.Cells["DATE_"].Value);
                    srsObjectPCI.Method = row.Cells["METHOD_"].Value.ToString();
                    srsObjectPCI.Type = row.Cells["TYPE_"].Value.ToString();
                    srsObjectPCI.Area = double.Parse(row.Cells["AREA"].Value.ToString());
                    srsObjectPCI.Distress = row.Cells["DISTRESS"].Value.ToString();
                    srsObjectPCI.Severity = row.Cells["SEVERITY"].Value.ToString();
                    srsObjectPCI.Amount = double.Parse(row.Cells["AMOUNT"].Value.ToString());
                }
                catch (Exception except)
                {
                    tw.WriteLine("Error: Validating PCI Detail object. " + except.Message + " Row:" + row.Index.ToString() + ", " +
                        row.Cells["FACILITY"].Value + ", " +
                        row.Cells["SECTION"].Value + ", " +
                        row.Cells["DATE_"].Value + ", " +
                        row.Cells["SAMPLE_"].Value);
                    m_bError = true;
                    continue;
                }

                if (!Validation.ValidateSection((SRSObject)srsObjectPCI, out strError))
                {
                    tw.WriteLine(strError + ", " +
                        row.Cells["FACILITY"].Value + ", " +
                        row.Cells["SECTION"].Value + ", " +
                        row.Cells["DATE_"].Value + ", " +
                        row.Cells["SAMPLE_"].Value);
                    m_bError = true;
                    continue;
                }

                if (!Validation.ValidatePCIDetail(srsObjectPCI, out strError))
                {
                    tw.WriteLine(strError + ", " +
                        row.Cells["FACILITY"].Value + ", " +
                        row.Cells["SECTION"].Value + ", " +
                        row.Cells["DATE_"].Value + ", " +
                        row.Cells["SAMPLE_"].Value);
                    m_bError = true;
                    continue;
                }
                
            }
            if (m_bError)
            {
                Global.WriteOutput("Errors encountered during validation, please check output file before continuing.");
            }
            else
            {
                Global.WriteOutput("Validation of PCI Distress complete. No errors or warnings.");
            }
            tw.Close();
        }
        
        private void SetRowColor(DataGridViewRow row, Color color)
        {
            foreach (DataGridViewCell cell in row.Cells)
            {
                cell.Style.BackColor = color;
            }
        }

        public void ImportPCI()
        {
            m_bError = false;
            bool bIsMetricData = false;
            if (m_bIsLinear) ValidateLinear();
            else ValidateSection();
            if (m_bError) return;

            string query = "SELECT OPTION_VALUE FROM OPTIONS WHERE OPTION_NAME = 'PCI_UNITS'";
            string units = DBMgr.ExecuteQuery(query).Tables[0].Rows[0].ItemArray[0].ToString();
            if (units == "METRIC")
            {
                bIsMetricData = true;
            }
            List<PCIDistressObject> distressRows = new List<PCIDistressObject>();
            query = "SELECT DISTRESSNUMBER, DISTRESSNAME, METHOD_, METRIC_CONVERSION FROM PCI_DISTRESS";
            DataSet allDistressInfo = DBMgr.ExecuteQuery(query);
            foreach (DataRow distressInfo in allDistressInfo.Tables[0].Rows)
            {
                PCIDistressObject distressProperties =
                    new PCIDistressObject(distressInfo["DISTRESSNAME"].ToString(),
                        int.Parse(distressInfo["DISTRESSNUMBER"].ToString()), (float.Parse(distressInfo["METRIC_CONVERSION"].ToString())));
                distressRows.Add(distressProperties);
            }

            string strMyDocumentsFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            strMyDocumentsFolder += "\\RoadCare Projects\\Temp";
            Directory.CreateDirectory(strMyDocumentsFolder);

            string strOutFile = strMyDocumentsFolder + "\\pci.txt";
            File.Delete(strOutFile);
            m_twPCI = new StreamWriter(strOutFile);

            string strOutFileDetails = strMyDocumentsFolder + "\\pciDetails.txt";
            File.Delete(strOutFileDetails);
            m_twPCIDetail = new StreamWriter(strOutFileDetails);

            if (m_bError) return;// If there is an error in the BULK PCI load, don't load

            List<LRSObjectPCI> listPCI = new List<LRSObjectPCI>();
            LRSObjectPCI pciPrevious = null;
            foreach (DataGridViewRow row in dgvPCIBulkLoad.Rows)
            {
                if (!Validation.IsValidRow(row)) continue;
                LRSObjectPCI lrsObjectPCI = new LRSObjectPCI(row.Cells["ROUTES"].Value.ToString(), double.Parse(row.Cells["BEGIN_STATION"].Value.ToString()), double.Parse(row.Cells["END_STATION"].Value.ToString()), row.Cells["DIRECTION"].Value.ToString(), row.Cells["SAMPLE_"].Value.ToString(), DateTime.Parse(row.Cells["DATE_"].Value.ToString()), row.Cells["METHOD_"].Value.ToString(), row.Cells["TYPE_"].Value.ToString(), double.Parse(row.Cells["AREA"].Value.ToString()), row.Cells["DISTRESS"].Value.ToString(), row.Cells["SEVERITY"].Value.ToString(), double.Parse(row.Cells["AMOUNT"].Value.ToString()));
                
                if (listPCI.Count > 0)
                {
                    if (pciPrevious.Route != lrsObjectPCI.Route ||
                        pciPrevious.BeginStation != lrsObjectPCI.BeginStation ||
                        pciPrevious.EndStation != lrsObjectPCI.EndStation ||
                        pciPrevious.Direction != lrsObjectPCI.Direction ||
                        pciPrevious.Sample != lrsObjectPCI.Sample ||
                        pciPrevious.Area != lrsObjectPCI.Area ||
                        pciPrevious.Method != lrsObjectPCI.Method ||
                        pciPrevious.Type != lrsObjectPCI.Type ||
                        pciPrevious.Date != lrsObjectPCI.Date)
                    {
                        WritePCISample(listPCI, m_nNextPCIID, distressRows, bIsMetricData);
                        listPCI.Clear();
                        m_nNextPCIID++;
                    }
                }

                pciPrevious = lrsObjectPCI;
                listPCI.Add(lrsObjectPCI);

            }
            WritePCISample(listPCI, m_nNextPCIID, distressRows, bIsMetricData);
            m_twPCI.Close();
            m_twPCIDetail.Close();

            DBMgr.SQLBulkLoad("PCI", strOutFile, '\t');

            DBMgr.SQLBulkLoad("PCI_DETAIL", strOutFileDetails, '\t');
            this.Close();  
        }

        public void ImportSRSPCI_()
        {					
            m_bError = false;
            bool bIsMetricData = false;
            if (m_bIsLinear)
            {
                ValidateLinear();
            }
            else
            {
                ValidateSection();
            }
            if (m_bError) return;

            string query = "SELECT OPTION_VALUE FROM OPTIONS WHERE OPTION_NAME = 'PCI_UNITS'";
            string units = DBMgr.ExecuteQuery(query).Tables[0].Rows[0].ItemArray[0].ToString();
            if (units == "METRIC")
            {
                bIsMetricData = true;
            }
            List<PCIDistressObject> distressRows = new List<PCIDistressObject>();
            query = "SELECT DISTRESSNUMBER, DISTRESSNAME, METHOD_, METRIC_CONVERSION FROM PCI_DISTRESS";
            DataSet allDistressInfo = DBMgr.ExecuteQuery(query);
            foreach (DataRow distressInfo in allDistressInfo.Tables[0].Rows)
            {
                PCIDistressObject distressProperties =
                    new PCIDistressObject(distressInfo["DISTRESSNAME"].ToString(),
                        int.Parse(distressInfo["DISTRESSNUMBER"].ToString()), float.Parse(distressInfo["METRIC_CONVERSION"].ToString()));
                distressRows.Add(distressProperties);
            }
            string strMyDocumentsFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            strMyDocumentsFolder += "\\RoadCare Projects\\Temp";
            Directory.CreateDirectory(strMyDocumentsFolder);

            string strOutFile = strMyDocumentsFolder + "\\pci.txt";
            File.Delete(strOutFile);
            m_twPCI = new StreamWriter(strOutFile);

            string strOutFileDetails = strMyDocumentsFolder + "\\pciDetails.txt";
            File.Delete(strOutFileDetails);
            m_twPCIDetail = new StreamWriter(strOutFileDetails);

            if (m_bError) return;// If there is an error in the BULK PCI load, don't load

            List<SRSObjectPCI> listPCI = new List<SRSObjectPCI>();
            SRSObjectPCI pciPrevious = null;
            foreach (DataGridViewRow row in dgvPCIBulkLoad.Rows)
            {
                if (!Validation.IsValidRow(row)) continue;
                SRSObjectPCI srsObjectPCI = new SRSObjectPCI(row.Cells["FACILITY"].Value.ToString(), row.Cells["SECTION"].Value.ToString(),row.Cells["SAMPLE_"].Value.ToString(), DateTime.Parse(row.Cells["DATE_"].Value.ToString()), row.Cells["METHOD_"].Value.ToString(), row.Cells["TYPE_"].Value.ToString(), double.Parse(row.Cells["AREA"].Value.ToString()), row.Cells["DISTRESS"].Value.ToString(), row.Cells["SEVERITY"].Value.ToString(), double.Parse(row.Cells["AMOUNT"].Value.ToString()));

                if (listPCI.Count > 0)
                {
                    if (pciPrevious.Facility != srsObjectPCI.Facility ||
                        pciPrevious.Section != srsObjectPCI.Section ||
                        pciPrevious.Sample != srsObjectPCI.Sample ||
                        pciPrevious.Area != srsObjectPCI.Area ||
                        pciPrevious.Method != srsObjectPCI.Method ||
                        pciPrevious.Type != srsObjectPCI.Type ||
                        pciPrevious.Date != srsObjectPCI.Date)
                    {
                        WritePCISample(listPCI, m_nNextPCIID, distressRows, bIsMetricData);
                        listPCI.Clear();
                        m_nNextPCIID++;
                    }
                }

                pciPrevious = srsObjectPCI;
                listPCI.Add(srsObjectPCI);

            }
            WritePCISample(listPCI, m_nNextPCIID, distressRows, bIsMetricData);
            m_twPCI.Close();
            m_twPCIDetail.Close();

            DBMgr.SQLBulkLoad("PCI", strOutFile, '\t');
            m_nNextPCIID++;
            m_nNextPCIID++;

            DBMgr.SQLBulkLoad("PCI_DETAIL", strOutFileDetails, '\t');
            this.Close();
        }

        private void WritePCISample(List<LRSObjectPCI> listPCI,int nNextID, List<PCIDistressObject> distressRows, bool bIsMetricData)
        {
            if (listPCI.Count == 0) return;
            nNextID++;

            double dArea = listPCI[0].Area;
            string strMethod = listPCI[0].Method;
            string strType = listPCI[0].Type;
            StringBuilder sbDeduct = new StringBuilder();
            double dPCI = 100.0;

            foreach (LRSObjectPCI pci in listPCI)
            {
                string strDistress = pci.Distress;
                string strSeverity = pci.Severity;
                double dAmount = pci.Amount;
                double dExtent = dAmount / dArea;

                if (strDistress != "No Distress")
                {
                    double dConvertArea = dArea;
                    if(bIsMetricData)
                    {
                        dConvertArea = dArea * ((distressRows.Find(delegate(PCIDistressObject distressObject)
                        { return distressObject.Distress == strDistress; })).MetricRatio);			
                    }
                    double dDeduct = CalculateCurrentRowDeducts(strMethod, strDistress, strSeverity, dAmount, dConvertArea);
                    if (sbDeduct.Length != 0) sbDeduct.Append(",");
                    sbDeduct.Append(dDeduct.ToString());
                    string strDetail = "\t" + nNextID.ToString() + "\t" + strDistress + "\t" + strSeverity + "\t" + dAmount.ToString() + "\t" + (dExtent * 100).ToString("f6") + "\t" + dDeduct.ToString("f6");
                    m_twPCIDetail.WriteLine(strDetail);
                }
            }
            if (sbDeduct.Length > 0)
            {
                dPCI = PciDistress.ComputePCIValue(sbDeduct.ToString(), strMethod);
            }
            string strPCI = nNextID.ToString() + "\t" + listPCI[0].Route + "\t" + listPCI[0].BeginStation.ToString() + "\t" + listPCI[0].EndStation.ToString() + "\t" + listPCI[0].Direction +
                "\t\t\t\t" + listPCI[0].Date.ToString() + "\t\t" + listPCI[0].Method + "\t" + dArea.ToString() + "\t" + dPCI.ToString("f2") + "\t\t\t\t" + listPCI[0].Type;

            m_twPCI.WriteLine(strPCI);
        }

        private void WritePCISample(List<SRSObjectPCI> listPCI, int m_nNextID, List<PCIDistressObject> distressRows, bool bIsMetricData)
        {
            if (listPCI.Count == 0) return;
            m_nNextID++;

            double dArea = listPCI[0].Area;
            string strMethod = listPCI[0].Method;
            string strType = listPCI[0].Type;
            StringBuilder sbDeduct = new StringBuilder();
            double dPCI = 100.0;

            foreach (SRSObjectPCI pci in listPCI)
            {
                string strDistress = pci.Distress;
                string strSeverity = pci.Severity;
                double dAmount = pci.Amount;
                double dExtent = dAmount / dArea;

                if (strDistress != "No Distress")
                    {
                    double dConvertArea = dArea;
                    if (bIsMetricData)
                    {
                        dConvertArea = dArea * ((distressRows.Find(delegate(PCIDistressObject distressObject)
                            { return distressObject.Distress == strDistress; })).MetricRatio);
                    }
                    double dDeduct = CalculateCurrentRowDeducts(strMethod, strDistress, strSeverity, dAmount, dConvertArea);

                    if (sbDeduct.Length != 0) sbDeduct.Append(",");
                    sbDeduct.Append(dDeduct.ToString());
                    string strDetail = "\t" + m_nNextPCIID.ToString() + "\t" + strDistress + "\t" + strSeverity + "\t" + dAmount.ToString() + "\t" + dExtent.ToString("f8") + "\t" + dDeduct.ToString("f8");
                    m_twPCIDetail.WriteLine(strDetail);
                }
            }
            
            if (sbDeduct.Length > 0)
            {
                if(listPCI[0].Facility == "TL1")
                {

                }
                dPCI = PciDistress.ComputePCIValue(sbDeduct.ToString(), strMethod);
            }

            string strPCI = m_nNextPCIID.ToString() + "\t\t\t\t\t" + listPCI[0].Facility + "\t" + listPCI[0].Section + "\t" + listPCI[0].Sample + "\t" + listPCI[0].Date.ToString() + "\t\t" + listPCI[0].Method + "\t" + dArea.ToString() + "\t" + dPCI.ToString("f2") + "\t\t\t\t" + listPCI[0].Type + "\t";
            m_twPCI.WriteLine(strPCI);
        }

        private double CalculateCurrentRowDeducts(string strMethod, string strDistress, string strSeverity, double dAmount, double dArea)
        {
            PCIMethodObject method = Validation.PCIMethods.Find(delegate(PCIMethodObject pciMethod){return pciMethod.Method == strMethod;});
            if(method == null) return 0.0;
            
            double dPCIDeduct = 0.0;
            if (PciDistress.IsWASHCLKMethod(method.Method))
            {
                double dExtent = dAmount / dArea;
                string extent = string.Format("{0:F2}", dExtent);
                int nDistress = method.GetDistress(strDistress);
                dPCIDeduct = PciDistress.pvt_ComputeNonPCIDeduct(method.Method, nDistress, strSeverity, dExtent);
            }
            else
            {
                dPCIDeduct = PciDistress.pvt_ComputePCIDeduct(method.GetDistress(strDistress), strSeverity, dAmount, dArea);
            }
            return dPCIDeduct;
        }

        private void pasteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PCIPasteRows();
        }

        private void PCIPasteRows()
        {
            // Write out the rows to paste in to a file
            string s = Clipboard.GetText();
            string[] lines = s.Split('\n');

            string strMyDocumentsFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            strMyDocumentsFolder += "\\RoadCare Projects\\Temp";
            Directory.CreateDirectory(strMyDocumentsFolder);

            string strOutFile = strMyDocumentsFolder + "\\PCI_DETAILS_TO_VALIDATE.txt";
            TextWriter tw = new StreamWriter(strOutFile);
            foreach (string line in lines)
            {
                tw.WriteLine(line);
            }
            tw.Close();

            if (m_bIsLinear)
            {
                DBMgr.ExecuteNonQuery("DELETE FROM PCI_VALIDATION_LINEAR");
                // Now load that file into the PCI_VALIDATION table.
                DBMgr.SQLBulkLoad("PCI_VALIDATION_LINEAR", strOutFile, '\t');

                if (dataAdapter != null) dataAdapter.Dispose();// Free up the resources
                if (binding != null) binding.Dispose();
                if (table != null) table.Dispose();

                try
                {
                    string strQuery = "SELECT * FROM PCI_VALIDATION_LINEAR";
                    dataAdapter = new DataAdapter(strQuery);

                    // Populate a new data table and bind it to the BindingSource.
                    table = new DataTable();
                    table.Locale = System.Globalization.CultureInfo.InvariantCulture;
                    dataAdapter.Fill(table);
                    binding = new BindingSource();
                    binding.DataSource = table;
                    dgvPCIBulkLoad.DataSource = binding;
                    bindingNavigatorPCIValidation.BindingSource = binding;
                    dgvPCIBulkLoad.ClipboardCopyMode = DataGridViewClipboardCopyMode.EnableAlwaysIncludeHeaderText;
                }
                catch (Exception exception)
                {
                    Global.WriteOutput("Error: Building PCI Linear view.  " + exception.Message);
                }

            }
            else 
            {
                DBMgr.ExecuteNonQuery("DELETE FROM PCI_VALIDATION");
                // Now load that file into the PCI_VALIDATION table.
                DBMgr.SQLBulkLoad("PCI_VALIDATION", strOutFile, "\\t");

                if (dataAdapter != null) dataAdapter.Dispose();// Free up the resources
                if (binding != null) binding.Dispose();
                if (table != null) table.Dispose();

                try
                {
                    string strQuery = "SELECT * FROM PCI_VALIDATION";
                    dataAdapter = new DataAdapter(strQuery);

                    // Populate a new data table and bind it to the BindingSource.
                    table = new DataTable();
                    table.Locale = System.Globalization.CultureInfo.InvariantCulture;
                    dataAdapter.Fill(table);
                    binding = new BindingSource();
                    binding.DataSource = table;
                    dgvPCIBulkLoad.DataSource = binding;
                    bindingNavigatorPCIValidation.BindingSource = binding;
                    dgvPCIBulkLoad.ClipboardCopyMode = DataGridViewClipboardCopyMode.EnableAlwaysIncludeHeaderText;
                }
                catch (Exception exception)
                {
                    Global.WriteOutput("Error: Building PCI Section view.  " + exception.Message);
                }
            }
            Validation.LoadSRS();
        }

        private void toolStripButtonImport_Click(object sender, EventArgs e)
        {
            if (m_bIsLinear) ImportPCI();
            else ImportSRSPCI_();
            //ImportPCI();
            FormManager.formPCIDocument.Close();
        }

        private void tsbPCIDistress_Click(object sender, EventArgs e)
        {
            FormPCIDistressManager pciDistressManager = new FormPCIDistressManager();
            pciDistressManager.Show(FormManager.GetDockPanel());
        }
    }
}
