using System.Web;

namespace BridgeCare.Interfaces
{
    public interface ICommittedProjects
    {
        void SaveCommittedProjectsFiles(HttpRequest request, BridgeCareContext db);
        byte[] ExportCommittedProjects(int simulationId, int networkId, BridgeCareContext db);
    }
}
