using BridgeCare.ApplicationLog;
using BridgeCare.EntityClasses;
using BridgeCare.Interfaces;
using BridgeCare.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace BridgeCare.DataAccessLayer
{
    public class CommittedDAL : ICommitted
    {
        public void SaveCommittedProjects(List<CommittedProjectModel> committedProjectModels, BridgeCareContext db)
        {
            try
            {
                foreach (var committedProjectModel in committedProjectModels)
                {
                    var existingCommitted = db.CommittedProjects.FirstOrDefault(c => c.SECTIONID == committedProjectModel.SectionId && c.SIMULATIONID == committedProjectModel.SimulationId);
                    if (existingCommitted != null)
                    {
                        continue;
                    }
                    var committed = CreateCommitted(committedProjectModel);
                    db.CommittedProjects.Add(committed);
                    db.SaveChanges();

                    // get
                    var Insertedcommitted = GetCommittedProject(committedProjectModel.SimulationId, committedProjectModel.SectionId, committedProjectModel.Years, db);

                    // Add consequences             
                    foreach (var commitConsequence in committedProjectModel.CommitConsequences)
                    {
                        committed.COMMIT_CONSEQUENCES.Add(new CommitConsequencesEntity(commitConsequence.Attribute_, commitConsequence.Change_));
                    }
                    db.SaveChanges();
                }
            }
            catch (SqlException ex)
            {
                HandleException.SqlError(ex, "COMMITTED_");
            }
            catch (OutOfMemoryException ex)
            {
                HandleException.OutOfMemoryError(ex);
            }
        }

        private CommittedEntity CreateCommitted(CommittedProjectModel committedProjectModel)
        {
            return new CommittedEntity(committedProjectModel.SimulationId, committedProjectModel.SectionId, committedProjectModel.Years, committedProjectModel.TreatmentName, committedProjectModel.YearSame, committedProjectModel.YearAny, committedProjectModel.Budget, committedProjectModel.Cost);
        }

        public CommittedEntity GetCommittedProject(int simulationId, int sectionId, int years, BridgeCareContext db)
        {
            return db.CommittedProjects.FirstOrDefault(c => c.SIMULATIONID == simulationId && c.SECTIONID == sectionId && c.YEARS == years);
        }
    }
}