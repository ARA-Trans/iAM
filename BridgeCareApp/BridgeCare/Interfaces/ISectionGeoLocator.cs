using BridgeCare.Models;
using System.Linq;

namespace BridgeCare.Interfaces
{
    public interface ISectionGeoLocator
    {
        IQueryable<LatitudeLongitudeSectionModel> GetLatitudeLongitude(int NetworkId, BridgeCareContext db);
    }
}