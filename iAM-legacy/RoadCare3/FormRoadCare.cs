namespace RoadCare3
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.IO;
    using System.Reflection;
    using System.Windows.Forms;
    using CompileEquationCriteria;
    using DatabaseManager;
    using Reports;
    using Reports.DDOT;
    using RoadCare3.Properties;
    using RoadCareDatabaseOperations;
    using WeifenLuo.WinFormsUI.Docking;


    public partial class FormRoadCare : Form
    {
        private SolutionExplorer m_solutionExplorer = null;
        private FormOutputWindow m_formOutputWindow = null;

        private bool m_bIsCancelled = false;		

        private FormSpecialReportConfig specialReportForm;

        public FormRoadCare()
        {
            InitializeComponent();

            var formLogin = new FormLogin();
            //Project is now open source so removing license check.
            //var now = DateTime.Now;
            //if (now >= Global.ExactMomentWhenLicenseExpires)
            //{
            //    MessageBox.Show(
            //        "License has expired.  Please contact Applied Research Associates to renew.",
            //        Global.BrandCaptionForMessageBoxes,
            //        MessageBoxButtons.OK,
            //        MessageBoxIcon.Error);

            //    Environment.Exit(0);
            //}

            //if (now >= Global.FirstDayOfLicenseExpirationWarning)
            //{
            //    MessageBox.Show(
            //        $"Warning: License expires {Global.LastDayOfLicense}.  Please contact Applied Research Associates to renew.",
            //        Global.BrandCaptionForMessageBoxes,
            //        MessageBoxButtons.OK,
            //        MessageBoxIcon.Warning);
            //}

            var strFile = "";
#if MDSHA
            AuthenticationResult loginAttempt = MDSHAAuthentication.Authenticate( Global.SecurityOperations );
            if( loginAttempt.Successful )
#endif
            if (formLogin.ShowDialog() == DialogResult.OK)
            {
                // DBOps will use the provider to determine SQL syntax.
                DBOp.Provider = DBMgr.NativeConnectionParameters.Provider;
                // Show the Output Window
                m_formOutputWindow = new FormOutputWindow();
                FormManager.AddOutputWindow(m_formOutputWindow);

                // Show the solution explorer
                m_solutionExplorer = new SolutionExplorer(ref dockPanelMain);
                m_solutionExplorer.TabText = "RoadCare Explorer";
                FormManager.AddSolutionExplorer(m_solutionExplorer);

                String myDocumentsFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

                String projectsDirectory = myDocumentsFolder + "\\RoadCare Projects";
                if(!Directory.Exists(projectsDirectory))
                {
                    Directory.CreateDirectory(projectsDirectory);
                }
                
                
                myDocumentsFolder += "\\RoadCare Projects\\Temp";

                Settings.Default.ROADCARE_FILE_PATH = myDocumentsFolder;

                // Add the explution soLOLar and the output window to the main dock panel if we do not have a previous settings file.
                strFile = myDocumentsFolder + "\\TAB_SETTINGS.xml";
                if (!File.Exists(strFile))
                {
                    m_formOutputWindow.Show(dockPanelMain, DockState.DockBottom);
                    m_solutionExplorer.Show(dockPanelMain, DockState.DockLeft);
                }
                else
                {
                    // Load the tab settings (and other user settings?) from the XML file
                    try
                    {
                        DeserializeDockContent deserializeDockContent = new DeserializeDockContent(GetContentFromPersistString);
                        dockPanelMain.LoadFromXml(strFile, deserializeDockContent);
                    }
                    catch (Exception exc)
                    {
                        Global.WriteOutput("Error: Problem processing TAB_SETTINGS.xml file. " + exc.Message);
                    }
                }
                if (FormManager.GetOutputWindow().Visible == false)
                {
                    m_formOutputWindow.Show(dockPanelMain, DockState.DockBottom);

                }
                this.Text = "DSS RoadCare:" + DBMgr.GetNativeConnection().Database.ToString();
                if (!Global.SecurityOperations.CanLoadBinaries())
                {
                    loadBinariesToolStripMenuItem.Enabled = false;
                }
            }
            else
            {
                m_bIsCancelled = true;
#if MDSHA
                //MessageBox.Show( loginAttempt.Message );
#else
#endif
            }
            return;
        }



        public void MarshallExcel()
        {
            backgroundWorkerExcelMarshal.RunWorkerAsync();
        }

        private void formRoadCare_Load(object sender, EventArgs e)
        {
            //Environment.CurrentDirectory = Path.GetDirectoryName(Application.ExecutablePath);
            //string specialFolder = ".\\TempDLL";

            //// Re-create the folder that was deleted.
            //Directory.CreateDirectory(specialFolder);

            if (!m_bIsCancelled)
            {
                //// Start an instance of Excel for marshalling purposes
                //Thread thread = new Thread(new ThreadStart(MarshallExcel));
                //thread.Priority = ThreadPriority.BelowNormal;
                //thread.Start();

                // Loads field map on a property grid from display names to database names.
                Global.LoadAttributeHash();
                Global.LoadNetworkHash();
            }
            else
            {
                this.Close();
            }
        }

        private IDockContent GetContentFromPersistString(string persistString)
        {
            if (persistString == typeof(SolutionExplorer).ToString())
                return m_solutionExplorer;
            if (persistString == typeof(FormOutputWindow).ToString())
                return m_formOutputWindow;
            
            // Below is the code necessary to load documents back after the user closes the program and re-opens it.
            // The current implementation produces unexpected results when re-opening documents and needs to be further
            // explored.

            //else if (persistString.Contains(typeof(FormAttributeDocument).ToString()))
            //{
            //    string[] parsedStrings = persistString.Split(new char[] { ',' });
            //    if (parsedStrings.Length != 2)
            //        return null;

            //    if (parsedStrings[0] != typeof(FormAttributeDocument).ToString())
            //        return null;
            //    if (DBMgr.TableExist(parsedStrings[1].ToString()))
            //    {
            //        TreeNode tn = FormManager.GetSolutionExplorer().GetTreeView().Nodes["NodeAttribute"].Nodes[parsedStrings[1].ToString()];
            //        FormManager.GetSolutionExplorer().GetTreeView().SelectedNode = tn;
            //        FormAttributeDocument form = new FormAttributeDocument(parsedStrings[1].ToString());
            //        FormManager.AddRawAttributeForm(form);
            //        form.Tag = parsedStrings[1].ToString();
            //        return form;
            //    }
            //}
            return null;
            
        }

        private void solutionExplorerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!FormManager.IsSolutionExplorerOpen())
            {
                m_solutionExplorer = new SolutionExplorer(ref dockPanelMain);
                m_solutionExplorer.TabText = "RoadCare Explorer";
                m_solutionExplorer.Show(dockPanelMain, DockState.DockLeft);
                FormManager.AddSolutionExplorer(m_solutionExplorer);
            }
        }

        private void simulationToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            FormSummaryCheck check = new FormSummaryCheck();
            check.Show();
           
        }

        private void timerSimulation_Tick(object sender, EventArgs e)
        {
            List<Simulation.SimulationMessage> listSimulation = Simulation.SimulationMessaging.GetProgressList();
            lock (listSimulation)
            {
                foreach (Simulation.SimulationMessage message in listSimulation)
                {
                    Global.WriteOutput(message.Message);
                }
            }
            Simulation.SimulationMessaging.ClearProgressList();

            //if (!m_simulationThread.IsAlive)
            //{
            //    timerSimulation.Stop();
            //    this.Cursor = Cursors.Default;
            //}
        }

        private void outputWindowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!FormManager.IsOutputWindowOpen())
            {
                m_formOutputWindow = new FormOutputWindow();
                m_formOutputWindow.Show(dockPanelMain, DockState.DockBottom);
                FormManager.AddOutputWindow(m_formOutputWindow);
            }
        }

        private void FormRoadCare_FormClosed(object sender, FormClosedEventArgs e)
        {
            Reports.Report.CloseExcel();
            GC.Collect();
        }

        private void tsmiExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void tsmiAbout_Click(object sender, EventArgs e)
        {
            AboutBoxRoadCare aboutBox = new AboutBoxRoadCare();
            aboutBox.StartPosition = FormStartPosition.CenterScreen;
            aboutBox.ShowDialog();
        }

        private void optionsToolStripMenuItem1_Click( object sender, EventArgs e )
        {
            if( Global.SecurityOperations.CanModifyOptions() )
            {
                FormOptions formOptions = new FormOptions();
                formOptions.ShowDialog();
            }
        }

        private void helpToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            string helpFile = Path.Combine(System.Windows.Forms.Application.StartupPath,
                                    "Roadcare_Help.chm"); 
            Help.ShowHelp(this, helpFile);
        }

        private void loadBinariesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            BinaryLoader makeBinaries = new BinaryLoader();
            this.Cursor = Cursors.WaitCursor;
            makeBinaries.CreateDLLs();
            this.Cursor = Cursors.Arrow;
        }

        private void mileagePerFuncClassAndWardToolStripMenuItem_Click( object sender, EventArgs e )
        {
            FormNetworkSelector chooseNetwork = new FormNetworkSelector();
            chooseNetwork.ShowDialog();
            if (chooseNetwork.m_IsValidID)
            {
                MileageByFunctionalClassiAndWard mbfcawReport = new MileageByFunctionalClassiAndWard(chooseNetwork.NetworkID, Global.SecurityOperations.CurrentUserName);
                mbfcawReport.CreateReport();
                Global.WriteOutput("Done generating MileagePerFunctionalClassAndWard report at " + DateTime.Now.TimeOfDay);
            }
        }

        private void hPMSReportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormHPMS formHPMS = new FormHPMS();
            formHPMS.Show(dockPanelMain, DockState.Document);
        }

        private void unlockAllNetworkRollupsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string delete = "DELETE FROM MULTIUSER_LOCK";
            DBMgr.ExecuteNonQuery(delete);
        }

        private void backgroundWorkerExcelMarshal_DoWork(object sender, DoWorkEventArgs e)
        {
            Report.XL.Visible = false;
            Report.XL.UserControl = false;
            Microsoft.Office.Interop.Excel._Workbook oWB = Report.CreateWorkBook();
            Microsoft.Office.Interop.Excel._Worksheet oSheet = (Microsoft.Office.Interop.Excel._Worksheet)oWB.ActiveSheet;
            oSheet.Cells.Font.Bold = true;
            oWB.Close(false, Missing.Value, Missing.Value);
        }

        private void specialReportsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // If the Special Reports form is already created, just bring it to
            // the front. Otherwise, create it and keep track of it, allowing it
            // to "un-track" itself when it's closed.
            if (this.specialReportForm == null)
            {
                this.specialReportForm = new FormSpecialReportConfig();
                this.specialReportForm.FormClosed +=
                    (sender_, e_) => this.specialReportForm = null;
                this.specialReportForm.Show(); // non-modal!
            }

            this.specialReportForm.BringToFront();
        }

        private void importOMSToolStripMenuItem_Click(object sender, EventArgs e)
        {


        }

        private void menuMain_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }
    }
}
