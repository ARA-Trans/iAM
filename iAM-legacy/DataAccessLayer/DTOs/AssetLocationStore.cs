using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataAccessLayer.DTOs
{
    public class AssetLocationStore
    {
        string _oid;
        string _id;
        string _name;
        string _shape;


        public string OID
        {
            get { return _oid; }
            set { _oid = value; }
        }


        public string ID
        {
            get { return _id; }
            set { _id = value; }
        }
        
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public string Shape
        {
            get { return _shape; }
            set { _shape = value; }
        }

    }
}
