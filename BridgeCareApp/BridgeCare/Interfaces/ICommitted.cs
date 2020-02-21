using System.Collections.Generic;
using BridgeCare.EntityClasses;
using BridgeCare.Models;

namespace BridgeCare.Interfaces
{
    public interface ICommitted
    {
        void SaveCommittedProjects(List<CommittedProjectModel> committedProjectModels, BridgeCareContext db);
        void SavePermittedCommittedProjects(List<CommittedProjectModel> committedProjectModels, BridgeCareContext db, string username);

        List<CommittedEntity> GetCommittedProjects(int simulationId, BridgeCareContext db);
        List<CommittedEntity> GetPermittedCommittedProjects(int simulationId, BridgeCareContext db, string username);
    }
}
