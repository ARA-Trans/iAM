using BridgeCare.ApplicationLog;
using BridgeCare.Interfaces;
using BridgeCare.Models;
using System;
using System.Data.SqlClient;
using System.Linq;

namespace BridgeCare.DataAccessLayer
{
    public class SectionsDAL : ISections
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(SectionsDAL));
        public SectionsDAL()
        {
        }

        public IQueryable<SectionModel> GetSections(int networkId, BridgeCareContext db)
        {
            IQueryable<SectionModel> rawQueryForData = null;

            var select = String.Format("SELECT sectionid, facility as referenceKey, section as referenceId, {0} as networkId FROM Section_{0}", networkId);

            try
            {
                rawQueryForData = db.Database.SqlQuery<SectionModel>(select).AsQueryable();
            }
            catch (SqlException ex)
            {
                log.Error(ex.Message);
                HandleException.SqlError(ex, "Section_");
            }
            catch (OutOfMemoryException ex)
            {
                log.Error(ex.Message);
                HandleException.OutOfMemoryError(ex);
            }
            return rawQueryForData;
        }

        public int GetSectionId(int networkId, int brKey, BridgeCareContext db)
        {
            try
            {
                var sectionId = -1;
                // create sql connection
                var connection = new SqlConnection(db.Database.Connection.ConnectionString);
                connection.Open();
                // create sql parameter facilityParameter
                var facilityParameter = new SqlParameter()
                {
                    ParameterName = "@facility",
                    Value = brKey
                };
                // create query string
                var query = $"SELECT Sectionid FROM Section_{networkId} WHERE facility = @facility";
                // create sql command with query string and connection
                var sqlCommand = new SqlCommand(query, connection);
                // add facilityParameter as a parameter for the sql command
                sqlCommand.Parameters.Add(facilityParameter);
                // get data reader from sql command
                var dataReader = sqlCommand.ExecuteReader();
                // check that the data reader has returned rows and can execute a Read
                if (dataReader.HasRows && dataReader.Read())
                {
                    sectionId = dataReader.GetFieldValue<int>(0);
                }
                // close the data reader
                dataReader.Close();
                // close the connection
                connection.Close();
                // return sectionId
                return sectionId;
            }
            catch (SqlException ex)
            {
                log.Error(ex.Message);
                HandleException.SqlError(ex, "SectionId error");
            }
            return -1;
        }

        public int GetBrKey(int networkId, int sectionId, BridgeCareContext db)
        {
            try
            {
                var brKey = -1;
                // create sql connection
                var connection = new SqlConnection(db.Database.Connection.ConnectionString);
                connection.Open();
                // create sql parameter sectionIdParameter
                var sectionIdParameter = new SqlParameter()
                {
                    ParameterName = "@sectionId",
                    Value = sectionId
                };
                // create query string
                var query = $"SELECT facility FROM Section_{networkId} WHERE sectionId = @sectionId";
                // create sql command with query string and connection
                var sqlCommand = new SqlCommand(query, connection);
                // add sectionIdParameter as a parameter for the sql command
                sqlCommand.Parameters.Add(sectionIdParameter);
                // get data reader from sql command
                var dataReader = sqlCommand.ExecuteReader();
                // check that the data reader has returned rows and can execute a Read
                if (dataReader.HasRows && dataReader.Read())
                {
                    brKey = Int32.Parse(dataReader.GetFieldValue<string>(0));
                }
                // close the data reader
                dataReader.Close();
                // close the connection
                connection.Close();
                // return brKey
                return brKey;
            }
            catch (SqlException ex)
            {
                log.Error(ex.Message);
                HandleException.SqlError(ex, "SectionId error");
            }
            return -1;
        }
    }
}