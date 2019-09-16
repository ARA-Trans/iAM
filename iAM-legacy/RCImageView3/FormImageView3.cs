using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

using RCImageView3.Properties;
using WeifenLuo.WinFormsUI.Docking;
using RoadCare3;
using DatabaseManager;
using RoadCareDatabaseOperations;
using RoadCareGlobalOperations;

namespace RCImageView3
{
    public partial class FormImageView3 : Form
    {
        FormImageViewSolutionExplorer m_formSolutionExplorer;
        FormOutputWindow m_formOutputWindow;
        FormNavigation m_formNavigation;
        bool m_bClose = false;
		//bool m_lockOut = false;

        public FormImageView3()
        {
            InitializeComponent();
            RestoreApplicationLocation.GeometryFromString(Properties.Settings.Default.WindowGeometry, this);
   			FormLogin formLogin = new FormLogin();
            if (formLogin.ShowDialog() == DialogResult.OK)
            {
                // DBOps will use the provider to determine SQL syntax.
                DBOp.Provider = DBMgr.NativeConnectionParameters.Provider;
            }
            else
            {
                m_bClose = true;
               
            }
        }

        private void FormImageView3_Load(object sender, EventArgs e)
        {
            if (m_bClose)
            {
                this.Close();
                return;
            }
            this.Text = "RoadCare ImageView:Loading...";
            m_formSolutionExplorer = new FormImageViewSolutionExplorer();
            m_formOutputWindow = new FormOutputWindow();
            m_formNavigation = new FormNavigation();

            OutputWindow.Output = m_formOutputWindow;
            FormManager.AddOutputWindow(m_formOutputWindow);

            m_formOutputWindow.CloseButton = false;

            String strMyDocumentsFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            strMyDocumentsFolder += "\\RoadCare Projects\\ImageView\\Temp";
            Settings.Default.IMAGEVIEW_FILEPATH = strMyDocumentsFolder;

            // Add the explution soLOLar and the output window to the main dock panel if we do not have a previous settings file.
            String strFile = strMyDocumentsFolder + "\\TAB_SETTINGS.xml";
            if (!File.Exists(strFile))
            {
                m_formSolutionExplorer.Show(dockPanelMain, DockState.DockLeft);
                m_formOutputWindow.Show(dockPanelMain, DockState.DockBottom);

                int nWidth = m_formSolutionExplorer.Width;
                int nAppWidth = this.Width;


                m_formNavigation.Show(dockPanelMain,new Rectangle(nWidth,40,nAppWidth-nWidth,80));
            }
            else
            {
                // Load the tab settings (and other user settings?) from the XML file
                try
                {
                    DeserializeDockContent deserializeDockContent = new DeserializeDockContent(GetContentFromPersistString);
                    dockPanelMain.LoadFromXml(strFile, deserializeDockContent);

                    if (!m_formSolutionExplorer.Visible) m_formSolutionExplorer.Show(dockPanelMain, DockState.DockLeft);
                
                }
                catch (Exception exc)
                {
                    m_formSolutionExplorer.Show(dockPanelMain, DockState.DockLeft);
                    m_formOutputWindow.Show(dockPanelMain, DockState.DockBottom);
                    OutputWindow.WriteOutput("Error: Problem processing TAB_SETTINGS.xml file. " + exc.Message);
                }

            }
            OutputWindow.DockPanel = dockPanelMain;
            m_formNavigation.Navigation = new NavigationObject(ImageViewManager.Networks);

            if (String.IsNullOrEmpty(m_formNavigation.Navigation.ImagePath))
            {
                String strImagePath = Settings.Default.IMAGEPATH;
                if(String.IsNullOrEmpty(strImagePath))
                {
                    if(folderBrowserDialogImagePath.ShowDialog() == DialogResult.OK)
                    {
                        strImagePath = folderBrowserDialogImagePath.SelectedPath;
                        Settings.Default.IMAGEPATH = strImagePath;
                    }
                }
                m_formNavigation.Navigation.ImagePath = strImagePath;
            }

            this.Text = "RoadCare ImageView:" + DBMgr.GetNativeConnection().Database;


           



        }


        private IDockContent GetContentFromPersistString(string persistString)
        {
            if (persistString == typeof(FormImageViewSolutionExplorer).ToString())
                return m_formSolutionExplorer;
            if (persistString == typeof(FormOutputWindow).ToString())
                return m_formOutputWindow;
            if (persistString == typeof(FormNavigation).ToString())
                return m_formNavigation;

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

        private void FormImageView3_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (!m_bClose)
            {
                String strMyDocumentsFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

                strMyDocumentsFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                strMyDocumentsFolder += "\\RoadCare Projects\\ImageView\\Temp";
                Directory.CreateDirectory(strMyDocumentsFolder);
                String strOutFile = strMyDocumentsFolder + "\\TAB_SETTINGS.xml";
                dockPanelMain.SaveAsXml(strOutFile);

                Properties.Settings.Default.WindowGeometry = RestoreApplicationLocation.GeometryToString(this);
                Properties.Settings.Default.Save();
            }
        }
    }
}
