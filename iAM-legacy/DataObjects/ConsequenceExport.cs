using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataObjects
{
    public class ConsequenceExport
    {
        String m_strAttribute;
        String m_strChange;

        public String Attribute
        {
            get { return m_strAttribute; }
            set { m_strAttribute = value; }

        }

        public String Change
        {

            get { return m_strChange; }
            set { m_strChange = value; }

        }
    }
}
