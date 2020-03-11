using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BridgeCare.Services.SummaryReport
{
    public static class ShortNamesForTreatments
    {
        internal static Dictionary<string, string> GetShortNamesForTreatments()
        {
            var shortNames = new Dictionary<string, string>
            {
                {"Culvert Preservation", "Culv_Pres" },
                {"Culvert Rehab (Other)", "Culv_Rehab_Other" },
                {"Culvert Replacement (Box/Frame/Arch)", "Culv_Rep_Box" },
                {"Culvert Replacement (Other)", "Culv_Rep_Other" },
                {"Culvert Replacement (Pipe)", "Culv_Rep_Pipe" },
                {"Painting (Joint/Spot/Zone)", "Spot_Paint" },
                {"Painting (Full)", "Full_Paint" },
                {"Structural Overlay/Joints/Coatings", "Struct_Overlay" },
                {"Epoxy/Joint Glands/Coatings", "Epx" },
                {"Large Bridge Preservation", "LB_Pres" },
                {"Deck Replacement", "Deck_Replc" },
                {"Substructure Rehab", "Sub_Rehab" },
                {"Superstructure Rep/Rehab", "Sup_Rpl" },
                {"Large Bridge Rehab", "LB_Rehab" },
                {"Bridge Replacement", "Brdg_Repl" },
                {"Preservation","MPMS_Pres" },
                {"Rehabilitation", "MPMS_Rehab" },
                {"Removal", "MPMS_Rem" },
                {"Repair", "MPMS_Repair" },
                {"Replacement", "MPMS_Repl" }
            };
            return shortNames;
        }
    }
}
