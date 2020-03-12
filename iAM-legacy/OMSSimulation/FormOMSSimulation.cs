using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using DatabaseManager;
using System.Data.SqlClient;
using Simulation;
using System.Threading;

namespace OMSSimulation
{
    public partial class FormOMSSimulation : Form
    {
        bool _isLastTick;

         public FormOMSSimulation()
        {
            InitializeComponent();
            cgOMS.Prefix = "cgDE_";
        }

        private void FormOMSSimulation_Load(object sender, EventArgs e)
        {
            textBoxConnectionString.Text = Properties.Settings.Default.ConnectionString;
            comboBoxSimulationDescription.Focus();
            if(!string.IsNullOrWhiteSpace(textBoxConnectionString.Text))
           {
               ConnectToDatabase(textBoxConnectionString.Text);
           }
        }

       


        private void textBoxConnectionString_Validated(object sender, EventArgs e)
        {

            Properties.Settings.Default.ConnectionString = textBoxConnectionString.Text;
            Properties.Settings.Default.Save();
            ConnectToDatabase(textBoxConnectionString.Text);
            if (DBMgr.GetNativeConnection() != null)
            {
                if (DBMgr.GetNativeConnection().State == ConnectionState.Open)
                {
                    tabControlOutput.TabPages.Clear();
                    comboBoxSimulationDescription.Items.Clear();

                    FillSimulations();
                }
            }
        }

        public void ConnectToDatabase(string connectionString)
        {
            try
            {
                DBMgr.NativeConnectionParameters = new ConnectionParameters(connectionString, false,"MSSQL");
                labelConnection.Text = "Connected to database " + DBMgr.GetNativeConnection().Database;
                labelConnection.ForeColor = Color.DarkGreen;
            }
            catch (Exception e)
            {
                labelConnection.Text = "Error connecting to database. " + e.Message.Replace("\r\n","");
                labelConnection.ForeColor = Color.DarkRed;
            }
        }

        private void buttonRunSimulation_Click(object sender, EventArgs e)
        {
            OMSSimulation oms = (OMSSimulation)comboBoxSimulationDescription.SelectedItem;
            if (oms != null && oms.TabPage == null)
            {

                tabControlOutput.TabPages.Add(oms.CreateNewTabPage());
                tabControlOutput.SelectedTab = oms.TabPage;
                RunModel(oms);
            }
            else if (oms != null && oms.TabPage != null)
            {
                oms.TextBox.Clear();
                oms.Thread = null;
                RunModel(oms);

            }
            LoadEditSection(oms.SimulationID);
        }



        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (object item in comboBoxSimulationDescription.Items)
            {
                OMSSimulation oms = (OMSSimulation)item;
                if (tabControlOutput.SelectedTab == oms.TabPage)
                {
                    oms.TabPage = null;
                    if (oms.Thread != null)
                    {
                        oms.Thread.Abort();
                        oms.Thread = null;
                    }
                }
            }
            tabControlOutput.TabPages.Remove(tabControlOutput.SelectedTab);

        }


        private void FillSimulations()
        {

            using (SqlConnection connection = new SqlConnection(textBoxConnectionString.Text))
            {
                comboBoxSimulationDescription.Items.Clear();
                try
                {
                    connection.Open();
                    string query = "SELECT NETWORKID, SIMULATIONID, SIMULATION FROM " + cgOMS.Prefix + "SIMULATIONS";
                    SqlCommand cmd = new SqlCommand(query, connection);
                    SqlDataReader dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        string networkID = dr["NETWORKID"].ToString();
                        string simulationIDd = dr["SIMULATIONID"].ToString();
                        string simulation = dr["SIMULATION"].ToString();
                        comboBoxSimulationDescription.Items.Add(new OMSSimulation(networkID,simulationIDd,simulation));
                    }
                }
                catch(Exception e)
                {
                    labelConnection.Text = "Error retrieving simulations. " + e.Message.Replace("\r\n", "");
                }
            }
        }



        private void RunModel(OMSSimulation oms)
        {

            try
            {
                if (SimulationMessaging.Valid)
                {
                    oms.TextBox.Text = "Beginning Simulation " + oms.ToString() + " at " + DateTime.Now.ToString();
                    Simulation.Simulation simulation = new Simulation.Simulation(oms.Simulation, "", oms.SimulationID, oms.NetworkID);
                    oms.Thread = new Thread(new ParameterizedThreadStart(simulation.CompileSimulation));
                    oms.Thread.Start(false);
                    timerSimulation.Start();
                }
            }
            catch (Exception e)
            {
                oms.TextBox.Text = "Error starting simulation " + oms.ToString() + ". " + e.Message;
            }
        }

        private void timerSimulation_Tick(object sender, EventArgs e)
        {
            bool isOneAlive = false;
            List<SimulationMessage> listSimulation = Simulation.SimulationMessaging.GetProgressList();
            lock (listSimulation)
            {
                foreach (object item in comboBoxSimulationDescription.Items)
                {
                    OMSSimulation oms = (OMSSimulation)item;
                    if (oms.Thread != null && oms.Thread.IsAlive)
                    {
                        List<SimulationMessage> listOMS = listSimulation.FindAll(delegate(SimulationMessage sm) { return sm.SimulationID == oms.SimulationID; });
                        foreach (SimulationMessage message in listOMS)
                        {
                            oms.TextBox.Text += "\r\n";
                            oms.TextBox.Text += message.Message + "(" + message.Percent.ToString() + "%)";
                            oms.TextBox.SelectionStart = oms.TextBox.Text.Length;
                            oms.TextBox.ScrollToCaret();
                            oms.TextBox.Refresh();
                        }
                        isOneAlive = true;
                        _isLastTick = false;
                    }
                }
            }
            //Updates user web user interface for Cartegraph on progress.  Checks if it is running.  If not running tells interface.
            Simulation.SimulationMessaging.ClearProgressList();
            if (!isOneAlive)
            {
                if (!_isLastTick)
                {
                    _isLastTick = true;
                }
                else
                {
                    timerSimulation.Stop();
                }
            }
        }


        private void LoadEditSection(string simulationID)
        {
            textBoxSectionID.Text = Properties.Settings.Default.LastSectionID;

            using (SqlConnection connection = new SqlConnection(textBoxConnectionString.Text))
            {
                comboBoxYear.Items.Clear();
                try
                {
                    connection.Open();
                    string query = "SELECT FIRSTYEAR,NUMBERYEARS FROM " + cgOMS.Prefix + "INVESTMENTS WHERE SIMULATIONID='" + simulationID + "'";
                    SqlCommand cmd = new SqlCommand(query, connection);
                    SqlDataReader dr = cmd.ExecuteReader();
                    if (dr.Read())
                    {
                        int firstYear = Convert.ToInt32(dr["FIRSTYEAR"]);
                        int numberYears = Convert.ToInt32(dr["NUMBERYEARS"]);
                        for (int year = firstYear; year < firstYear + numberYears; year++)
                        {
                            comboBoxYear.Items.Add(year);
                        }
                    }
                }
                catch
                {
                }
            }


            using (SqlConnection connection = new SqlConnection(textBoxConnectionString.Text))
            {
                comboBoxTreatment.Items.Clear();
                try
                {
                    connection.Open();
                    string query = "SELECT TREATMENT FROM " + cgOMS.Prefix + "TREATMENTS WHERE SIMULATIONID='" + simulationID + "'";
                    SqlCommand cmd = new SqlCommand(query, connection);
                    SqlDataReader dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        string treatment = dr["TREATMENT"].ToString();
                        comboBoxTreatment.Items.Add(treatment);

                    }
                }
                catch
                {
                }
            }
        }

        private void textBoxSectionID_TextChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.LastSectionID = textBoxSectionID.Text;
            Properties.Settings.Default.Save();
        }

        private void buttonUpdate_Click(object sender, EventArgs e)
        {


        }

    }
}
