using System.Web;

namespace BridgeCare.Interfaces
{
    public interface ICommittedProjects
    {
        void SaveCommittedProjectsFiles(HttpRequest httpRequest, BridgeCareContext db);

        byte[] ExportCommittedProjects(int simulationId, int networkId, BridgeCareContext db);
    }
}
