using BridgeCare.Models;
using System.Linq;

namespace BridgeCare.Interfaces
{
    public interface ISections
    {
        IQueryable<SectionModel> GetSections(NetworkModel data, BridgeCareContext db);
    }
}