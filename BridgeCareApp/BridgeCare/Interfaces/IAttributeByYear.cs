﻿using BridgeCare.Models;
using System.Collections.Generic;

namespace BridgeCare.Interfaces
{
    public interface IAttributeByYear
    {
        List<AttributeByYearModel> GetHistoricalAttributes(SectionModel sectionModel, BridgeCareContext db);

        List<AttributeByYearModel> GetProjectedAttributes(SimulatedSegmentAddressModel segmentAddressModel, BridgeCareContext db);
    }
}