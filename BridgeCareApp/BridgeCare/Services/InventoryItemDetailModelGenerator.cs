using BridgeCare.ApplicationLog;
using BridgeCare.Interfaces;
using BridgeCare.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BridgeCare.Services
{
    public class InventoryItemDetailModelGenerator: IInventoryItemDetailModelGenerator
    {
        /// <summary>
        /// Generate InventoryItemDetailModel
        /// </summary>        
        /// <param name="inventoryModel"></param>
        /// <returns></returns>
        public InventoryItemDetailModel MakeInventoryItemDetailModel(InventoryModel inventoryModel)
        {
            var inventoryItems = inventoryModel.InventoryItems;
            var inventoryItemDetailModel = new InventoryItemDetailModel();

            try
            {
                AddLocation(inventoryItemDetailModel, inventoryItems);
                AddAgeService(inventoryItemDetailModel, inventoryItems);
                AddManagement(inventoryItemDetailModel, inventoryItems);
                AddDeckInformation(inventoryItemDetailModel, inventoryItems);
                AddSpanInformation(inventoryItemDetailModel, inventoryItems);
                AddNbiLoadRating(inventoryItemDetailModel, inventoryItems);
                AddPosting(inventoryItemDetailModel, inventoryItems);
                AddRoadwayInfo(inventoryItemDetailModel, inventoryItems);
                AddCurrentConditionDuration(inventoryItemDetailModel, inventoryItems);
                AddRiskScores(inventoryItemDetailModel);
                AddOperatingInventoryRating(inventoryItemDetailModel);                  
            }
            catch (Exception ex)
            {
                HandleException.GeneralError(ex);
            }

            return inventoryItemDetailModel;
        }

        private void AddOperatingInventoryRating(InventoryItemDetailModel inventoryItemDetailModel)
        {
            // TODO Operating Rating (OR) vs Inventory Rating (IR)
            inventoryItemDetailModel.OperatingRatingInventoryRatingGrouping = new OperatingRatingInventoryRatingGrouping { RatingRows = new List<OperatingRatingInventoryRatingRow>() { }, MinRatioLegalLoad = new LabelValue(string.Empty, string.Empty) };
        }

        private void AddRiskScores(InventoryItemDetailModel inventoryItemDetailModel)
        {
            //TODO Risk scores: currently const 0 assigned as per UI
            inventoryItemDetailModel.RiskScores = new RiskScores { Old = 0, New = 0 };
        }

        private void AddCurrentConditionDuration(InventoryItemDetailModel inventoryItemDetailModel, List<InventoryItemModel> inventoryItems)
        {
            var currentConditionColumns = new List<string> { "DECK", "SUP", "SUB", "CULV" };
            var currentDurationColumns = new List<string> { "DECK_D", "SUP_D", "SUB_D", "CULV_D" };            
            inventoryItemDetailModel.CurrentConditionDuration = CreateConditionDurationRows(inventoryItems, currentConditionColumns, currentDurationColumns);
            var priorConditionColumns = new List<string> { "PRIOR_DECK_C", "PRIOR_SUP_C", "PRIOR_SUB_C", "PRIOR_CULV_C" };
            var priorDurationColumns = new List<string> { "PRIOR_DECK_D", "PRIOR_SUP_D", "PRIOR_SUB_D", "PRIOR_CULV_D" };
            inventoryItemDetailModel.PreviousConditionDuration = CreateConditionDurationRows(inventoryItems, priorConditionColumns, priorDurationColumns);
        }

        private List<ConditionDuration> CreateConditionDurationRows(List<InventoryItemModel> inventoryItems, List<string> conditionColumns, List<string> durationColumns)
        {
            var conditionDurationRows = new List<ConditionDuration>();
            for (var index = 0; index < conditionColumns.Count; index++)
            {
                var inventoryItem = inventoryItems.FirstOrDefault(i => i.ColumnName == conditionColumns[index]);
                var conditionDuration = new ConditionDuration { Condition = inventoryItem.DisplayValue, Name = inventoryItem.ViewName };
                inventoryItem = inventoryItems.FirstOrDefault(i => i.ColumnName == durationColumns[index]);
                conditionDuration.Duration = inventoryItem.DisplayValue;
                conditionDurationRows.Add(conditionDuration);
            }

            return conditionDurationRows;
        }

        private void AddRoadwayInfo(InventoryItemDetailModel inventoryItemDetailModel, List<InventoryItemModel> inventoryItems)
        {
            var roadwayInfoColumns = new List<string> { "ADTTOTAL", "FUNC_CLASS", "OVER_STREET_CLEARANCE", "UNDER_CLEARANCE", "NHS_IND" };
            inventoryItemDetailModel.RoadwayInfo = CreateLabelValues(inventoryItems, roadwayInfoColumns);
        }

        private void AddPosting(InventoryItemDetailModel inventoryItemDetailModel, List<InventoryItemModel> inventoryItems)
        {
            var postingColumns = new List<string> { "POST_STATUS_DATE", "POST_STATUS2", "SPEC_RESTRICT_POST", "SINGLE" };
            inventoryItemDetailModel.Posting = CreateLabelValues(inventoryItems, postingColumns);
        }

        private void AddNbiLoadRating(InventoryItemDetailModel inventoryItemDetailModel, List<InventoryItemModel> inventoryItems)
        {
            var nbiLoadRatingColumns = new List<string> { "LOAD_TYPE", "NBI", "INV_RATING_TON", "OPR_RATING_TON", "SLC_RATING_FACTOR", "IR_RATING_FACTOR", "OR_RATING_FACTOR", "RATING_DATASET" };
            inventoryItemDetailModel.NbiLoadRating = CreateLabelValues(inventoryItems, nbiLoadRatingColumns);
        }

        private void AddSpanInformation(InventoryItemDetailModel inventoryItemDetailModel, List<InventoryItemModel> inventoryItems)
        {
            var spanInformationColumns = new List<string> { "NUMBER_SPANS", "MAIN_SPAN_MATERIAL", "MAIN_SPAN_DESIGN", "APPROACH_SPAN_MATERIAL", "APPROACH_SPAN_DESIGN", "MAXIMUM_SPAN_LENGTH", "LENGTH", "DECK_AREA", "TOTAL_LENGTH", "FC_GROUP_NUMBER_MAIN", "FC_GROUP_NUMBER_APPROACH" };
            inventoryItemDetailModel.SpanInformation = CreateLabelValues(inventoryItems, spanInformationColumns);
        }

        private void AddDeckInformation(InventoryItemDetailModel inventoryItemDetailModel, List<InventoryItemModel> inventoryItems)
        {
            var deckInformationColumns = new List<string> { "DECK_STRUCTURE_TYPE", "DECK_SHEET_TYPE_PENNDOT", "DECK_SURFACE_TYPE", "DECK_MEMBRANE_TYPE", "DECK_PROTECTION", "DECK_WIDTH", "SKEW" };
            inventoryItemDetailModel.DeckInformation = CreateLabelValues(inventoryItems, deckInformationColumns);
        }

        private void AddManagement(InventoryItemDetailModel inventoryItemDetailModel, List<InventoryItemModel> inventoryItems)
        {
            var managementColumns = new List<string> { "MAINT_RESP", "OWNER_CODE", "MPO", "REPORT_GROUP", "SUBM_AGENCY", "NBISLEN", "HISTSIGN", "SHP_KEY_NUMBER", "BUS_PLAN_NETWORK" };
            inventoryItemDetailModel.Management = CreateLabelValues(inventoryItems, managementColumns);
        }

        private void AddAgeService(InventoryItemDetailModel inventoryItemDetailModel, List<InventoryItemModel> inventoryItems)
        {
            var ageAndServiceColumns = new List<string> { "YEAR_BUILT", "YEAR_RECON", "TYPE_OF_SERVICE_ON", "TYPE_OF_SERVICE_UNDER" }; 
            inventoryItemDetailModel.AgeAndService = CreateLabelValues(inventoryItems, ageAndServiceColumns);
        }

        private void AddLocation(InventoryItemDetailModel inventoryItemDetailModel, List<InventoryItemModel> inventoryItems)
        {
            var locationColumns = new List<string> { "DISTRICT", "COUNTY", "MUNI_CODE", "FEATURE_INTERSECTED", "FEATURE_CARRIED", "LOCATION" };
            const string locationId = "5A02";
            inventoryItemDetailModel.Location = CreateLabelValues(inventoryItems, locationColumns);
            inventoryItemDetailModel.Name = inventoryItemDetailModel.Location.FirstOrDefault(l => l.Label.StartsWith(locationId)).Value;
            inventoryItemDetailModel.Label = locationId;
        }

        private List<LabelValue> CreateLabelValues(List<InventoryItemModel> inventoryItems, List<string> columns)
        {
            var labelValues = new List<LabelValue>();
            foreach (var column in columns)
            {
                var inventoryItem = inventoryItems.FirstOrDefault(i => i.ColumnName == column);
                labelValues.Add(new LabelValue(inventoryItem.Id + " " + inventoryItem.ViewName, inventoryItem.DisplayValue));
            }

            return labelValues;
        }
    }
}