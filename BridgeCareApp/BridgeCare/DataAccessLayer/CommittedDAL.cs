using BridgeCare.EntityClasses;
using BridgeCare.Interfaces;
using BridgeCare.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
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
            // Remove all existing commitments for the sections in the submitted commitments
            foreach (var committedProjectModel in committedProjectModels)
            {
                var oldCommittments = db.CommittedProjects
                    .Where(c =>
                        c.SECTIONID == committedProjectModel.SectionId &&
                        c.SIMULATIONID == committedProjectModel.SimulationId
                    );

                foreach (var committedEntity in oldCommittments)
                {
                    db.CommittedProjects.Remove(committedEntity);
                }
            }

            // Add the new commitments
            foreach (var committedProjectModel in committedProjectModels)
            {
                db.CommittedProjects.Add(new CommittedEntity(committedProjectModel));
            }

            db.SaveChanges();
        }

        /// <summary>
        /// Save committed projects in the database, if the user owns the scenario
        /// </summary>
        /// <param name="committedProjectModels"></param>
        /// <param name="db"></param>
        public void SavePermittedCommittedProjects(List<CommittedProjectModel> committedProjectModels, BridgeCareContext db, string username)
        {
            foreach (var committedProjectModel in committedProjectModels) {
                if (!db.Simulations.Any(s => s.SIMULATIONID == committedProjectModel.SimulationId))
                    throw new RowNotInTableException($"No simulation found with id {committedProjectModel.SimulationId}");
                if (!db.Simulations.Include(s => s.USERS).First(s => s.SIMULATIONID == committedProjectModel.SimulationId).UserCanModify(username))
                    throw new UnauthorizedAccessException("You are not authorized to modify this scenario's committed projects.");
            }
            SaveCommittedProjects(committedProjectModels, db);
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

        /// <summary>
        /// Get all the committed projects for a given simulation id, if owned by the current user
        /// </summary>
        /// <param name="simulationId"></param>
        /// <param name="db"></param>
        /// <param name="username"></param>
        /// <returns></returns>
        public List<CommittedEntity> GetPermittedCommittedProjects(int simulationId, BridgeCareContext db, string username)
        {
            if (!db.Simulations.Any(s => s.SIMULATIONID == simulationId))
                throw new RowNotInTableException($"No scenario found with id {simulationId}.");
            if (!db.Simulations.Include(s => s.USERS).First(s => s.SIMULATIONID == simulationId).UserCanRead(username))
                throw new UnauthorizedAccessException("You are not authorized to view this scenario's committed projects.");
            return GetCommittedProjects(simulationId, db);
        }
    }
}
