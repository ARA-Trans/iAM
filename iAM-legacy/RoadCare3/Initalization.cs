using System;
using System.Collections.Generic;
using System.Text;

namespace RoadCare3
{
    public class Initalization
    {
        private String m_strUserName;
        private String m_strProvider;
        private String m_strServer;
        private String m_strDataSource;

        public String m_strExcelHandle;
        public String m_strProcessID;

        public String ProcessID
        {
            get { return m_strProcessID; }
            set { m_strProcessID = value; }
        }

        public String UserName
        {
            get { return m_strUserName; }
            set { m_strUserName = value; }
        }

        public String Provider
        {
            get { return m_strProvider; }
            set { m_strProvider = value; }
        }

        public String Server
        {
            get { return m_strServer; }
            set { m_strServer = value; }
        }

        public String DataSource
        {
            get { return m_strDataSource; }
            set { m_strDataSource = value; }
        }

        public String ExcelHandle
        {
            get { return m_strExcelHandle; }
            set { m_strExcelHandle = value; }
        }
    }
}
