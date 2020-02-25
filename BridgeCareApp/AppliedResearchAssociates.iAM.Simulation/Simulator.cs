using System;
using System.Collections.Generic;
using System.Linq;

namespace AppliedResearchAssociates.iAM.Simulation
{
    public class Simulator
    {
        private Investment Investment;
        private List<Section> Sections;
        private AnalysisMethod Method;
        private List<Deteriorate> Deteriorates;

        public void Simulate()
        {
            // fill OCI weights.

            // load attributes.

            // drop previous simulation.

            // get simulation method.

            // if conditional RSL, load conditional RSL.

            // get simulation attributes.

            RunSimulation();
        }

        private void RunSimulation()
        {
            // fill committed projects.

            // calculate areas for all sections.

            // update simulation attributes.

            // spend all committed projects.

            // "ready to go"

            foreach (var year in Enumerable.Range(Investment.StartYear, 1 + Investment.AnalysisPeriod))
            {
                // apply "deteriorate"/performance curves.

                // determine benefit/cost.

                // load & apply committed projects.

                // calculate network averages and "deficient base" (after committed).

                // either (a) spend as budget permits or (b) spend until targets/deficient met.

                // report targets/deficient.
            }

            // create simulation table.

            // "write simulation" for each section.

            // database bulk load for each "simulation table".

            // if multi-year, "solve".

            SimulationMessaging.ListAttributes = m_listAttributes;

            //SELECT column_name FROM information_schema.columns WHERE table_name =
            // Get a list of all the columns in the segmented tables
            // The juridiction determines which sections are included in Simulation.
            // Users are expected to enter query with year modifier (i.e DISTRICT_2006 = 'M-11')

            if (!FillSectionList()) return;

            if (!FillCommittedProjects()) return;

            //Calculate areas for all sections.
            int areaYear = Investment.StartYear - 1;
            if (SimulationMessaging.IsOMS) areaYear = Investment.StartDate.Year;

            foreach (Sections section in m_listSections)
            {
                section.CalculateArea(areaYear);
            }

            if (!UpdateSimulationAttributes()) return;

            //Spend all committed projects.
            //Remove money for committed projects from the all budgets, all years so that it is accounted
            //for scheduled and split treatments.
            //The report will show the money spent in the appropriate year.  We just need to move the spending in memory before the analysis.
            foreach (var section in m_listSections)
            {
                foreach (var commit in section.YearCommit)
                {
                    if (commit.Year >= Investment.StartYear)
                    {
                        Investment.SpendBudget(commit.Cost, commit.Budget, commit.Year.ToString());
                    }
                }
            }


            //After FillSectionList everything is read to go to start simulating!
            //Everything is rolled up to the year of the start date.
            for (int nYear = Investment.StartYear; nYear < Investment.StartYear + Investment.AnalysisPeriod; nYear++)
            {
                //Apply Deteriorate/Performance curves.
                ApplyDeterioration(nYear);

                //Determine Benefit/Cost
                m_listApplyTreatment.Clear();
                DetermineBenefitCostIterative(nYear);

                //Load Committed Projects.  These get comitted (and spent) regardless of budget.
                //Apply committed projects
                ApplyCommitted(nYear);

                //Calculate network averages and deficient base (after committed).
                DetermineTargetAndDeficient(nYear);

                if (Method.TypeAnalysis.Contains("Multi"))
                {
                    //MULTIBUDGET FIX
                    SpendBudgetPermits(nYear, "None");
                }
                else
                {
                    switch (Method.TypeBudget)
                    {
                        case "No Spending":
                            SpendBudgetPermits(nYear, "None");
                            break;

                        case "As Budget Permits":
                            SpendBudgetPermits(nYear, "");
                            break;

                        case "Unlimited":
                            SpendBudgetPermits(nYear, "Unlimited");
                            break;

                        case "Until Targets Met":
                        case "Until Deficient Met":
                        case "Targets/Deficient Met":
                            SpendUntilTargetsDeficientMet(nYear);
                            break;
                    }
                }
                ReportTargetDeficient(nYear);
            }

            if (!CreateSimulationTable(m_strNetworkID, m_strSimulationID)) return;

            List<TextWriter> listSimulationWriters = new List<TextWriter>();
            for (var i = 0; i < m_dictionarySimulationTables.Count; i++)
            {
                listSimulationWriters.Add(SimulationMessaging.CreateTextWriter(SimulationMessaging.SimulationTable + "_" + i + ".txt", out _));
            }

            foreach (Sections section in m_listSections)
            {
                section.WriteSimulation(Investment.StartYear, Investment.StartYear + Investment.AnalysisPeriod - 1, m_dictionaryAttributeSimulationTable, listSimulationWriters);
            }

            foreach (var tw in listSimulationWriters)
            {
                tw.Close();
            }
            String strMyDocumentsFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            strMyDocumentsFolder += "\\RoadCare Projects\\Temp";
            for (var j = 0; j < m_dictionarySimulationTables.Count; j++)
            {
                string sOutFile = strMyDocumentsFolder + "\\" + SimulationMessaging.SimulationTable + "_" + j + ".txt";

                switch (DBMgr.NativeConnectionParameters.Provider)
                {
                    case "MSSQL":
                        DBMgr.SQLBulkLoad(SimulationMessaging.SimulationTable + "_" + j, sOutFile, "\\t");
                        break;

                    case "ORACLE":
                        DBMgr.OracleBulkLoad(DBMgr.NativeConnectionParameters, SimulationMessaging.SimulationTable, sOutFile, DBMgr.GetTableColumns(SimulationMessaging.SimulationTable), "\\t");
                        break;

                    default:
                        throw new NotImplementedException("TODO: Create ANSI implementation for XXXXXXXXXXXX");
                }
            }

            //Switch for multi-year
            switch (Method.TypeAnalysis)
            {
                case "Multi-year Incremental Benefit/Cost":
                case "Multi-year Remaining Life/Cost":
                    SolveMultipleYear();
                    break;
            }
        }

        private void ApplyDeterioration(int nYear)
        {
            foreach (var section in Sections)
            {
                section.ResetSectionForNextYear();
                section.AddRemainingLifeHash(nYear);
                //Don't apply deterioration if a SplitTreatment(cashflow) project is partially complete
                var applyDeterioration = true;
                foreach (var committed in section.YearCommit)
                {
                    if (!string.IsNullOrWhiteSpace(committed.SplitTreatmentId) && nYear == committed.Year)
                    {
                        applyDeterioration = false;
                    }
                }

                foreach (var deteriorate in Deteriorates)
                {
                    //This calculates the base attribute value for next year.
                    //Calculates Base Benefit IF it is the BENEFIT variable.
                    //Calculates the Base Remaining life if a deficient level is entered.
                    if (applyDeterioration)
                    {
                        section.ApplyDeteriorate(deteriorate, nYear);
                    }
                }

                //Determine the controlling RSL bin and set the values of each to the proper level
                if (Method.IsConditionalRSL)
                {
                    foreach (var deteriorate in Deteriorates)
                    {
                        section.NormalizeConditionalRSL(deteriorate.Attribute, nYear);
                    }
                }

                //Move not deteriorating/performnace variables into the new year.
                section.ApplyNonDeteriorate(nYear, m_Investment);
            }
        }
    }

    internal class Deteriorate
    {

    }

    internal class AnalysisMethod
    {
        public bool IsConditionalRSL { get; set; }
    }

    internal class Section
    {

    }

    internal class Simulation
    {
    }

    internal class Investment
    {
        public int StartYear { get; set; }
        public int AnalysisPeriod { get; set; }
    }
}
