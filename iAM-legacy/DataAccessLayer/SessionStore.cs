using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataAccessLayer.DTOs;

namespace DataAccessLayer
{
    public class SessionStore
    {
        string _sessionID;
        string _simulationID;
        string _userName;
        string _orderByField;
        DateTime _sessionDate;
        List<string> _OIDs;
        List<string> _encodedActivities;
        SimulationStore _simulation;
        List<string> _treatmentsPerYear;
        List<ActivityStore> _activities;
        List<AssetLocationStore> _locations;
        Dictionary<string,  Dictionary<string, Dictionary<string,string>>> _oidResults;

        public SimulationStore Simulation
        {
            get { return _simulation; }
            set { _simulation = value; }
        }

        public List<string> OIDs
        {
            get { return _OIDs; }
            set { _OIDs = value; }
        }

        public string SessionID
        {
            get { return _sessionID; }
            set { _sessionID = value; }
        }

        public string SimulationID
        {
            get { return _simulationID; }
            set { _simulationID = value; }
        }

        public DateTime SessionDate
        {
            get { return _sessionDate; }
            set { _sessionDate = value; }
        }

        public string UserName
        {
            get { return _userName; }
            set { _userName = value; }
        }

        public string OrderByField
        {
            get { return _orderByField; }
            set { _orderByField = value; }
        }


        public List<string> TreatmentsPerYear
        {
            get { return _treatmentsPerYear; }
            set { _treatmentsPerYear = value; }
        }

        public List<ActivityStore> Activities
        {
            get { return _activities; }
            set { _activities = value; }
        }

        public Dictionary<string, Dictionary<string, Dictionary<string, string>>> OIDResults
        {
            get { return _oidResults; }
            set { _oidResults = value; }
        }

        /// <summary>
        ///  All activityStores for a given Section(oid)
        /// </summary>
        public List<string> EncodedActivities
        {
            get { return _encodedActivities; }
            set { _encodedActivities = value; }
        }

        /// <summary>
        /// List of locations of assets (contains OID, ID, Street and WKT)
        /// </summary>
        public List<AssetLocationStore> Locations
        {
            get { return _locations; }
            set { _locations = value; }
        }
    }
}
