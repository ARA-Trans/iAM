using System;
using System.Collections.Generic;
using System.Text;
using System.EnterpriseServices;
using System.Data.SqlClient;
using System.Data.OleDb;
using System.Data;


namespace DatabaseManager
{
    public class Transaction : IDbTransaction
    {
        private SqlTransaction sqlTransaction;
        private OleDbTransaction oleTransaction;

        private SqlConnection sqlConn = null;
        private OleDbConnection oleConn = null;

        private bool bUseSql = false;

        public Transaction(object oConn, bool bIsOleDb)
        {
            if (bIsOleDb)
            {
                bUseSql = false;
                oleConn = (OleDbConnection)oConn;
            }
            else
            {
                bUseSql = true;
                sqlConn = (SqlConnection)oConn;
            }
        }

        public void BeginTransaction()
        {
            if (sqlConn != null)
            {
                SqlCommand sqlComm = sqlConn.CreateCommand();
                sqlTransaction = sqlConn.BeginTransaction();
                sqlComm.Transaction = sqlTransaction;
            }
            else
            {
                oleConn.CreateCommand();
                oleTransaction = oleConn.BeginTransaction();
            }
        }

        #region IDbTransaction Members

        public void Commit()
        {
            if (bUseSql) { sqlTransaction.Commit(); }
            else { oleTransaction.Commit(); }
        }

        public IDbConnection Connection
        {
            get 
            { 
                if (bUseSql) 
                { 
                    return sqlTransaction.Connection; 
                } 
                else 
                { 
                    return oleTransaction.Connection;
                } 
            }
        }

        public IsolationLevel IsolationLevel
        {
            get 
            {
                if (bUseSql)
                {
                    return sqlTransaction.IsolationLevel;
                }
                else
                {
                    return oleTransaction.IsolationLevel;
                }
            }
        }

        public void Rollback()
        {
            if (bUseSql)
            {
                sqlTransaction.Rollback();
            }
            else
            {
                oleTransaction.Rollback();
            }
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            if (bUseSql)
            {
                sqlTransaction.Dispose();
            }
            else
            {
                oleTransaction.Dispose();
            }
        }

        #endregion
    }
}
