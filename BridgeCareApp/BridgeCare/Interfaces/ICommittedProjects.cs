using System;
using System.Web;

namespace BridgeCare.Interfaces
{
    public interface ICommittedProjects
    {
        void SaveCommittedProjectsFiles(HttpFileCollection files, string selectedScenarioId, string networkId, BridgeCareContext db);
    }
}
