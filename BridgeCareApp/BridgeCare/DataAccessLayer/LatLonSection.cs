using BridgeCare.ApplicationLog;
using BridgeCare.Interfaces;
using BridgeCare.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace BridgeCare.DataAccessLayer
{
    public class LatLonSection : ILatLonSection
    {
        public LatLonSection() { }
        public IQueryable<LatLonSectionModel> GetLatLon(int NetworkId, BridgeCareContext db)
        {
            IQueryable<LatLonSectionModel> rawQueryForData = null;

            // bug in program: Long is null in rolled up DB even whe the source DB has valid data, including isnull statments to change these to'0'
            // if not the mapping to a non nullable data type crashes the entity framwork
            var select = String.Format("SELECT Sectionid,isnull(Lat,0) Lat,isnull(Long,0) Long FROM Segment_{0}_NS0", NetworkId);

            try
            {
                rawQueryForData = db.Database.SqlQuery<LatLonSectionModel>(select).AsQueryable();

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