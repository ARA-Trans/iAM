using System;
using System.Web;

namespace BridgeCare.Interfaces
{
    public interface ICommittedProjects
    {
        void SaveCommittedProjectsFiles(HttpFileCollection files, BridgeCareContext db);
    }
}
