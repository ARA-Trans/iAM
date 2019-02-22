using BridgeCare.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace BridgeCare.Interfaces
{
    public interface IAttributeByYear  {

        List<AttributeByYearModel> GetHistoricalAttributes(int NetworkId, int sectionID,  BridgeCareContext db);
        List<AttributeByYearModel> GetProjectedAttributes(int SimulationId, int NetworkId, int SectionId, BridgeCareContext db);
    }
}