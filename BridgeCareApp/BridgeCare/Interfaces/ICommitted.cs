using System.Collections.Generic;
using BridgeCare.EntityClasses;
using BridgeCare.Models;

namespace BridgeCare.Interfaces
{
    public interface ICommitted
    {
        void SaveCommittedProjects(List<CommittedProjectModel> committedProjectModels, BridgeCareContext db);
        void SaveOwnedCommittedProjects(List<CommittedProjectModel> committedProjectModels, BridgeCareContext db, string username);

        List<CommittedEntity> GetCommittedProjects(int simulationId, BridgeCareContext db);
        List<CommittedEntity> GetOwnedCommittedProjects(int simulationId, BridgeCareContext db, string username);
    }
}
