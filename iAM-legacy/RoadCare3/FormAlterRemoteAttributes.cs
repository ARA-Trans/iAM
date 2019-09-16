using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DatabaseManager;
using RoadCare3.Properties;

namespace RoadCare3
{
    public partial class FormAlterRemoteAttributes : Form
    {
        private string _oldTableName;
        private string _newTableName;
        private string _netDefName;
        List<RemoteAttribute> _toAlter;


        public string OldTableName
        {
            get { return _oldTableName; }
            set { _oldTableName = value; }
        }
        
        public string NewTableName
        {
            get { return _newTableName; }
            set { _newTableName = value; }
        }

        public FormAlterRemoteAttributes(string networkDefintion)
        {
            InitializeComponent();
            _netDefName = networkDefintion;
            string query = "";
            if(DBMgr.NativeConnectionParameters.Provider == "MSSQL")
            {
                query = "SELECT ATTRIBUTE_, SQLVIEW FROM ATTRIBUTES_ WHERE NATIVE_ = 'False' AND NETWORK_DEFINITION_NAME = '" + networkDefintion + "'";
            }
            if(DBMgr.NativeConnectionParameters.Provider == "ORACLE")
            {
                query = "SELECT ATTRIBUTE_, SQLVIEW FROM ATTRIBUTES_ WHERE NATIVE_ = 0 AND NETWORK_DEFINITION_NAME = '" + networkDefintion + "'";
            }
            DataSet remoteAttributeData = DBMgr.ExecuteQuery(query);
            List<string> updatesToPerform = new List<string>();
            foreach (DataRow dr in remoteAttributeData.Tables[0].Rows)
            {
                string attributeName = dr["ATTRIBUTE_"].ToString();
                string viewStmt = dr["SQLVIEW"].ToString();
                RemoteAttribute toAdd = new RemoteAttribute(networkDefintion, attributeName, viewStmt);
                listBoxRemoteAttributes.Items.Add(toAdd);
            }
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void listBoxRemoteAttributes_SelectedValueChanged(object sender, EventArgs e)
        {
            if (listBoxRemoteAttributes.SelectedItem != null)
            {
                textBoxSQLView.Text = ((RemoteAttribute)listBoxRemoteAttributes.SelectedItem).SqlView;
            }
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            string oldTableName = textBoxOldTableName.Text;
            string newTableName = textBoxNewTableName.Text;
            List<string> updatesToPerform = new List<string>();
            for (int i = 0; i < listBoxRemoteAttributes.SelectedItems.Count; i++)
            {
                RemoteAttribute current = ((RemoteAttribute)listBoxRemoteAttributes.SelectedItems[i]);
                string attributeName = current.AttributeName;
                string viewStmt = current.SqlView;
                if (viewStmt.Contains(oldTableName))
                {
                    // Perform the update.
                    viewStmt = viewStmt.Replace(oldTableName, newTableName);
                    string update = "UPDATE ATTRIBUTES_ SET SQLVIEW = '" + viewStmt + "' WHERE ATTRIBUTE_ = '" + attributeName + "' AND NETWORK_DEFINITION_NAME = '" + _netDefName + "'";

                    // Drop and recreate the views.
                    // TODO: Will have to create special tables/views for all ATTRIBUTES and ASSETS when creating remote or native attributes in the system. The tables/views created will need to include
                    // the network definition that the attribute belongs to. Alternate option is to move to remote data only, and handle everything from the ATTRIBUTES_ table.
                    // TODO: Hook up SQLVIEW text window to ATTRIBUTES_.
                    string dropView = "DROP VIEW " + attributeName + "";
                    updatesToPerform.Add(dropView);
                    string createView = "CREATE VIEW " + attributeName + " AS " + viewStmt;
                    updatesToPerform.Add(createView);
                    updatesToPerform.Add(update);

                }
                else
                {
                    //MessageBox.Show("Could not find old table name in SQL VIEW statement. Please check the name and try again.", "Alter Attribute Failed");
                    Global.WriteOutput("Could not find old table name in SQL VIEW statement for attribute " + attributeName + ".");
                    //foundMismatch = true;
                }
            }
            //DBMgr.ExecuteBatchNonQuery(updatesToPerform);
            foreach (string update in updatesToPerform)
            {
                try
                {
                    DBMgr.ExecuteNonQuery(update);
                    //Global.WriteOutput("Updated: " + update);
                }
                catch (Exception exc)
                {
                    Global.WriteOutput("Error processing update. " + update);
                }
            }

            this.Close();
        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            // Format is just going to be old table name, new table name, selected attributes.
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "Text Files | *.txt";
            sfd.DefaultExt = "txt";
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                string oldTableName = textBoxOldTableName.Text;
                string newTableName = textBoxNewTableName.Text;
                TextWriter tw = new StreamWriter(sfd.FileName);
                tw.WriteLine(oldTableName);
                tw.WriteLine(newTableName);
                foreach (RemoteAttribute ra in listBoxRemoteAttributes.SelectedItems)
                {
                    string attributeName = ra.AttributeName;
                    string sqlView = ra.SqlView;
                    tw.WriteLine(attributeName);
                    tw.WriteLine(sqlView);
                }
                tw.Close();
            }
        }

        private void buttonLoad_Click(object sender, EventArgs e)
        {
            List<RemoteAttribute> toSelect = new List<RemoteAttribute>();
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Text Files | *.txt";
            ofd.DefaultExt = "txt";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                TextReader tr = new StreamReader(ofd.FileName);
                textBoxOldTableName.Text = tr.ReadLine();
                textBoxNewTableName.Text = tr.ReadLine();
                while (tr.Peek() >= 0)
                {
                    string attributeName = tr.ReadLine();
                    if (attributeName != "")
                    {
                        // If we read in a valida attribute, it will be followed by a sqlView statement.
                        string sqlView = tr.ReadLine();
                        toSelect.Add(new RemoteAttribute(_netDefName, attributeName, sqlView));
                    }
                }
                tr.Close();
                for (int i = 0; i < listBoxRemoteAttributes.Items.Count; i++)
                {
                    RemoteAttribute toCheck = (RemoteAttribute)listBoxRemoteAttributes.Items[i];
                    if (toSelect.Exists(delegate(RemoteAttribute ra) { return ra.AttributeName == toCheck.AttributeName; }))
                    {
                        listBoxRemoteAttributes.SetSelected(i, true);
                    }
                }
            }
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Format is just going to be old table name, new table name, selected attributes.
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "Text Files | *.txt";
            sfd.DefaultExt = "txt";
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                string oldTableName = textBoxOldTableName.Text;
                string newTableName = textBoxNewTableName.Text;
                TextWriter tw = new StreamWriter(sfd.FileName);
                tw.WriteLine(oldTableName);
                tw.WriteLine(newTableName);
                foreach (RemoteAttribute ra in listBoxRemoteAttributes.SelectedItems)
                {
                    string attributeName = ra.AttributeName;
                    string sqlView = ra.SqlView;
                    tw.WriteLine(attributeName);
                    tw.WriteLine(sqlView);
                }
                tw.Close();
            }
        }

        private void loadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            List<RemoteAttribute> toSelect = new List<RemoteAttribute>();
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Text Files | *.txt";
            ofd.DefaultExt = "txt";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                TextReader tr = new StreamReader(ofd.FileName);
                textBoxOldTableName.Text = tr.ReadLine();
                textBoxNewTableName.Text = tr.ReadLine();
                while (tr.Peek() >= 0)
                {
                    string attributeName = tr.ReadLine();
                    if (attributeName != "")
                    {
                        // If we read in a valida attribute, it will be followed by a sqlView statement.
                        string sqlView = tr.ReadLine();
                        toSelect.Add(new RemoteAttribute(_netDefName, attributeName, sqlView));
                    }
                }
                tr.Close();
                for (int i = 0; i < listBoxRemoteAttributes.Items.Count; i++)
                {
                    RemoteAttribute toCheck = (RemoteAttribute)listBoxRemoteAttributes.Items[i];
                    if (toSelect.Exists(delegate(RemoteAttribute ra) { return ra.AttributeName == toCheck.AttributeName; }))
                    {
                        listBoxRemoteAttributes.SetSelected(i, true);
                    }
                }
            }
        }

        private void updateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RemoteAttribute selectedAttribute = ((RemoteAttribute)listBoxRemoteAttributes.SelectedItem);
            string update = "UPDATE ATTRIBUTES_ SET SQLVIEW = '" + textBoxSQLView.Text + "' WHERE ATTRIBUTE_ = '" + selectedAttribute.AttributeName;
            string dropView = "DROP VIEW " + selectedAttribute.AttributeName;
            string createView = "CREATE VIEW " + selectedAttribute.AttributeName + " AS " + textBoxSQLView.Text;
            try
            {
                DBMgr.ExecuteNonQuery(update);
                DBMgr.ExecuteNonQuery(dropView);
                DBMgr.ExecuteNonQuery(createView);
            }
            catch (Exception exc)
            {
                Global.WriteOutput("Error updating VIEW. " + exc.Message);
            }
        }
    }

    public class RemoteAttribute
    {
        private string _networkDefinition;
        private string _attributeName;

        public string AttributeName
        {
            get { return _attributeName; }
            set { _attributeName = value; }
        }
        private string _sqlView;

        public string SqlView
        {
            get { return _sqlView; }
            set { _sqlView = value; }
        }

        public RemoteAttribute(string networkDefintion, string attributeName, string sqlView)
        {
            _networkDefinition = networkDefintion;
            _attributeName = attributeName;
            _sqlView = sqlView;
        }

        public override string ToString()
        {
            return _attributeName;
        }
    }
}
