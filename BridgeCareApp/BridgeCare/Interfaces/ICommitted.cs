using System.Collections.Generic;
using BridgeCare.Models;

namespace BridgeCare.Interfaces
{
    public interface ICommitted
    {
        void SaveCommittedProjects(List<CommittedProjectModel> committedProjectModels, BridgeCareContext db);
    }
}
