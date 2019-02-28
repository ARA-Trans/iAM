using BridgeCare.ApplicationLog;
using BridgeCare.Interfaces;
using BridgeCare.Models;
using BridgeCare.Utility;
using BridgeCare.DataAccessLayer.DataAdapters;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace BridgeCare.DataAccessLayer
{
    public class AttributeByYear : IAttributeByYear
    {
        private readonly BridgeCareContext db;

        public AttributeByYear(BridgeCareContext context)
        {
            db = context ?? throw new ArgumentNullException(nameof(context));
        }

        public List<AttributeByYearModel> GetProjectedAttributes(SimulatedSegmentAddressModel segmentAddressModel, BridgeCareContext db)
        {
            if (segmentAddressModel == null)
                return new List<AttributeByYearModel>();

            var selectStatement = String.Format("SELECT * FROM SIMULATION_{0}_{1} WHERE SectionID={2}",
                segmentAddressModel.NetworkId, segmentAddressModel.SimulationId, segmentAddressModel.SectionId);

            return GeneralAttributeValueQuery(selectStatement);
        }

        public List<AttributeByYearModel> GetHistoricalAttributes(SectionModel sectionModel, BridgeCareContext db)
        {
            if (sectionModel == null)
                return new List<AttributeByYearModel>();

            var selectStatement = String.Format(
                "SELECT * FROM SEGMENT_{0}_NS0 WHERE SectionID={1}",
                sectionModel.NetworkId, sectionModel.SectionId);

            return GeneralAttributeValueQuery(selectStatement);
        }

        public List<AttributeByYearModel> GeneralAttributeValueQuery(string selectStatement)
        {
            try
            {
                DataTable queryReturnValues = UtilityFunctions.NonEntitySQLQuery(selectStatement, db);
                return new AttributeYearDataTable(queryReturnValues).Fetch();
            }
            catch (SqlException ex)
            {
                HandleException.SqlError(ex, "Query:" + selectStatement);
            }
            catch (OutOfMemoryException ex)
            {
                HandleException.OutOfMemoryError(ex);
            }

            return new List<AttributeByYearModel>();
        }

       
    }
}