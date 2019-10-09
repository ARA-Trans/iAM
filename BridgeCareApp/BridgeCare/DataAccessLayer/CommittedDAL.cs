using BridgeCare.ApplicationLog;
using BridgeCare.EntityClasses;
using BridgeCare.Interfaces;
using BridgeCare.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;

namespace BridgeCare.DataAccessLayer
{
    public class CommittedDAL : ICommitted
    {
        /// <summary>
        /// Save committed projects in the database
        /// </summary>
        /// <param name="committedProjectModels"></param>
        /// <param name="db"></param>
        public void SaveCommittedProjects(List<CommittedProjectModel> committedProjectModels, BridgeCareContext db)
        {
            foreach (var committedProjectModel in committedProjectModels)
            {
                var committedProjectEntity = db.CommittedProjects
                    .FirstOrDefault(c => c.SECTIONID == committedProjectModel.SectionId && c.SIMULATIONID == committedProjectModel.SimulationId);

                if (committedProjectEntity == null)
                    db.CommittedProjects.Add(new CommittedEntity(committedProjectModel));
            }

            db.SaveChanges();
        }

        /// <summary>
        /// Get all the committed projects for a given simulation id
        /// </summary>
        /// <param name="simulationId"></param>
        /// <param name="db"></param>
        /// <returns></returns>
        public List<CommittedEntity> GetCommittedProjects(int simulationId, BridgeCareContext db)
        {
            return db.CommittedProjects.Include(c => c.COMMIT_CONSEQUENCES).Where(c => c.SIMULATIONID == simulationId).ToList();
        }
    }
}