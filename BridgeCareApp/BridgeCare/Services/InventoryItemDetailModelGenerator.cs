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
            var currentConditionDurationColumns = new List<string> { "DECK", "SUP", "SUB", "CULV" };
            // TODO Need more info on it.
            inventoryItemDetailModel.CurrentConditionDuration = new List<ConditionDuration>();
            inventoryItemDetailModel.PreviousConditionDuration = new List<ConditionDuration>();
        }

        private void AddRoadwayInfo(InventoryItemDetailModel inventoryItemDetailModel, List<InventoryItemModel> inventoryItems)
        {
            var roadwayInfoColumns = new List<string> { "ADTTOTAL", "FUNC_CLASS", "NHS_IND" };

            inventoryItemDetailModel.RoadwayInfo = CreateLabelValues(inventoryItems, roadwayInfoColumns);
        }

        private void AddPosting(InventoryItemDetailModel inventoryItemDetailModel, List<InventoryItemModel> inventoryItems)
        {
            var postingColumns = new List<string> { "POST_STATUS_DATE", "POST_STATUS2", "SPEC_RESTRICT_POST", "SINGLE" };

            inventoryItemDetailModel.Posting = CreateLabelValues(inventoryItems, postingColumns);
        }

        private void AddNbiLoadRating(InventoryItemDetailModel inventoryItemDetailModel, List<InventoryItemModel> inventoryItems)
        {
            var nbiLoadRatingColumns = new List<string> { };
            // TODO Need more info on it.
            inventoryItemDetailModel.NbiLoadRating = new List<LabelValue>();
        }

        private void AddSpanInformation(InventoryItemDetailModel inventoryItemDetailModel, List<InventoryItemModel> inventoryItems)
        {
            var spanInformationColumns = new List<string> { "NUMBER_SPANS", "LENGTH", "DECK_AREA", };

            inventoryItemDetailModel.SpanInformation = CreateLabelValues(inventoryItems, spanInformationColumns);
        }

        private void AddDeckInformation(InventoryItemDetailModel inventoryItemDetailModel, List<InventoryItemModel> inventoryItems)
        {
            var deckInformationColumns = new List<string> { "DECK_WIDTH" };

            inventoryItemDetailModel.DeckInformation = CreateLabelValues(inventoryItems, deckInformationColumns);
        }

        private void AddManagement(InventoryItemDetailModel inventoryItemDetailModel, List<InventoryItemModel> inventoryItems)
        {
            var managementColumns = new List<string> { "OWNER_CODE", "MPO", "SUBM_AGENCY", "NBISLEN", "BUS_PLAN_NETWORK" };

            inventoryItemDetailModel.Management = CreateLabelValues(inventoryItems, managementColumns);
        }

        private void AddAgeService(InventoryItemDetailModel inventoryItemDetailModel, List<InventoryItemModel> inventoryItems)
        {
            var ageAndServiceColumns = new List<string> { "YEAR_BUILT", "YEAR_RECON" };            

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