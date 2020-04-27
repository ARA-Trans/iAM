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
                {"Culvert Rehab (Other)", "Culv_Rehab_Other" },
                {"Culvert Replacement (Box/Frame/Arch)", "Culv_Rep_Box" },
                {"Culvert Replacement (Other)", "Culv_Rep_Other" },
                {"Culvert Replacement (Pipe)", "Culv_Rep_Pipe" },
                {"County Maintenance - Deck Work", "CM_Deck" },
                {"County Maintenance - Superstructure Work", "CM_Super" },
                {"County Maintenance - Substructure Work", "CM_Sub" },
                {"Painting (Joint/Spot/Zone)", "Spot_Paint" },
                {"Painting (Full)", "Full_Paint" },
                {"Structural Overlay/Joints/Coatings", "Struct_Overlay" },
                {"Epoxy/Joint Glands/Coatings", "Epx" },
                {"Bituminous Overlay", "Bit_Over" },
                {"Deck Replacement", "Deck_Replc" },
                {"Substructure Rehab", "Sub_Rehab" },
                {"Superstructure Rep/Rehab", "Sup_Rpl" },
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
