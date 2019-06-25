using System.Web;

namespace BridgeCare.Interfaces
{
    public interface ICommittedProjects
    {
        void SaveCommittedProjectsFiles(HttpFileCollection files, string selectedScenarioId, string networkId, BridgeCareContext db);

        byte[] ExportCommittedProjects(int simulationId, int networkId, BridgeCareContext db);
    }
}
