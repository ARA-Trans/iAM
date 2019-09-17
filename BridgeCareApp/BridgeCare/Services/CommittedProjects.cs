using BridgeCare.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Http;
using OfficeOpenXml;
using BridgeCare.Models;
using BridgeCare.ApplicationLog;
using BridgeCare.EntityClasses;

namespace BridgeCare.Services
{
    public class CommittedProjects : ICommittedProjects
    {
        readonly ICommitted committedRepo;
        private readonly ISections sectionsRepo;

        public CommittedProjects(ICommitted committedRepo, ISections sectionsRepo)
        {
            this.committedRepo = committedRepo;
            this.sectionsRepo = sectionsRepo;
        }

        /// <summary>
        /// Save committed projects from the template files
        /// </summary>
        /// <param name="httpRequest"></param>
        /// <param name="db"></param>
        public void SaveCommittedProjectsFiles(HttpRequest httpRequest, BridgeCareContext db)
        {
            if (httpRequest.Files.Count < 1)
                throw new ConstraintException("Files Not Found");

            var files = httpRequest.Files;
            var selectedScenarioId = httpRequest.Form.Get("selectedScenarioId");
            var networkId = httpRequest.Form.Get("networkId");

            var committedProjectModels = new List<CommittedProjectModel>();
            for (int i = 0; i < files.Count; i++)
            {
                GetCommittedProjectModels(files[i], selectedScenarioId, networkId, committedProjectModels, db);
                committedRepo.SaveCommittedProjects(committedProjectModels, db);
            }
        }

        /// <summary>
        /// Export committed projects for a simulation
        /// </summary>
        /// <param name="simulationId"></param>
        /// <param name="networkId"></param>
        /// <param name="db"></param>
        /// <returns></returns>
        public byte[] ExportCommittedProjects(int simulationId, int networkId, BridgeCareContext db)
        {
            using (ExcelPackage excelPackage = new ExcelPackage(new System.IO.FileInfo("CommittedProjects.xlsx")))
            {
                // This method may stay here or if too long then move to helper class   Fill(worksheet, , db);
                var committedProjects = committedRepo.GetCommittedProjects(simulationId, db);
                var worksheet = excelPackage.Workbook.Worksheets.Add("Committed Projects");
                if (committedProjects.Count != 0)
                {
                    AddHeaderCells(worksheet, committedProjects.FirstOrDefault().COMMIT_CONSEQUENCES.ToList());
                    AddDataCells(worksheet, committedProjects, networkId, db);
                }
                return excelPackage.GetAsByteArray();
            }
        }

        private void AddDataCells(ExcelWorksheet worksheet, List<CommittedEntity> committedProjects, int networkId, BridgeCareContext db)
        {            
            var networkModel = new NetworkModel { NetworkId = networkId };
            var sectionModels = sectionsRepo.GetSections(networkModel, db).ToList();
            var row = 2;
            foreach(var committedProject in committedProjects)
            {                
                var column = 1;
                var sectionModel = sectionModels.FirstOrDefault(s => s.SectionId == committedProject.SECTIONID);
                // BRKEY, BMSID
                worksheet.Cells[row, column++].Value = Convert.ToInt32(sectionModel.ReferenceKey);
                worksheet.Cells[row, column++].Value = sectionModel.ReferenceId;

                // Committed_
                worksheet.Cells[row, column++].Value = committedProject.TREATMENTNAME;
                worksheet.Cells[row, column++].Value = committedProject.YEARS;
                worksheet.Cells[row, column++].Value = committedProject.YEARANY;
                worksheet.Cells[row, column++].Value = committedProject.YEARSAME;
                worksheet.Cells[row, column++].Value = committedProject.BUDGET;
                worksheet.Cells[row, column++].Value = committedProject.COST_;
                worksheet.Cells[row, column++].Value = string.Empty; // AREA

                // Consequences
                foreach(var commitConsequence in committedProject.COMMIT_CONSEQUENCES)
                {
                    worksheet.Cells[row, column++].Value = commitConsequence.CHANGE_;
                }
                row++;
            }
        }

        private void AddHeaderCells(ExcelWorksheet worksheet, List<CommitConsequencesEntity> commitConsequences)
        {
            var fixColumnHeaders = new List<string>() { "BRKEY", "BMSID", "TREATMENT", "YEAR", "YEARANY", "YEARSAME", "BUDGET", "COST", "AREA" };
            int headerRow = 1;
            for (int column = 0; column < fixColumnHeaders.Count; column++)
            {
                worksheet.Cells[headerRow, column + 1].Value = fixColumnHeaders[column];
            }
            var currentColumn = fixColumnHeaders.Count;
            foreach (var commitConsequence in commitConsequences)
            {
                worksheet.Cells[headerRow, ++currentColumn].Value = commitConsequence.ATTRIBUTE_;
            }
        }

        private void GetCommittedProjectModels(HttpPostedFile postedFile, string selectedScenarioId, string networkId, List<CommittedProjectModel> committedProjectModels, BridgeCareContext db)
        {
            try
            {
                var package = new ExcelPackage(postedFile.InputStream); //(new FileInfo(postedFile.FileName));
                ExcelWorksheet worksheet = package.Workbook.Worksheets[1];
                var headers = worksheet.Cells.GroupBy(cell => cell.Start.Row).First();
                var start = worksheet.Dimension.Start;
                var end = worksheet.Dimension.End;
                for (int row = start.Row + 1; row <= end.Row; row++)
                {
                    var column = start.Column + 2;
                    var brKey = Convert.ToInt32(GetCellValue(worksheet, row, 1));
                    var sectionId = sectionsRepo.GetSectionId(Convert.ToInt32(networkId), brKey, db);

                    // BMSID till COST -> entry in COMMITTED_                    
                    var committedProjectModel = new CommittedProjectModel
                    {
                        SectionId = sectionId,
                        SimulationId = Convert.ToInt32(selectedScenarioId),
                        TreatmentName = GetCellValue(worksheet, row, column),
                        Years = Convert.ToInt32(GetCellValue(worksheet, row, ++column)),
                        YearAny = Convert.ToInt32(GetCellValue(worksheet, row, ++column)),
                        YearSame = Convert.ToInt32(GetCellValue(worksheet, row, ++column)),
                        Budget = GetCellValue(worksheet, row, ++column),
                        Cost = Convert.ToInt32(GetCellValue(worksheet, row, ++column))
                    };

                    var CommitConsequences = new List<CommitConsequenceModel>();
                    // Ignore AREA column, from current column till end.Column -> attributes i.e. entry in COMMIT_CONSEQUENCES
                    for (var col = column + 2; col <= end.Column; col++)
                    {
                        CommitConsequences.Add(new CommitConsequenceModel
                        {
                            Attribute_ = GetHeader(headers, col),
                            Change_ = GetCellValue(worksheet, row, col)
                        });
                    }
                    committedProjectModel.CommitConsequences = CommitConsequences;
                    committedProjectModels.Add(committedProjectModel);
                }
            }
            catch (Exception ex)
            {
                HandleException.GeneralError(ex);
            }
        }

        private string GetHeader(IGrouping<int, ExcelRangeBase> headers, int col)
        {
            return (string)headers.Skip(col - 1).First().Value;
        }

        private string GetCellValue(ExcelWorksheet worksheet, int row, int col)
        {
            return worksheet.Cells[row, col].Value.ToString().Trim();
        }       
    }
}