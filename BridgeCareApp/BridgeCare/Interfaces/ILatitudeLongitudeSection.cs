using BridgeCare.Models;
using System.Linq;

namespace BridgeCare.Interfaces
{
    public interface ILatitudeLongitudeSection
    {
        IQueryable<LatitudeLongitudeSectionModel> GetLatitudeLongitude(int NetworkId, BridgeCareContext db);
    }
}