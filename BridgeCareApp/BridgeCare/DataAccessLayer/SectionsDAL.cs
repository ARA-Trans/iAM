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
        public SectionsDAL()
        {
        }

        public IQueryable<SectionModel> GetSections(NetworkModel data, BridgeCareContext db)
        {
            IQueryable<SectionModel> rawQueryForData = null;

            var select = String.Format("SELECT Sectionid,Facility as referenceKey,Section as referenceID,{0} as Networkid FROM Section_{0}", data.NetworkId);

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

        public int GetSectionId(int networkId, int brKey, BridgeCareContext db)
        {
            try
            {
                var select = String.Format("SELECT Sectionid FROM Section_{0} WHERE facility = {1}", networkId, brKey);

                int ret = db.Database.SqlQuery<int>(select).SingleOrDefault();

                return ret;
            }
            catch (SqlException ex)
            {
                HandleException.SqlError(ex, "SectionId error");
            }
            return -1;
        }

        public int GetBrKey(int networkId, int sectionID, BridgeCareContext db)
        {
            try
            {
                var select = String.Format("SELECT facility FROM Section_{0} WHERE sectionId = {1}", networkId, sectionID);

                string returnRaw = db.Database.SqlQuery<string>(select).SingleOrDefault();

                //this is most certainly an int if it is valid,
                //it functions as a key in the SIMULATION_XX_YY tables
                return Convert.ToInt32(returnRaw);
            }
            catch (SqlException ex)
            {
                HandleException.SqlError(ex, "SectionId error");
            }
            return -1;
        }
    }
}