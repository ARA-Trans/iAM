using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace DataAccessLayer.DTOs
{
    [DataContract]
    public class ActivityResultStore
    {
        string _resultID;
        string _activityName;
        string _oid;
        string _budget="";
        int _year;
        double _cost;
        int _resultType;
        Dictionary<string, string> _attributeChange;
        int _ociChange;
        int _commitOrder=0;

        [DataMember]
        public string ResultID
        {
            get { return _resultID; }
            set { _resultID = value; }
        }
        
        [DataMember]
        public string OID
        {
            get { return _oid; }
            set { _oid = value; }
        }
        
        [DataMember]
        public int Year
        {
            get { return _year; }
            set { _year = value; }
        }
        
        [DataMember]
        public string Budget
        {
            get { return _budget; }
            set { _budget = value; }
        }
        
        [DataMember]
        public double Cost
        {
            get { return _cost; }
            set { _cost = value; }
        }

        [DataMember]
        public int ResultType
        {
            get { return _resultType; }
            set { _resultType = value; }
        }

        [DataMember]
        public int CommitOrder
        {
            get { return _commitOrder; }
            set { _commitOrder = value; }
        }


        public Dictionary<string, string> AttributeChange
        {
            get { return _attributeChange; }
            set { _attributeChange = value; }
        }

        public int OCIChange
        {
            get { return _ociChange; }
            set { _ociChange = value; }
        }

        public string ActivityName
        {
            get { return _activityName; }
            set { _activityName = value; }
        }

        public ActivityResultStore()
        {
        }


        public ActivityResultStore(byte[] encodedActivity, List<string> activities, List<string> budgets, int startYear)
        {
            _resultType = Convert.ToInt32(encodedActivity[0]);
            
            int budgetIndex = Convert.ToInt32(encodedActivity[1]);
            _budget = budgets[budgetIndex];

            int activityIndex = Convert.ToInt32(encodedActivity[2]);
            _activityName = activities[activityIndex];

            int cost = BitConverter.ToInt32(encodedActivity,3);
            _cost = Convert.ToDouble(cost);

            int deltaYear = Convert.ToInt32(encodedActivity[7]);

            _year = startYear + deltaYear;

            _ociChange = Convert.ToInt32(encodedActivity[8]) - 100;
        }

        public byte[] EncodeActivity(List<string> activities, List<string> budgets, int startYear)
        {
            byte[] activity = new byte[9];
            activity[0] = Convert.ToByte(_resultType);
            activity[1] = Convert.ToByte(budgets.IndexOf(_budget));
            activity[2] = Convert.ToByte(activities.IndexOf(_activityName));
            int i = 3;
            ///Integer cost
            byte[] costByte = BitConverter.GetBytes(Convert.ToInt32(_cost));
            foreach (byte b in costByte)
            {
                activity[i] = b;
                i++;
            }
            int yearDelta = _year - startYear;
            activity[7] = Convert.ToByte(yearDelta);
            activity[8] = Convert.ToByte(_ociChange);
            return activity;
        }

        public void DecodeActivity(byte[] arrayActivity, List<string> activities, List<string> budgets, int startYear)
        {
            _resultType = Convert.ToInt32(arrayActivity[0]);
            
            int indexBudget = Convert.ToInt32(arrayActivity[1]);
            _budget = budgets[indexBudget];

            int indexActivity = Convert.ToInt32(arrayActivity[2]);
            _activityName = activities[indexActivity];

            ArraySegment<byte> segment = new ArraySegment<byte>(arrayActivity, 3, 4);
            _cost = Convert.ToDouble(BitConverter.ToInt32(arrayActivity, 3));

            _year = startYear + Convert.ToInt32(arrayActivity[7]);

            _ociChange = Convert.ToInt32(arrayActivity[8]);
        }

        public override string ToString()
        {
            return _activityName + "(" + _year.ToString() + ")";
        }

    }
}
