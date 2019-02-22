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
    public class Sections : ISections
    {
        public Sections() { }
        public IQueryable<SectionModel> GetSections(NetworkModel data, BridgeCareContext db)
        {
            IQueryable <SectionModel> rawQueryForData = null;

            var select = String.Format("SELECT Sectionid,Facility as BridgeID,Section as BridgeKey,{0} as Networkid FROM Section_{0}", data.NetworkId);

            try
            {
                rawQueryForData = db.Database.SqlQuery<SectionModel>(select).AsQueryable();
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