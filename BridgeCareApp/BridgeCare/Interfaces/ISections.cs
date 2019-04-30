using BridgeCare.Models;
using System.Linq;

namespace BridgeCare.Interfaces
{
    public interface ISections
    {
        IQueryable<SectionModel> GetSections(NetworkModel data, BridgeCareContext db);

        int GetBrKey(int networkID, int sectionID, BridgeCareContext db);

        int GetSectionId(int networkID, int brKey, BridgeCareContext db);
    }
}