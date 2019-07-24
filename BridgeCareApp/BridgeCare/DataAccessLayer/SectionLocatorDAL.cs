using BridgeCare.ApplicationLog;
using BridgeCare.Interfaces;
using BridgeCare.Models;
using System;
using System.Data.SqlClient;
using System.Linq;

namespace BridgeCare.DataAccessLayer
{
    public class SectionLocatorDAL : ISectionLocator
    {
        public SectionLocatorDAL()
        {
        }

        public SectionLocationModel Locate(SectionModel section, BridgeCareContext db)
        {
            try
            {
                var sectionLocationModel = new SectionLocationModel();
                // Including isnull statments to change these to '0' to avoid an
                // exception as mapping a null to a non nullable data type
                // crashes the entity framwork
                var query =
                    $"SELECT Sectionid, isnull(Lat,0) as Latitude, isnull(Long,0) as Longitude FROM Segment_{section.NetworkId}_NS0 WHERE SectionId = @sectionId";
                // create and open db connection
                var connection = new SqlConnection(db.Database.Connection.ConnectionString);
                connection.Open();
                // create a sql command with the query and connection
                var sqlCommand = new SqlCommand(query, connection);
                // add parameter to sqlCommand
                sqlCommand.Parameters.Add(new SqlParameter()
                {
                    ParameterName = "@sectionId",
                    Value = section.SectionId
                });
                // create data reader from the sql command
                var dataReader = sqlCommand.ExecuteReader();
                // check if data reader has rows and can execute a Read
                if (dataReader.HasRows && dataReader.Read())
                {
                    // get the section location details
                    sectionLocationModel.SectionId = dataReader.GetFieldValue<int>(0);
                    sectionLocationModel.Latitude = dataReader.GetFieldValue<double>(1);
                    sectionLocationModel.Longitude = dataReader.GetFieldValue<double>(2);
                }
                // close the data reader
                dataReader.Close();
                // close the connection
                connection.Close();
                // return the sectionLocationModel
                return sectionLocationModel;
            }
            catch (SqlException ex)
            {
                HandleException.SqlError(ex, "SectionLocator_");
            }
            catch (OutOfMemoryException ex)
            {
                HandleException.OutOfMemoryError(ex);
            }
            catch (Exception ex)
            {
                HandleException.GeneralError(ex);
            }
            return new SectionLocationModel();
        }
    }
}