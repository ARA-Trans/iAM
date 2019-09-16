using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;
using System.Data.OleDb;
using System.Windows.Forms;

namespace DatabaseManager
{
    public class ConnectionParameters
    {
        private String m_strServer;
        private String m_strDatabase;
        private String m_strUserName;
        private String m_strPassword;
        private String m_strProvider;
		private string m_port;
        private String m_connectionString;
		private String m_strOracleEntryName;
		private bool m_bIsOleDbConnection;
		private bool connString;
		private bool m_bIntegratedSecurity;
		private string m_SID;
		private string m_networkAlias;
		private string m_connectionName;
		private string m_viewStatement;
		private string m_connectionID;
		private string m_identifier;
		
        
        private SqlConnection sqlConn;
        private OleDbConnection oleDbConn;

        private bool m_bIsNative;

        public ConnectionParameters(string connectString, bool bIsOleDbConnection,string provider)
        {
            m_bIsOleDbConnection = bIsOleDbConnection;
            m_connectionString = connectString;
            m_strProvider = provider;
            m_bIsNative = true;
            
            if (bIsOleDbConnection)
            {
                oleDbConn = new OleDbConnection(m_connectionString);
            }
            else
            {
                ParseConnString(connectString, false);
                sqlConn = new SqlConnection(m_connectionString);
                m_strDatabase = sqlConn.Database;
            }

        }


		public ConnectionParameters(
			string port,
			string sid,
			string serviceName,
			string dbUserID,
			string dbPassword,
			bool integratedSecurity,
			string server,
			string databaseName,
			string connectionName,
			string viewStatement,
			string identifier,
			string connectionID,
			string provider,
			bool isNative)
		{
			m_port = port;
			m_SID = sid;
			m_networkAlias = serviceName;
			m_strUserName = dbUserID;
			m_strPassword = dbPassword;
			m_bIntegratedSecurity = integratedSecurity;
			m_strServer = server;
			m_strDatabase = databaseName;
			m_connectionName = connectionName;
			m_connectionID = connectionID;
			m_viewStatement = viewStatement;
			m_identifier = identifier;
			m_strProvider = provider;
			m_bIsNative = isNative;
			
			if (integratedSecurity)
			{
				switch (provider)
				{
					case "MSSQL":
						m_bIsOleDbConnection = false;
						m_connectionString = "Data Source=" + server + ";Initial Catalog=" + databaseName + ";Integrated Security=SSPI;";
						sqlConn = new SqlConnection(m_connectionString);
						break;
					case "ORACLE":
						m_bIsOleDbConnection = true;
                        if (NetworkAlias == "")
                        {
                            m_connectionString = "Provider=OraOLEDB.Oracle;Data Source=" + server + ";OSAuthent=1;";
                        }
                        else
                        {
                            m_connectionString = "Provider=OraOLEDB.Oracle;Data Source=" + NetworkAlias + ";OSAuthent=1;";
                        }
						m_strOracleEntryName = server + "." + databaseName;
						oleDbConn = new OleDbConnection(m_connectionString);
						break;
					default:
						throw new InvalidOperationException("Database type not recognized");
				}
			}
			else
			{
				switch (provider)
				{
					case "MSSQL":
						m_bIsOleDbConnection = false;
						m_connectionString = "Data Source=" + server + ";Initial Catalog=" + databaseName + ";User Id=" + dbUserID + ";Password=" + dbPassword + ";Persist Security Info=" + true + ";";
						sqlConn = new SqlConnection(m_connectionString);
						break;
					case "ORACLE":
						m_bIsOleDbConnection = true;
                        if (String.IsNullOrEmpty(NetworkAlias))
                        {
                            m_connectionString = "Provider=OraOLEDB.Oracle;Data Source = (DESCRIPTION = (CID = GTU_APP)(ADDRESS_LIST = (ADDRESS = (PROTOCOL = TCP)(HOST = " + server + ")(PORT = " + port + ")))(CONNECT_DATA = (SID = " + sid + ")(SERVER = DEDICATED)));User Id = " + dbUserID + ";Password = " + dbPassword + ";";
                        }
                        else
                        {
                            m_connectionString = "Provider=OraOLEDB.Oracle;Data Source=" + NetworkAlias + ";User Id=" + UserName + ";Password=" + Password + ";";
                        }
						m_strOracleEntryName = server + "." + databaseName;
						oleDbConn = new OleDbConnection(m_connectionString);
						break;
					default:
						throw new InvalidOperationException("Database type not recognized");
				} 
			}
		}

		public ConnectionParameters(string provider, string connectionString, bool native)
		{
			m_bIsNative = native;
			m_strProvider = provider;
			string startConnString = "";
			switch (provider)
			{
				case "MSSQL":
					startConnString = "Provider = SQLNCLI;";
					break;
				case "ORACLE":
					startConnString = "Provider=OraOLEDB.Oracle;";
					break;
				default:
					throw new NotImplementedException("TODO: Implement ANSI ConnectionParameters constructor.");
			}
			m_connectionString = startConnString + connectionString;
			connString = true;

			//ParseConnString( connectionString, false );
			CreateConnection();
		}

		public void ParseConnString( string connectionString, bool checkForProvider )
		{
			//"Data Source = (DESCRIPTION = (CID = GTU_APP)(ADDRESS_LIST = (ADDRESS = (PROTOCOL = TCP)(HOST = trans-tester)(PORT = 1521)))(CONNECT_DATA = (SID = MDSHA)(SERVER = DEDICATED))); User Id = DBO_MDSHA_FIX; Password = DBO_MDSHA_FIX;"

			m_strUserName = ParseUserName( connectionString );
			m_strPassword = ParsePassword( connectionString );

			if( checkForProvider )
			{
				m_strProvider = ParseProvider( connectionString );
			}
			switch (m_strProvider)
			{
				case "ORACLE":
					m_SID = ParseSID( connectionString );
					break;
			}
		}

		private string ParsePassword( string connectionString )
		{
			//Password = " + m_strPassword + ";";
			string workingConnectionString = TrimNonLiteralSpaces( connectionString );
			int startIndex = workingConnectionString.ToUpper().IndexOf( "PASSWORD=", 0 ) + 9;
			int endIndex = workingConnectionString.IndexOf( ';', startIndex ) - 1;
			int length = endIndex - startIndex + 1;
			string password = workingConnectionString.Substring( startIndex, length ).Trim();

			return password;

		}

		private string ParseUserName( string connectionString )
		{
			string workingConnectionString = TrimNonLiteralSpaces( connectionString );
			int startIndex = workingConnectionString.ToUpper().IndexOf( "USERID=", 0 ) + 7;
			int endIndex = workingConnectionString.IndexOf( ';', startIndex ) - 1;
			int length = endIndex - startIndex + 1;
			string userName = workingConnectionString.Substring( startIndex, length );

			return userName;
		}

		private string ParseSID( string connectionString )
		{
			string workingConnectionString = TrimNonLiteralSpaces( connectionString );

			int startIndex = workingConnectionString.ToUpper().IndexOf( "SID=", 0 ) + 4;

			int endIndex = -1;

			if( startIndex == 3 ) //"SID=" was found [-1 + 4 = 3]
			{
				startIndex = workingConnectionString.ToUpper().IndexOf( "DATASOURCE=", 0 ) + 11;
			}
			int semiColonIndex = workingConnectionString.IndexOf( ';', startIndex );
			int endParenthesisIndex = workingConnectionString.IndexOf( ')', startIndex );


			if( semiColonIndex > -1 )
			{
				if( endParenthesisIndex > -1 )
				{
					endIndex = Math.Min( semiColonIndex, endParenthesisIndex ) - 1;
				}
				else
				{
					endIndex = semiColonIndex - 1;
				}
			}
			else
			{
				if( endParenthesisIndex > -1 )
				{
					endIndex = endParenthesisIndex - 1;
				}
				else
				{
					endIndex = workingConnectionString.Length - 1;
				}
			}




			string sid = workingConnectionString.Substring( startIndex, endIndex - startIndex + 1 ).Trim();

			return sid;

		}

		private string TrimNonLiteralSpaces( string connectionString )
		{
			string trimmed = "";
			bool literalMode = false;
			for( int i = 0; i < connectionString.Length; ++i )
			{
				switch( connectionString[i] )
				{
					case '\"':
						trimmed = trimmed + connectionString[i];
						literalMode = !literalMode;
						break;
					case ' ':
						if( literalMode )
						{
							trimmed = trimmed + connectionString[i];
						}
						break;
					default:
						trimmed = trimmed + connectionString[i];
						break;
				}
			}

			return trimmed;
		}

		private string ParseProvider( string strConnectionString )
		{
			int equalsIndex= strConnectionString.IndexOf( '=' );
			int semiColonIndex = strConnectionString.IndexOf( ';', equalsIndex );
			string provider = strConnectionString.Substring( equalsIndex + 1, semiColonIndex - equalsIndex - 1 );
			switch( provider.ToUpper() )
			{
				case "MSDAORA":
				case "ORAOLEDB.ORACLE":
					provider = "ORACLE";
					break;
			}
			return provider.Trim().ToUpper();
		}

		private void CreateConnection()
        {
			if (connString)
			{
				if (String.Compare(m_strProvider, "MSSQL", true) == 0)
				{
					m_bIsOleDbConnection = false;
					sqlConn = new SqlConnection(m_connectionString);
				}
				else
				{
					m_bIsOleDbConnection = true;
					oleDbConn = new OleDbConnection( m_connectionString );
				}
			}
        }

		public String OracleNameEntry
		{
			get
			{
				return m_strOracleEntryName;
			}
		}
		
		public String Server
        {
            get { return m_strServer; }
        }

        public String Database
        {
            get { return m_strDatabase; }
        }

        public String UserName
        {
            get { return m_strUserName; }
        }

        public String Password
        {
            get { return m_strPassword; }
        }

        public String Provider
        {
            get { return m_strProvider; }
            set { m_strProvider = value; }
        }

		public string SID
		{
			get
			{
				return m_SID;
			}
		}

		public string NetworkAlias
		{
			get
			{
				return m_networkAlias;
			}
		}

		public string ViewStatement
		{
			get
			{
				return m_viewStatement;
			}
		}

		public string Port
		{
			get
			{
				return m_port;
			}
		}

        public String ConnectionString
        {
            get { return m_connectionString; }
            //set { m_connectionString = value; }
        }

        public OleDbConnection OleDbConnection
        {
            get
			{
				return oleDbConn;
			}
			set
			{
				oleDbConn = value;
			}
        }

        public SqlConnection SqlConnection
        {
			get
			{
				return sqlConn;
			}
			set
			{
				sqlConn = value;
			}
        }

        public bool IsNative
        {
            get { return m_bIsNative; }
        }

		public bool IsOleDBConnection
		{
			get
			{
				return m_bIsOleDbConnection;
			}
		}

		public string ConnectionName
		{
			get
			{
				return m_connectionName;
			}
		}

		public string Identifier
		{
			get
			{
				return m_identifier;
			}
		}

        public bool IsIntegratedSecurity
        {
            get
            {
                return m_bIntegratedSecurity;
            }
        }

    }
}
