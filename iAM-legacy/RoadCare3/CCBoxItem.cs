using System;
using System.Collections.Generic;
using System.Text;

namespace RoadCare3
{
    public class CCBoxItem {
        private int val;
        public int Value {
            get { return val; }
            set { val = value; }
        }
        
        private string name;
        public string Name {
            get { return name; }
            set { name = value; }
        }

        public CCBoxItem() {
        }

        public CCBoxItem(string name, int val) {
            this.name = name;
            this.val = val;
        }

        public override string ToString() {
            return name;
        }
    }
}
