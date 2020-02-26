using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Simulation
{
    public class SimulationMessage
    {
        string _simulationID;
        string _alternateID;
        string _message;
        int _percent;
        bool _isFatalError = false;
        bool _isProgress = false;

        public string SimulationID
        {
            get { return _simulationID; }
            set { _simulationID = value; }
        }

        public string Message
        {
            get { return _message; }
            set { _message = value; }
        }

        public string AlternateID
        {
            get { return _alternateID; }
            set { _alternateID = value; }
        }

        public int Percent
        {
            get { return _percent; }
            set { _percent = value; }
        }

        public bool IsFatalError
        {
            get { return _isFatalError; }
            set { _isFatalError = value; }
        }

        public bool IsProgress
        {
            get { return _isProgress; }
            set { _isProgress = value; }
        }



        public SimulationMessage(string message)
        {
            _message = message.ToString();
            _simulationID = SimulationMessaging.SimulationID;
            _alternateID = SimulationMessaging.AlternateID;
            _percent = -1;
            if (message.Length > 11)
            {
                if (_message.Substring(0, 11) == "Fatal Error")
                {
                    _isFatalError = true;
                }
            }
        }

        public SimulationMessage(string message, bool isProgress)
        {
            _message = message.ToString();
            _simulationID = SimulationMessaging.SimulationID;
            _alternateID = SimulationMessaging.AlternateID;
            _percent = -1;
            _isProgress = isProgress;
            if (message.Length > 11)
            {
                if (_message.Substring(0, 11) == "Fatal Error")
                {
                    _isFatalError = true;
                }
            }
        }

        public SimulationMessage(string message, int percent)
        {
            _message = message.ToString();
            _simulationID = SimulationMessaging.SimulationID;
            _alternateID = SimulationMessaging.AlternateID;
            _percent = percent;
        }
    }
}
