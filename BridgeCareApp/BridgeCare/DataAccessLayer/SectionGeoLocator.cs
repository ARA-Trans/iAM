using BridgeCare.ApplicationLog;
using BridgeCare.Interfaces;
using BridgeCare.Models;
using System;
using System.Data.SqlClient;
using System.Linq;

namespace BridgeCare.DataAccessLayer
{
    public class SectionGeoLocator : ISectionGeoLocator
    {
        public SectionGeoLocator()
        {
        }

        public IQueryable<LatitudeLongitudeSectionModel> GetLatitudeLongitude(int NetworkId, BridgeCareContext db)
        {
            IQueryable<LatitudeLongitudeSectionModel> rawQueryForData = null;

            // Including isnull statments to change these to '0' to avoid an exception
            // as  mapping a null to a non nullable data type crashes the entity framwork

            var select = String.Format("SELECT Sectionid,isnull(Lat,0) as Latitude,isnull(Long,0) as Longitude FROM Segment_{0}_NS0", NetworkId);

            try
            {
                rawQueryForData = db.Database.SqlQuery<LatitudeLongitudeSectionModel>(select).AsQueryable();
            }
            catch (SqlException ex)
            {
                HandleException.SqlError(ex, "Section_");
            }
            catch (OutOfMemoryException ex)
            {
                HandleException.OutOfMemoryError(ex);
            }
            return rawQueryForData;
        }
    }
}