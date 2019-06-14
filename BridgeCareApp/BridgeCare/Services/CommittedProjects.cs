using BridgeCare.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using OfficeOpenXml;
using BridgeCare.Models;
using BridgeCare.ApplicationLog;

namespace BridgeCare.Services
{
    public class CommittedProjects : ICommittedProjects
    {
        readonly ICommitted committed;
        private readonly ISections sections;

        public CommittedProjects(ICommitted committed, ISections sections)
        {
            this.committed = committed;
            this.sections = sections;
        }

        public void SaveCommittedProjectsFiles(HttpFileCollection files, string selectedScenarioId, string networkId, BridgeCareContext db)
        {
            var committedProjectModels = new List<CommittedProjectModel>();
            var postedFile = files[0];
            GetCommittedProjectModels(postedFile, selectedScenarioId, networkId, committedProjectModels, db);
            SaveCommittedProjects(committedProjectModels, db);

        }

        private void SaveCommittedProjects(List<CommittedProjectModel> committedProjectModels, BridgeCareContext db)
        {
            committed.SaveCommittedProjects(committedProjectModels, db);
        }

        private void GetCommittedProjectModels(HttpPostedFile postedFile, string selectedScenarioId, string networkId, List<CommittedProjectModel> committedProjectModels, BridgeCareContext db)
        {
            try
            {
                // below should be done on server side??
                var filePath = HttpContext.Current.Server.MapPath("~/" + postedFile.FileName);

                //postedFile.SaveAs(filePath);            
                var package = new ExcelPackage(postedFile.InputStream); //(new FileInfo(postedFile.FileName));
                ExcelWorksheet worksheet = package.Workbook.Worksheets[1];
                var headers = worksheet.Cells.GroupBy(cell => cell.Start.Row).First();
                var start = worksheet.Dimension.Start;
                var end = worksheet.Dimension.End;
                for (int row = start.Row + 1; row <= end.Row; row++)
                {
                    var column = start.Column + 2;
                    var brKey = Convert.ToInt32(GetCellValue(worksheet, row, 1));
                    var sectionId = sections.GetSectionId(Convert.ToInt32(networkId), brKey, db);

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