using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.OleDb;
using System.Data.Sql;
using System.Data.SqlClient;

namespace DatabaseManager
{
    public class DataReader// : IDisposable
    {
        private SqlDataReader SDR = null;
        private OleDbDataReader ODR = null;
		//private SqlConnection sqlConn = null;
		//private OleDbConnection oleDbConn = null;
		public DataReader(String strQuery) : this( strQuery, DBMgr.NativeConnectionParameters )
		{
		}

        public DataReader(String strQuery, ConnectionParameters cp)
        {
            if ( cp.IsOleDBConnection )
            {
				try
				{
					OleDbConnection readerConn = new OleDbConnection( cp.ConnectionString );
					readerConn.Open();
					OleDbCommand command = new OleDbCommand( strQuery, readerConn );
					command.CommandTimeout = 500000;
					ODR = command.ExecuteReader( CommandBehavior.Default );
				}
				catch( OleDbException oleE )
				{
					System.Diagnostics.Debug.WriteLine( oleE.Message );
					throw oleE;
				}
			}
            else
            {
				//try
				//{
					SqlConnection readerConn = new SqlConnection( cp.ConnectionString );
					readerConn.Open();
					SqlCommand command = new SqlCommand( strQuery, readerConn );
					command.CommandTimeout = 500000;
					SDR = command.ExecuteReader( CommandBehavior.CloseConnection );
				//}
                //catch( Exception sqlE )
                //{
                //    //System.Diagnostics.Debug.WriteLine( sqlE.Message );
                //    //throw sqlE;
                //}

            }
        }

		//~DataReader()
		//{
		//    Dispose( false );
		//}

		#region IDisposable Members

		//public void Dispose()
		//{
		//    Dispose( true );
		//    GC.SuppressFinalize( this );
		//}

		//protected virtual void Dispose( bool cleanManaged )
		//{
		//    if( cleanManaged )
		//    {
		//        if( sqlConn != null )
		//        {
		//            sqlConn.Close();
		//            sqlConn.Dispose();
		//            SDR.Dispose();
		//        }
		//        if( oleDbConn != null )
		//        {
		//            oleDbConn.Close();
		//            oleDbConn.Dispose();
		//            ODR.Dispose();
		//        }
		//    }
		//}

		public void Dispose()
		{
			if (SDR != null)
			{
				SDR.Dispose();
			}
			else
			{
				ODR.Dispose();
			}
		}

		#endregion
        #region IDataReader Members

        public void Close()
        {
            if (SDR != null) { SDR.Close(); }
            else { ODR.Close(); }
        }

        public int Depth
        {
            get
            {
                if (SDR != null) { return SDR.Depth; }
                else { return ODR.Depth; }
            }
        }

        public DataTable GetSchemaTable()
        {
            if (SDR != null) { return SDR.GetSchemaTable(); }
            else { return ODR.GetSchemaTable(); }
        }

        public bool IsClosed
        {
            get
            {
                if (SDR != null) { return SDR.IsClosed; }
                else { return ODR.IsClosed; }
            }
        }

        public bool NextResult()
        {
            if (SDR != null) { return SDR.NextResult(); }
            else { return ODR.NextResult(); }
        }

        public bool Read()
        {
            if (SDR != null) 
            {
                return SDR.Read();
            }
            else { return ODR.Read(); }
        }

        public int RecordsAffected
        {
            get
            {
                if (SDR != null) { return SDR.RecordsAffected; }
                else { return ODR.RecordsAffected; }
            }
        }

        #endregion

        #region IDataRecord Members

        public int FieldCount
        {
            get
            {
                if (SDR != null) { return SDR.FieldCount; }
                else { return ODR.FieldCount; }
            }
        }

        public bool GetBoolean(int i)
        {
            if (SDR != null) { return SDR.GetBoolean(i); }
            else { return ODR.GetBoolean(i); }
        }

        public byte GetByte(int i)
        {
            if (SDR != null) { return SDR.GetByte(i); }
            else { return ODR.GetByte(i); }
        }

        public long GetBytes(int i, long fieldOffset, byte[] buffer, int bufferoffset, int length)
        {
            if (SDR != null) { return SDR.GetBytes(i, fieldOffset, buffer, bufferoffset, length); }
            else { return ODR.GetBytes(i, fieldOffset, buffer, bufferoffset, length); }
        }

        public char GetChar(int i)
        {
            if (SDR != null) { return SDR.GetChar(i); }
            else { return ODR.GetChar(i); }
        }

        public long GetChars(int i, long fieldoffset, char[] buffer, int bufferoffset, int length)
        {
            if (SDR != null) { return SDR.GetChars(i, fieldoffset, buffer, bufferoffset, length); }
            else { return ODR.GetChars(i, fieldoffset, buffer, bufferoffset, length); }
        }

        public IDataReader GetData(int i)
        {
            if (SDR != null) { return SDR.GetData(i); }
            else { return ODR.GetData(i); }
        }

        public string GetDataTypeName(int i)
        {
            if (SDR != null) { return SDR.GetDataTypeName(i); }
            else { return ODR.GetDataTypeName(i); }
        }

        public DateTime GetDateTime(int i)
        {
            if (SDR != null) { return SDR.GetDateTime(i); }
            else { return ODR.GetDateTime(i); }
        }

        public decimal GetDecimal(int i)
        {
            if (SDR != null) { return SDR.GetDecimal(i); }
            else { return ODR.GetDecimal(i); }
        }

        public double GetDouble(int i)
        {
            if (SDR != null) { return SDR.GetDouble(i); }
            else { return ODR.GetDouble(i); }
        }

        public Type GetFieldType(int i)
        {
            if (SDR != null) { return SDR.GetFieldType(i); }
            else { return ODR.GetFieldType(i); }
        }

        public float GetFloat(int i)
        {
            if (SDR != null) { return SDR.GetFloat(i); }
            else { return ODR.GetFloat(i); }
        }

        public Guid GetGuid(int i)
        {
            if (SDR != null) { return SDR.GetGuid(i); }
            else { return ODR.GetGuid(i); }
        }

        public short GetInt16(int i)
        {
            if (SDR != null) { return SDR.GetInt16(i); }
            else { return ODR.GetInt16(i); }
        }

        public int GetInt32(int i)
        {
            if (SDR != null) { return SDR.GetInt32(i); }
            else { return ODR.GetInt32(i); }
        }

        public long GetInt64(int i)
        {
            if (SDR != null) { return SDR.GetInt64(i); }
            else { return ODR.GetInt64(i); }
        }

        public string GetName(int i)
        {
            if (SDR != null) { return SDR.GetName(i); }
            else { return ODR.GetName(i); }
        }

        public int GetOrdinal(string name)
        {
            if (SDR != null) { return SDR.GetOrdinal(name); }
            else { return ODR.GetOrdinal(name); }
        }

        public string GetString(int i)
        {
            if (SDR != null) { return SDR.GetString(i); }
            else { return ODR.GetString(i); }
        }

        public object GetValue(int i)
        {
            if (SDR != null) { return SDR.GetValue(i); }
            else { return ODR.GetValue(i); }
        }

        public int GetValues(object[] values)
        {
            if (SDR != null) { return SDR.GetValues(values); }
            else { return ODR.GetValues(values); }
        }

        public bool IsDBNull(int i)
        {
            if (SDR != null) { return SDR.IsDBNull(i); }
            else { return ODR.IsDBNull(i); }
        }

        public object this[string name]
        {
            get
            {
                if (SDR != null) { return SDR[name]; }
                else { return ODR[name]; }
            }
        }

        public object this[int i]
        {
            get
            {
                if (SDR != null) { return SDR[i]; }
                else { return ODR[i]; }
            }
        }

        #endregion
    }
}
