using BridgeCare.Models;
using System.Collections.Generic;

namespace BridgeCare.Interfaces
{
    public interface IAttributesByYear
    {
        List<AttributeByYearModel> GetHistoricalAttributes(SectionModel sectionModel, BridgeCareContext db);

        List<AttributeByYearModel> GetProjectedAttributes(SimulatedSegmentIdsModel segmentAddressModel, BridgeCareContext db);
    }
}