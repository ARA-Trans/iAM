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
    public class Committed : ICommitted
    {
        public void SaveCommittedProjects(List<CommittedProjectModel> committedProjectModels, BridgeCareContext db)
        {
            try
            {
                foreach (var committedProjectModel in committedProjectModels)
                {
                    var existingCommitted = db.COMMITTEDPROJECTs.FirstOrDefault(c => c.SECTIONID == committedProjectModel.SectionId && c.SIMULATIONID == committedProjectModel.SimulationId);
                    if (existingCommitted != null)
                    {
                        continue;
                    }
                    var committed = CreateCommitted(committedProjectModel);
                    db.COMMITTEDPROJECTs.Add(committed);
                    db.SaveChanges();

                    // get
                    var Insertedcommitted = GetCommittedProject(committedProjectModel.SimulationId, committedProjectModel.SectionId, committedProjectModel.Years, db);

                    // Add consequences             
                    foreach (var commitConsequence in committedProjectModel.CommitConsequences)
                    {
                        committed.CommitConsequences.Add(new COMMIT_CONSEQUENCES(commitConsequence.Attribute_, commitConsequence.Change_));
                    }
                    db.SaveChanges();
                }
            }
            catch (SqlException ex)
            {
                HandleException.SqlError(ex, "COMMITTED_PROJECT");
            }
            catch (OutOfMemoryException ex)
            {
                HandleException.OutOfMemoryError(ex);
            }
        }

        private COMMITTED_PROJECT CreateCommitted(CommittedProjectModel committedProjectModel)
        {
            return new COMMITTED_PROJECT(committedProjectModel.SimulationId, committedProjectModel.SectionId, committedProjectModel.Years, committedProjectModel.TreatmentName, committedProjectModel.YearSame, committedProjectModel.YearAny, committedProjectModel.Budget, committedProjectModel.Cost);
        }

        public COMMITTED_PROJECT GetCommittedProject(int simulationId, int sectionId, int years, BridgeCareContext db)
        {
            return db.COMMITTEDPROJECTs.FirstOrDefault(c => c.SIMULATIONID == simulationId && c.SECTIONID == sectionId && c.YEARS == years);
        }
    }
}