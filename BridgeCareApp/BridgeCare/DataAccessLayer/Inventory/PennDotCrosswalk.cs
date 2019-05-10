using BridgeCare.Models;
using System.Collections.Generic;

namespace BridgeCare.DataAccessLayer
{
    static class PennDotCrosswalk
    {
        public static List<InventoryItemModel> InventoryItems { get; private set; }

        static PennDotCrosswalk()
        {
            InventoryItems = new List<InventoryItemModel>();

            Add("BRKEY", "5A03", "BRKEY");
            Add("DISTRICT", "5A04", "DISTRICT");
            Add("COUNTY", "5A05", "COUNTY");
            Add("BRIDGE_ID", "5A01", "BRIDGE_ID");
            Add("LOCATION", "5A02", "LOCATION / STRUCTURENAME");
            Add("FEATURE_CARRIED", "5A08", "FEATURECARRIED");
            Add("FEATURE_INTERSECTED", "5A07", "FEATUREINTERSECTED");
            Add("OWNER_CODE", "5A21", "OWNER  CODE");
            Add("LENGTH", "5B18", "LENGTH");
            Add("DECK_AREA", "5B19", "Deck Area");
            Add("NUMBER_SPANS", "5B11/5B14", "# Spans");
            Add("STRUCTURE_TYPE", "6A26-29", "StructureType");
            Add("YEAR_BUILT", "5A15", "YEARBUILT");
            Add("POST_STATUS", "VP02", "POSTSTATUS");
            Add("SINGLE", "VP04", "Single(Tons);");
            Add("COMB", "VP05", "Comb(Tons);");
            Add("OTHER", "VP03", "Other");
            Add("DECK", "1A01", "DECK");
            Add("SUP", "1A04", "SUP");
            Add("SUB", "1A02", "SUB");
            Add("CULV", "1A03", "CULV");
            //Add("BR_COND", "4A14", "BR COND"); //taken out here because no column
            Add("STRUCT_DEF", "4A12", "Struct Def");
            Add("FUNC_OBSOL", "4A12", "Func Obsol");
            Add("SUFF_RATE", "4A13", "SUFF RATE");
            Add("BRKEY2", "5A03 ", "BRKEY");
            Add("INSPDATE", "7A01", "INSPDATE");
            Add("INSPTYPE", "7A03", "INSPTYPE");
            Add("INSPSTAT", "1A09", "INSPSTAT");
            Add("LEG_DIST_1", "6A03", "LEGDIST1");
            Add("LEG_DIST_2", "6A03", "LEGDIST2");
            Add("SEN_DIST_1", "6A01", "SENDIST1");
            Add("SEN_DIST_2", "6A01", "SENDIST2");
            Add("CONG_DIST_1", "6A02", "CONGDIST1");
            Add("CONG_DIST_2", "6A02", "CONGDIST2");
            Add("MPO", "5A23", "MPO");
            Add("MUNI_CODE", "5A06", "MUNICODE");
            Add("SUBM_AGENCY", "6A06", "SUBMAGENCY");
            Add("LAT", "5A10", "LAT");
            Add("LONG", "5A11", "LONG");
            Add("DECK_AREA2", "5B19", "Deck Area");
            Add("BUS_PLAN_NETWORK", "6A19", "BUS_PLAN_NETWORK");
            Add("FUNC_CLASS", "5C22", "Func Class");
            Add("OVER_WATER", "Over Water?", "");
            Add("NHS_IND", "5C29", "NHS_IND");
            Add("HBRR_ELIG", "6B41", "HBRR_ELIG");
            Add("NBI_RATING", "4A12 ", "NBI_RATING");
            Add("POST_STATUS_DATE", "VP01", "POST STATUS DATE");
            Add("DECK_WIDTH", "5B07", "deck_width");
            Add("YEAR_RECON", "5A16", "year_recon");
            Add("POST_LIMIT_WEIGHT", "VP04", "POST_LIMIT_WEIGHT");
            Add("POST_LIMIT_COMB", "VP05", "POST_LIMIT_COMB");
            Add("POST_STATUS2", "VP02", "POST_STATUS");
            Add("SPEC_RESTRICT_POST", "VP03", "SPEC_RESTRICT_POST");
            Add("WATERADEQ", "1A06", "WATERADEQ");
            Add("STRRATTING", "4A09", "STRRATING");//spelled wrong here 'STRRATTING' to align with DB
            Add("DECKGEEOM", "4A10", "DECKGEOM");//spelled wrong here 'DECKGEEOM' to align with DB
            Add("UNDERCLR", "4A11", "UNDERCLR");
            Add("APPRALIGN", "4A02", "APPRALIGN");
            Add("ADTTOTAL", "5C10", "ADTTOTAL");
            Add("TRUCKPCT", "5C14", "TRUCKPCT");
            Add("ROUTENUM", "5C06", "ROUTENUM");
            Add("LANE", "5C08", "LANES");
            Add("ADTYEAR", "5C11", "ADTYEAR");
            Add("DEFHWY", "5C28", "DEFHWY");
            Add("SUMLANE", "5A19", "SUMLANES");
            Add("ROADWIDTH", "5C27", "ROADWIDTH");
            Add("AROADWIDTH", "5C26", "AROADWIDTH");
            Add("NBISLEN", "5E01", "NBISLEN");
            Add("PROPWORK", "3B01 ", "PROPWORK");
            Add("IRLOAD", "4B07", "IRLOAD");
            // New columns -- string except if mentioned
            Add("TYPE_OF_SERVICE_ON", "5A17", "Type of Service On"); 
            Add("TYPE_OF_SERVICE_UNDER", "5A18", "Type of Service Under");

            Add("MAINT_RESP", "5A20", "Maint Resp");

            Add("DECK_STRUCTURE_TYPE", "5B01", "Deck Structure Type");
            Add("DECK_SHEET_TYPE_PENNDOT", "6A38", "Deck Sheet Type (PennDOT)");
            Add("DECK_SURFACE_TYPE", "5B02", "Deck Surface Type");
            Add("DECK_MEMBRANE_TYPE", "5B03", "Deck Membrane Type");
            Add("DECK_PROTECTION", "5B04", "Deck Protection");
            Add("SKEW", "5B09", "Skew"); // int

            Add("MAIN_SPAN_MATERIAL", "5B12", "Main Span Material");
            Add("MAIN_SPAN_DESIGN", "5B13", "Main Span Design");
            Add("APPROACH_SPAN_MATERIAL", "5B15", "Approach Span Material");
            Add("APPROACH_SPAN_DESIGN", "5B16", "Approach Span Design");
            Add("MAXIMUM_SPAN_LENGTH", "5B17", "Maximum Span Length"); // double
            Add("TOTAL_LENGTH", "5B20", "Total Length"); // double
            Add("FC_GROUP_NUMBER_MAIN", "6A44", "FC Group Number (Main)");
            Add("FC_GROUP_NUMBER_APPROACH", "6A44", "FC Group Number (Approach)");

            Add("LOAD_TYPE", "IR04", "Load Type"); // int
            Add("NBI", "IR05", "NBI"); // int
            Add("INV_RATING_TON", "IR010", "Inv Rating Ton"); // double
            Add("OPR_RATING_TON", "IR11", "Opr Rating Ton"); // double
            Add("SLC_RATING_FACTOR", "IR11a", "SLC Rating Factor"); // double
            Add("IR_RATING_FACTOR", "IR20", "IR Rating Factor"); // double
            Add("OR_RATING_FACTOR", "IR21", "OR Rating Factor"); // double
            Add("RATING_DATASET", "IR17", "Rating Dataset");

            Add("OVER_STREET_CLEARANCE", "4A15", "Over Street Clearance"); // double
            Add("UNDER_CLEARANCE", "4A17", "Under Clearance"); // double



        }

        private static void Add(string columnNameKey, string id, string description)
        {
            InventoryItemModel inventoryItem = new InventoryItemModel(columnNameKey, id, description);
            InventoryItems.Add(inventoryItem);
        }
    }
}