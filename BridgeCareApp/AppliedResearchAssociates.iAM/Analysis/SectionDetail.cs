using System;
using System.Collections.Generic;

namespace AppliedResearchAssociates.iAM.Analysis
{
    public sealed class SectionDetail
    {
        public SectionDetail(string sectionName, string facilityName)
        {
            SectionName = sectionName ?? throw new ArgumentNullException(nameof(sectionName));
            FacilityName = facilityName ?? throw new ArgumentNullException(nameof(facilityName));
        }

        public List<TreatmentOptionDetail> DetailsOfTreatmentOptions { get; } = new List<TreatmentOptionDetail>();

        public List<TreatmentConsiderationDetail> DetailsOfTreatmentConsiderations { get; } = new List<TreatmentConsiderationDetail>();

        public string FacilityName { get; }

        public string NameOfAppliedTreatment { get; set; }

        public string SectionName { get; }

        public Dictionary<string, double> ValuePerNumberAttribute { get; } = new Dictionary<string, double>();

        public Dictionary<string, string> ValuePerTextAttribute { get; } = new Dictionary<string, string>();
    }
}
