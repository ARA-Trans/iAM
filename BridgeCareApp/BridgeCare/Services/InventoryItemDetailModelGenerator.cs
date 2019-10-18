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
            var inventoryNbiLoadRatings = inventoryModel.InventoryNbiLoadRatings;
            var inventoryItemDetailModel = new InventoryItemDetailModel();

            try
            {
                AddLocation(inventoryItemDetailModel, inventoryItems);
                AddAgeService(inventoryItemDetailModel, inventoryItems);
                AddManagement(inventoryItemDetailModel, inventoryItems);
                AddDeckInformation(inventoryItemDetailModel, inventoryItems);
                AddSpanInformation(inventoryItemDetailModel, inventoryItems);
                AddNbiLoadRating(inventoryItemDetailModel, inventoryNbiLoadRatings);
                AddPosting(inventoryItemDetailModel, inventoryItems);
                AddRoadwayInfo(inventoryItemDetailModel, inventoryItems);
                AddCurrentConditionDuration(inventoryItemDetailModel, inventoryItems);
                AddRiskScores(inventoryItemDetailModel);
                AddOperatingInventoryRating(inventoryItemDetailModel, inventoryItems);                  
            }
            catch (Exception ex)
            {
                HandleException.GeneralError(ex);
            }

            return inventoryItemDetailModel;
        }

        private void AddOperatingInventoryRating(InventoryItemDetailModel inventoryItemDetailModel, List<InventoryItemModel> inventoryItems)
        {
            var operatingRatingInventoryRatingGrouping = new OperatingRatingInventoryRatingGrouping();
            AddRatingRows(operatingRatingInventoryRatingGrouping, inventoryItems);
            AddMinRatioLegalLoad(operatingRatingInventoryRatingGrouping, inventoryItems);         
            inventoryItemDetailModel.OperatingRatingInventoryRatingGrouping = operatingRatingInventoryRatingGrouping;
        }

        private void AddMinRatioLegalLoad(OperatingRatingInventoryRatingGrouping operatingRatingInventoryRatingGrouping, List<InventoryItemModel> inventoryItems)
        {
            var minRatioColumns = new List<string> { "MIN_RATIO" };
            operatingRatingInventoryRatingGrouping.MinRatioLegalLoad = CreateLabelValues(inventoryItems, minRatioColumns).FirstOrDefault();
        }

        private void AddRatingRows(OperatingRatingInventoryRatingGrouping operatingRatingInventoryRatingGrouping, List<InventoryItemModel> inventoryItems)
        {
            var hs20Columns = new List<string> { "HS20_OR", "HS20_IR", "HS20_RATIO" };
            var h20Columns = new List<string> { "H20_OR", "H20_IR", "H20_RATIO" };
            var ml80Columns = new List<string> { "ML80_OR", "ML80_IR", "ML80_RATIO" };
            var tk527Columns = new List<string> { "TK527_OR", "TK527_IR", "TK527_RATIO" };

            var ratingRows = new List<OperatingRatingInventoryRatingRow>();
            AddRatingRow(hs20Columns, ratingRows, inventoryItems);
            AddRatingRow(h20Columns, ratingRows, inventoryItems);
            AddRatingRow(ml80Columns, ratingRows, inventoryItems);
            AddRatingRow(tk527Columns, ratingRows, inventoryItems);
            operatingRatingInventoryRatingGrouping.RatingRows = ratingRows;
        }

        private void AddRatingRow(List<string> hs20Columns, List<OperatingRatingInventoryRatingRow> ratingRows, List<InventoryItemModel> inventoryItems)
        {
            var labelValues = CreateLabelValues(inventoryItems, hs20Columns);
            ratingRows.Add(new OperatingRatingInventoryRatingRow { OperatingRating = labelValues[0], InventoryRating = labelValues[1], RatioLegalLoad = labelValues[2] });
        }

        private void AddRiskScores(InventoryItemDetailModel inventoryItemDetailModel)
        {
            //TODO Risk scores: currently const 0 assigned as per UI
            inventoryItemDetailModel.RiskScores = new RiskScores { Old = 0, New = 0 };
        }

        private void AddCurrentConditionDuration(InventoryItemDetailModel inventoryItemDetailModel, List<InventoryItemModel> inventoryItems)
        {
            var currentConditionColumns = new List<string> { "DECK", "SUP", "SUB", "CULV" };
            var currentDurationColumns = new List<string> { "DECK_DUR", "SUP_DUR", "SUB_DUR", "CULV_DUR" };            
            inventoryItemDetailModel.CurrentConditionDuration = CreateConditionDurationRows(inventoryItems, currentConditionColumns, currentDurationColumns);
            var priorConditionColumns = new List<string> { "PREV_DECK", "PREV_SUP", "PREV_SUB", "PREV_CULV" };
            var priorDurationColumns = new List<string> { "PREV_DECK_DUR", "PREV_SUP_DUR", "PREV_SUB_DUR", "PREV_CULV_DUR" };
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
            var roadwayInfoColumns = new List<string> { "ADTTOTAL", "FUNC_CLASS", "VCLROVER", "VCLROVER", "NHS_IND" };
            inventoryItemDetailModel.RoadwayInfo = CreateLabelValues(inventoryItems, roadwayInfoColumns);
        }

        private void AddPosting(InventoryItemDetailModel inventoryItemDetailModel, List<InventoryItemModel> inventoryItems)
        {
            var postingColumns = new List<string> { "POST_STATUS_DATE", "POST_STATUS2", "SPEC_RESTRICT_POST", "SINGLE" };
            inventoryItemDetailModel.Posting = CreateLabelValues(inventoryItems, postingColumns);
        }

        private void AddNbiLoadRating(InventoryItemDetailModel inventoryItemDetailModel, List<InventoryNbiLoadRatingModel> inventoryNbiLoadRatings)
        {
            var nbiLoadRatings = new List<NbiLoadRating>();
            var nbiLoadRatingColumns = new List<string> { "LOAD_TYPE", "NBI", "INV_RATING_TON", "OPR_RATING_TON", "SLC_RATING_FACTOR", "IR_RATING_FACTOR", "OR_RATING_FACTOR", "RATING_DATASET" };
            foreach (var inventoryNbiLoadRating in inventoryNbiLoadRatings)
            {
                var nbiLoadRating = new NbiLoadRating { NbiLoadRatingRow = CreateLabelValues(inventoryNbiLoadRating.NbiLoadRatingItems, nbiLoadRatingColumns) };

                nbiLoadRatings.Add(nbiLoadRating);
            }
            inventoryItemDetailModel.NbiLoadRatings = nbiLoadRatings;
        }

        private void AddSpanInformation(InventoryItemDetailModel inventoryItemDetailModel, List<InventoryItemModel> inventoryItems)
        {
            var spanInformationColumns = new List<string> { "NUMBER_SPANS", "MATERIALMAIN", "DESIGNMAIN", "MATERIALAPPR", "DESIGNAPPR", "LENGTH", "DECK_AREA", "TOT_LENGTH", "MAIN_FC_GROUP_NUM", "APPR_FC_GROUP_NUM" };
            inventoryItemDetailModel.SpanInformation = CreateLabelValues(inventoryItems, spanInformationColumns);
        }

        private void AddDeckInformation(InventoryItemDetailModel inventoryItemDetailModel, List<InventoryItemModel> inventoryItems)
        {
            var deckInformationColumns = new List<string> { "DKSTRUCTYP", "DEPT_DKSTRUCTYP", "DKSURFTYPE", "DKMEMBTYPE", "DKPROTECT", "DECK_WIDTH", "SKEW" };
            inventoryItemDetailModel.DeckInformation = CreateLabelValues(inventoryItems, deckInformationColumns);
        }

        private void AddManagement(InventoryItemDetailModel inventoryItemDetailModel, List<InventoryItemModel> inventoryItems)
        {
            var managementColumns = new List<string> { "CUSTODIAN", "OWNER_CODE", "MPO", "SUBM_AGENCY", "NBISLEN", "HISTSIGN", "CRGIS_SHPOKEY_NUM", "BUS_PLAN_NETWORK" };
            inventoryItemDetailModel.Management = CreateLabelValues(inventoryItems, managementColumns);
        }

        private void AddAgeService(InventoryItemDetailModel inventoryItemDetailModel, List<InventoryItemModel> inventoryItems)
        {
            var ageAndServiceColumns = new List<string> { "YEAR_BUILT", "YEAR_RECON", "SERVTYPON", "SERVTYPUND" }; 
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