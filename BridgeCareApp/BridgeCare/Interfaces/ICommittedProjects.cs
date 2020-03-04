using BridgeCare.Models;
using System.Web;

namespace BridgeCare.Interfaces
{
    public interface ICommittedProjects
    {
        void SaveCommittedProjectsFiles(HttpRequest request, BridgeCareContext db, UserInformationModel userInformation);
        byte[] ExportCommittedProjects(int simulationId, int networkId, BridgeCareContext db, UserInformationModel userInformation);
    }
}
