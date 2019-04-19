using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BridgeCare.DataAccessLayer
{
    public class PennDotScreenLabels
    {
        string Id;
        string DescriptiveName;
        public PennDotScreenLabels(string AlphaNumericId, string IdName)
        {
            Id = AlphaNumericId;
            DescriptiveName = IdName;
        }
        
    }
    public class PennDotCrosswalkDictionary : Dictionary<string, PennDotScreenLabels>
    {
        PennDotCrosswalkDictionary()
        {
            Add("BRKEY", "5A03", "BRKEY");
            Add("DISTRICT", "5A04", "DISTRICT");
            Add("COUNTY", "5A05", "COUNTY");
            Add("BRIDGE_ID", "5A01", "BRIDGE_ID");
            Add("LOCATION", "5A02", "LOCATION / STRUCTURENAME");
            Add("FEATURE_CARRIED", "5A08", "FEATURECARRIED");
            Add("FEATURE_INTERSECTED", "5A07", "FEATUREINTERSECTED");
            Add("OWNER_CODE", "5A21", "OWNER  CODE");
            Add("LENGTH", "5B18", "LENGTH");
            Add("Deck_Area", "5B19", "Deck Area");
            Add("Number_Spans", "5B11/5B14", "# Spans");
            Add("Structure_Type", "6A26-29", "StructureType");
            Add("YEAR_BUILT", "5A15", "YEARBUILT");
            Add("POST_STATUS", "VP02", "POSTSTATUS");
            Add("Single_", "VP04", "Single(Tons);");
            Add("Comb", "VP05", "Comb(Tons);");
            Add("Other", "VP03", "Other");
            Add("DECK", "1A01", "DECK");
            Add("SUP", "1A04", "SUP");
            Add("SUB", "1A02", "SUB");
            Add("CULV", "1A03", "CULV");
            Add("BR_Cond", "4A14", "BR COND");
            Add("Struct_Def", "4A12", "Struct Def");
            Add("Func_Obsol", "4A12", "Func Obsol");
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
            Add("Deck_Area2", "5B19", "Deck Area");
            Add("BUS_PLAN_NETWORK", "6A19", "BUS_PLAN_NETWORK");
            Add("Func_Class", "5C22", "Func Class");
            Add("Over_Water", "Over Water?", "");
            Add("NHS_IND", "5C29", "NHS_IND");
            Add("HBRR_ELIG", "6B41", "HBRR_ELIG");
            Add("NBI_RATING", "4A12 ", "NBI_RATING");
            Add("POST_STATUS_DATE", "VP01", "POST STATUS DATE");
            Add("deck_width", "5B07", "deck_width");
            Add("year_recon", "5A16", "year_recon");
            Add("POST_LIMIT_WEIGHT", "VP04", "POST_LIMIT_WEIGHT");
            Add("POST_LIMIT_COMB", "VP05", "POST_LIMIT_COMB");
            Add("POST_STATUS2", "VP02", "POST_STATUS");
            Add("SPEC_RESTRICT_POST", "VP03", "SPEC_RESTRICT_POST");
            Add("WATERADEQ", "1A06", "WATERADEQ");
            Add("STRRATING", "4A09", "STRRATING");
            Add("DECKGEOM", "4A10", "DECKGEOM");
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
        }
        public void Add(string columnNameKey, string id, string description)
        {
            PennDotScreenLabels val = new PennDotScreenLabels(id, description);
            this.Add(columnNameKey, val);
        }                            
        
    }
}