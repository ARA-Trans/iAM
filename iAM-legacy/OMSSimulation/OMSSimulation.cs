using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace OMSSimulation
{
    public class OMSSimulation
    {
        string _simulationID;
        string _networkID;

        string _simulation;
        Thread _thread;
        TabPage _tabPage;
        TextBox _textBox;


        public string SimulationID
        {
            get { return _simulationID; }
            set { _simulationID = value; }
        }

        public string NetworkID
        {
            get { return _networkID; }
            set { _networkID = value; }
        }
        
        public string Simulation
        {
            get { return _simulation; }
            set { _simulation = value; }
        }

        public Thread Thread
        {
            get { return _thread; }
            set { _thread = value; }
        }

        public TabPage TabPage
        {
            get { return _tabPage; }
            set { _tabPage = value; }
        }

        public TextBox TextBox
        {
            get { return _textBox; }
            set { _textBox = value; }
        }


        public OMSSimulation(string networkID, string simulationID, string simulation)
        {
            _networkID = networkID;
            _simulationID = simulationID;
            _simulation = simulation;
        }

        public override string ToString()
        {
            return _simulation + "(" + _simulationID + ")";
        }


        public TabPage CreateNewTabPage()
        {
            _tabPage = new TabPage(this.ToString());
            _textBox = new TextBox();
            _textBox.Multiline = true;
            _textBox.Dock = DockStyle.Fill;
            _tabPage.Controls.Add(_textBox);
            _textBox.ScrollBars = ScrollBars.Both;
            return _tabPage;
        }



    }
}
