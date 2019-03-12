using BridgeCare.ApplicationLog;
using BridgeCare.Interfaces;
using BridgeCare.Models;
using System;
using System.Data.SqlClient;
using System.Linq;

namespace BridgeCare.DataAccessLayer
{
    public class SectionLocator : ISectionLocator
    {
        public SectionLocator()
        {
        }

        public IQueryable<SectionLocationModel> Locate(SectionModel section, BridgeCareContext db)
        {
            var rawQueryForData = Enumerable.Empty<SectionLocationModel>();
            try
            {
                // Including isnull statments to change these to '0' to avoid an
                // exception as mapping a null to a non nullable data type
                // crashes the entity framwork
                var select = String.Format("SELECT Sectionid,isnull(Lat,0) as Latitude,isnull(Long,0) as Longitude FROM Segment_{0}_NS0 WHERE SectionId = {1}", section.NetworkId, section.SectionId);
                rawQueryForData = db.Database.SqlQuery<SectionLocationModel>(select);
            }
            catch (SqlException ex)
            {
                HandleException.SqlError(ex, "SectionLocator_");
            }
            catch (OutOfMemoryException ex)
            {
                HandleException.OutOfMemoryError(ex);
            }
            return rawQueryForData.AsQueryable();
        }
    }
}