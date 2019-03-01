using BridgeCare.Models;
using System.Linq;

namespace BridgeCare.Interfaces
{
    public interface ISectionLocator
    {
        IQueryable<SectionLocationModel> GetLatitudeLongitude(int NetworkId, BridgeCareContext db);
    }
}