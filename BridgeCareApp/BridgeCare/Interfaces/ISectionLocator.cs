using BridgeCare.Models;
using System.Linq;

namespace BridgeCare.Interfaces
{
    public interface ISectionLocator
    {
        IQueryable<SectionLocationModel> Locate(SectionModel section, BridgeCareContext db);
    }
}