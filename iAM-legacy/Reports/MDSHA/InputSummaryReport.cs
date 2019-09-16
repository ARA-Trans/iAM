namespace Reports
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Reflection;
    using System.Windows.Forms;

    using DatabaseManager;
    using Microsoft.Office.Interop.Excel;
    using RoadCareDatabaseOperations;

    public class InputSummaryReport
    {
        private readonly string NetworkId;

        private readonly string SimulationId;

        private readonly string Network;

        private readonly string Simulation;

        public InputSummaryReport(
            string networkId,
            string simulationId,
            string network,
            string simulation)
        {
            this.NetworkId = networkId;
            this.SimulationId = simulationId;
            this.Network = network;
            this.Simulation = simulation;
        }

        public void CreateInputSummaryReport()
        {
            Report.XL.Visible = false;
            Report.XL.UserControl = false;

            Microsoft.Office.Interop.Excel._Workbook oWB =
                Report.CreateWorkBook();
            
            Microsoft.Office.Interop.Excel._Worksheet oSheet =
                (Microsoft.Office.Interop.Excel._Worksheet)oWB.ActiveSheet;

            Report.SheetPageSetup(oSheet, "Input Summary", 50d, 20d, 10d, "Database: " + DBMgr.NativeConnectionParameters.Database, DateTime.Now.ToLongDateString(), "Page &P", 1);
            Range oR = oSheet.get_Range("A1", "A1");

            object oEndCell = new object();
            string strReportName = "Input Summary Report";
            int sheetRow;
            DataSet dsPage = null,
                    dsNetwork = null,
                    dsSimulations = null,
                    dsPriority = null,
                    dsPriorityFund = null;
            DataSet dsTarget = null,
                    dsDeficient = null;

            try
            {
                dsPage = DBOp.QueryPageHeader(strReportName);
                dsNetwork = DBOp.GetNetworkDesc(NetworkId);
                dsSimulations = DBOp.QuerySimulations(SimulationId);
                dsPriority = DBOp.QueryPriority(SimulationId);
                dsPriorityFund = DBOp.QueryPriorityFund(SimulationId);
                dsTarget = DBOp.QueryTargets(SimulationId);
                dsDeficient = DBOp.QueryDeficients(SimulationId);
            }
            catch (Exception e)
            {
                throw e;
            }

            Cursor c = Cursor.Current;
            Cursor.Current = new Cursor(Cursors.WaitCursor.Handle);

            DataRow drPage = dsPage.Tables[0].Rows[0];
            DataRow drNetwork = dsNetwork.Tables[0].Rows[0];
            DataRow drSimulations = dsSimulations.Tables[0].Rows[0];

            string strMajorTitle = drPage["phText"].ToString();
            string strTemp, strFilter;

            #region report graphic
            // Place report graphic
            //string strPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\RoadCare Projects\\" + drPage["reportGraphicFile"].ToString();
			string strPath = ".\\" + drPage["reportGraphicFile"].ToString();
            Report.PlaceReportGraphic(strPath, oSheet.get_Range("A1", Missing.Value), oSheet);
            #endregion

            #region default column widths
            // set some column widths
            oR = oSheet.get_Range("A1:A1", Missing.Value);
            oR.ColumnWidth = 21;
            oR = oSheet.get_Range("B:D", Missing.Value);
            oR.ColumnWidth = 13.4;
            oR = oSheet.get_Range("E:H", Missing.Value);
            oR.ColumnWidth = 7;
            #endregion

            int ndx;
            // calculate the size for the array
            int aryCols =  2;
            int aryRows = 22;

            object[,] oData = new object[aryRows, aryCols];
            Report.ClearDataArray(ref oData);
            Hashtable titles = new Hashtable();

            #region General Info
            titles.Add("General Information", new Titles("General Information", 4, 1, false, "SECTION", 0));
            sheetRow = 4;
            // GENERAL INFO
            oData[0,1] = strMajorTitle;
            oData[3,0] = "General Information";
            oData[4,0] = "Database:";
            oData[4,1] = DBMgr.NativeConnectionParameters.Database;
            oData[5,0] = "Network Name:";
            oData[5,1] = Network;
            oData[6,0] = "Network Description:";
            oData[6,1] = drNetwork["Description"].ToString();
            oData[7, 0] = "Simulation Name:";
            oData[7, 1] = drSimulations["Simulation"].ToString();
            oData[8, 0] = "Simulation Description:";
            oData[8, 1] = drSimulations["Comments"].ToString();
            oData[9, 0] = "Created By:";
            oData[9, 1] = drSimulations["Username"].ToString();
            oData[10, 0] = "Created On:";
            strTemp = drSimulations["Date_created"].ToString();
            oData[10, 1] = Report.Left(strTemp, 9);

            sheetRow = 13;
            #endregion

            #region Analysis
            // ANALYSIS
            titles.Add("Analysis", new Titles("Analysis", sheetRow, 1, false, "SECTION", 0));
            oData[12, 0] = "Analysis";
            oData[13, 0] = "Optimization:";
            oData[13, 1] = drSimulations["Analysis"].ToString();
            oData[14, 0] = "Budget:";
            oData[14, 1] = drSimulations["Budget_constraint"].ToString();
            oData[15, 0] = "Weighting:";
            oData[15, 1] = drSimulations["Weighting"].ToString();
            oData[16, 0] = "Benefit:";
            oData[16, 1] = drSimulations["Benefit_variable"].ToString();
            oData[17, 0] = "Benefit Limit:";
            oData[17, 1] = drSimulations["Benefit_limit"].ToString();
            oData[18, 0] = "Jurisdiction Criteria:";
            oData[18, 1] = drSimulations["Jurisdiction"].ToString();
            #endregion

            #region Priority
            // PRIORITY
            sheetRow = 21;
            titles.Add("Priority", new Titles("Priority", sheetRow, 1, false, "GROUP",0));
            oData[20, 0] = "Priority";
            oEndCell = "A1";
            oEndCell = Report.WriteObjectArrayToExcel(oData, oSheet, oEndCell, false, false);

            int nCol;

            aryRows = DBMgr.ExecuteScalar("SELECT Count(*) FROM Priority WHERE simulationID = " + SimulationId) + 1;
            DataSet dsInvestments = DBOp.QueryInvestments(SimulationId);   // get the budgetorder
			string[] strBudgetOrder = dsInvestments.Tables[0].Rows[0].ItemArray[5].ToString().Split(',');
            aryCols = strBudgetOrder.Count() + 2;

            Hashtable criteraColumns = new Hashtable(); // used in formatting below
            criteraColumns.Add("Priority", aryCols);
            criteraColumns.Add("Target", 5);
            criteraColumns.Add("Deficient", 5);
            criteraColumns.Add("Performance", 4);
            criteraColumns.Add("Feasibility", 1);
            criteraColumns.Add("Cost", 3);
            criteraColumns.Add("Consequence", 3);

            Report.Resize2DArray(ref oData, aryRows, aryCols);
            Report.ClearDataArray(ref oData);

            sheetRow++;
            titles.Add("Priority_col", new Titles("Priority", sheetRow, aryCols, true, "COLUMN", aryRows - 1));
            sheetRow++;

            oData[0, 0] = "Priority";
            ndx = 1;
            foreach (string str in strBudgetOrder)
            {
                oData[0, ndx++] = str;
            }
            oData[0, ndx] = "Criteria";
            ndx = 1;
            foreach (DataRow dr in dsPriority.Tables[0].Rows)
            {
                oData[ndx, 0] = dr["PriorityLevel"].ToString();
                oData[ndx, aryCols-1] = dr["Criteria"].ToString();
                //oData[ndx, 4] = dr["Criteria"].ToString();
                nCol = 1;
                foreach(string str in strBudgetOrder) 
                {
                    strFilter = "priorityID = " + dr["priorityID"].ToString() + " and budget = '" + str + "'";
                    foreach (DataRow dr1 in dsPriorityFund.Tables[0].Select(strFilter))
                    {
                        oData[ndx, nCol] = dr1["funding"].ToString();
                    }
                    nCol++;
                }
                sheetRow++;
                ndx++;
            }

            oEndCell = "A" + Convert.ToString(Report.GetRowNumber(oEndCell.ToString()));
            oEndCell = Report.WriteObjectArrayToExcel(oData, oSheet, oEndCell, false,true);
            #endregion

            #region Target
            // TARGET
            sheetRow += 2;
            aryCols = 5;
            aryRows = dsTarget.Tables[0].Rows.Count + 2;
            Report.Resize2DArray(ref oData, aryRows, aryCols);
            Report.ClearDataArray(ref oData);

            titles.Add("Target", new Titles("Target", sheetRow - 1, aryCols, false, "GROUP", 0));
            titles.Add("Target_col", new Titles("Attribute", sheetRow, aryCols, true, "COLUMN", aryRows - 2));
            oData[0, 0] = "Target";
            oData[1, 0] = "Attribute";
            oData[1, 1] = "Name";
            oData[1, 2] = "Year";
            oData[1, 3] = "Target";
            oData[1, 4] = "Criteria";
            ndx = 2;

            foreach (DataRow dr in dsTarget.Tables[0].Rows)
            {
                nCol = 0;
                oData[ndx, nCol++] = dr["Attribute_"].ToString();
                oData[ndx, nCol++] = dr["TargetName"].ToString();
                oData[ndx, nCol++] = dr["Years"].ToString();
                oData[ndx, nCol++] = dr["TargetMean"].ToString();
                oData[ndx++, nCol++] = dr["Criteria"].ToString();
                sheetRow++;
            }

            oEndCell = "A" + Convert.ToString(Report.GetRowNumber(oEndCell.ToString()) + 2);
            oEndCell = Report.WriteObjectArrayToExcel(oData, oSheet, oEndCell, false, true);
            #endregion

            #region Deficient
            // DEFICIENT
            aryCols = 5;
            aryRows = dsDeficient.Tables[0].Rows.Count + 2;
            Report.Resize2DArray(ref oData, aryRows, aryCols);
            Report.ClearDataArray(ref oData);
            sheetRow += 2;
            titles.Add("Deficient", new Titles("Deficient", sheetRow, aryCols, false, "GROUP", 0));
            titles.Add("Deficient_col", new Titles("Attribute", sheetRow + 1, aryCols, true, "COLUMN", aryRows - 2));
            sheetRow++;

            oData[0, 0] = "Deficient";
            oData[1, 0] = "Attribute";
            oData[1, 1] = "Name";
            oData[1, 2] = "Deficient Level";
            oData[1, 3] = "Allowed Deficient (%)";
            oData[1, 4] = "Criteria";
            ndx = 2;
            foreach (DataRow dr in dsDeficient.Tables[0].Rows)
            {
                nCol = 0;
                oData[ndx, nCol++] = dr["Attribute_"].ToString();
                oData[ndx, nCol++] = dr["DeficientName"].ToString();
                oData[ndx, nCol++] = dr["Deficient"].ToString();
                oData[ndx, nCol++] = dr["PercentDeficient"].ToString();
                oData[ndx++, nCol++] = dr["Criteria"].ToString();
                sheetRow++;
            }

            oEndCell = "A" + Convert.ToString(Report.GetRowNumber(oEndCell.ToString()) + 2);
            oEndCell = Report.WriteObjectArrayToExcel(oData, oSheet, oEndCell, false,true);
            #endregion  // deficient

            #region Investments
            // INVESTMENTS
            aryCols = 2;
            aryRows = 5;
            Report.Resize2DArray(ref oData, aryRows, aryCols);
            Report.ClearDataArray(ref oData);
            sheetRow += 2;
            titles.Add("Investments", new Titles("Investment", sheetRow, 1, false, "SECTION", 0));
           
            oData[0,0] = "Investment";
            oData[1,0] = "Start Year:";
            oData[1,1] = dsInvestments.Tables[0].Rows[0].ItemArray[1].ToString();
            oData[2, 0] = "Analysis Period:";
            oData[2, 1] = dsInvestments.Tables[0].Rows[0].ItemArray[2].ToString();
            oData[3, 0] = "Inflation Rate:";
            oData[3, 1] = dsInvestments.Tables[0].Rows[0].ItemArray[3].ToString();
            oData[4, 0] = "Discount Rate:";
            oData[4, 1] = dsInvestments.Tables[0].Rows[0].ItemArray[4].ToString();

            oEndCell = "A" + Convert.ToString(Report.GetRowNumber(oEndCell.ToString()) + 2);
            oEndCell = Report.WriteObjectArrayToExcel(oData, oSheet, oEndCell, false, false);
            sheetRow += aryRows;
            #endregion

            #region Budget
            // Budget
			switch (DBMgr.NativeConnectionParameters.Provider)
			{
				case "MSSQL":
					aryRows = DBMgr.ExecuteScalar("SELECT Count(DISTINCT [Year_]) FROM YearlyInvestment WHERE simulationID = " + SimulationId) + 2;
					break;
				case "ORACLE":
					aryRows = DBMgr.ExecuteScalar("SELECT Count(DISTINCT Year_) FROM YearlyInvestment WHERE simulationID = " + SimulationId) + 2;
					break;
				default:
					throw new NotImplementedException("TODO: Create ANSI implementation for CreateInputSummaryReport()");
					//break;
			}
            aryCols = strBudgetOrder.Count() + 1;
            criteraColumns.Add("Budget", aryCols);
            sheetRow += 1;
            titles.Add("Budget", new Titles("Budget", sheetRow, aryCols, false, "GROUP", 0));
            sheetRow++;
            titles.Add("Years", new Titles("Years", sheetRow, aryCols, false, "COLUMN", aryRows - 2));
            sheetRow++;


            Report.Resize2DArray(ref oData, aryRows, aryCols);
            Report.ClearDataArray(ref oData);

            oData[0, 0] = "Budget";
            oData[1,0] = "Years";
            ndx = 1;
            foreach (string str in strBudgetOrder)
            {
                oData[1, ndx++] = str;
            }
            ndx = 2;
            DataSet dsYearlyInvestment = DBOp.QueryYearlyInvestment(SimulationId);
            strFilter = "SELECT DISTINCT Year_ FROM YearlyInvestment WHERE SimulationID = "+SimulationId;
            DataSet dsYearlyYears = DBMgr.ExecuteQuery(strFilter);

            foreach (DataRow dr in dsYearlyYears.Tables[0].Rows)
            {
                oData[ndx, 0] = dr["Year_"].ToString();
                nCol = 1;
                foreach (string str in strBudgetOrder)
                {
                    strFilter = "Year_ = " + dr["Year_"].ToString() + " AND Budgetname = '" + str + "'";
                    foreach (DataRow dr1 in dsYearlyInvestment.Tables[0].Select(strFilter))
                    {
                        oData[ndx, nCol] = dr1["Amount"].ToString();
                    }
                    nCol++;
                }
                ndx++;
                sheetRow++;
            }

            oEndCell = "A" + Convert.ToString(Report.GetRowNumber(oEndCell.ToString()) + 2);
            oEndCell = Report.WriteObjectArrayToExcel(oData, oSheet, oEndCell, false,true);
            #endregion

            #region Performance
            //PERFORMANCE
            aryRows = DBMgr.ExecuteScalar("SELECT Count(*) FROM Performance WHERE simulationID = " + SimulationId) + 2;
            aryCols = 4;
            Report.Resize2DArray(ref oData, aryRows, aryCols);
            Report.ClearDataArray(ref oData);
            sheetRow += 1;
            titles.Add("Performance", new Titles("Performance", sheetRow, aryCols, false, "SECTION", 0));
            sheetRow++;
            titles.Add("Performance_col", new Titles("Attribute", sheetRow, aryCols, true, "COLUMN", aryRows - 2));
            sheetRow++;

            oData[0, 0] = "Performance";
            oData[1, 0] = "Attribute";
            oData[1, 1] = "Equation Name";
            oData[1, 2] = "Equation";
            oData[1, 3] = "Criteria";

            DataSet dsPerformance = DBOp.QueryPerformance(SimulationId);
            ndx = 2;
            foreach (DataRow dr in dsPerformance.Tables[0].Rows)
            {
                nCol = 0;
                oData[ndx, nCol++] = dr["Attribute_"].ToString();
                oData[ndx, nCol++] = dr["EquationName"].ToString();
                oData[ndx, nCol++] = dr["Equation"].ToString();
                oData[ndx++, nCol++] = dr["Criteria"].ToString();
                sheetRow++;
            }

            oEndCell = "A" + Convert.ToString(Report.GetRowNumber(oEndCell.ToString()) + 2);
            oEndCell = Report.WriteObjectArrayToExcel(oData, oSheet, oEndCell, false, true);
            #endregion

            #region Treatment
            // TREATMENT
            DataSet dsTreatments = DBOp.QueryTreatments(SimulationId);
            DataSet dsFeasibility = DBOp.QueryFeasibility(SimulationId);
            DataSet dsCosts = DBOp.QueryCosts(SimulationId);
            DataSet dsConsequences = DBOp.QueryConsequences(SimulationId);

            int blankLines = 5, grpHeaders = 3, colHeaders = 3, title = 1, preamble = 5;
            aryRows = (blankLines + grpHeaders + colHeaders + title + preamble) * dsTreatments.Tables[0].Rows.Count;
            aryRows += dsFeasibility.Tables[0].Rows.Count + dsCosts.Tables[0].Rows.Count + dsConsequences.Tables[0].Rows.Count;
            aryCols = 3;
            Report.Resize2DArray(ref oData, aryRows, aryCols);
            Report.ClearDataArray(ref oData);
            sheetRow++;
            titles.Add("Treatment", new Titles("Treatment", sheetRow, aryCols, false, "SECTION", 0));
            sheetRow++;
            int treatNum = 1;   // keep track of where each treatment section begins
            string strGrid = "";
            List<string> gridList = new List<string>();

            oData[0, 0] = "Treatment";
            ndx = 2;
            sheetRow++;
            foreach (DataRow dr in dsTreatments.Tables[0].Rows)
            {
                titles.Add("Treatment"+treatNum.ToString(), new Titles("Name", sheetRow, 1, false, "BOLD", 4));
                oData[ndx, 0] = "Treatment Name:";
                oData[ndx++, 1] = dr["Treatment"].ToString();
                sheetRow++;
                oData[ndx, 0] = "Description:";
                oData[ndx++, 1] = dr["Description"].ToString();
                sheetRow++;
                oData[ndx, 0] = "Budget:";
                oData[ndx++, 1] = dr["Budget"].ToString();
                sheetRow++;
                oData[ndx, 0] = "Years Before Any:";
                sheetRow++;
                oData[ndx++, 1] = dr["BeforeAny"].ToString();
                sheetRow++;
                oData[ndx, 0] = "Years Before Same:";
                oData[ndx++, 1] = dr["BeforeSame"].ToString();
                sheetRow++;

                //Feasibility
                ndx++;
                int count = DBMgr.ExecuteScalar("SELECT Count(*) FROM Feasibility WHERE TreatmentID = " + dr["TreatmentID"].ToString());
                titles.Add("Feasibility" + treatNum.ToString() + "grp", new Titles("Feasibility", sheetRow, 1, false, "GROUP", 0));
                oData[ndx++, 0] = "Feasibility";
                sheetRow++;
                titles.Add("Feasibility" + treatNum.ToString() + "col", new Titles("Criteria", sheetRow, 1, true, "COLUMN", count));
                oData[ndx++, 0] = "Criteria";
                sheetRow++;
                if (count == 0)
                {
                    aryRows += 2;
                    ndx++;
                    sheetRow++;
                }
                else
                {
                    //aryRows += count + 1;
                    //Report.Resize2DArrayKeepData(ref oData, aryRows, aryCols);
                    strFilter = "TreatmentID = " + dr["TreatmentID"].ToString();
                    foreach (DataRow dr1 in dsFeasibility.Tables[0].Select(strFilter))
                    {
                        Report.BuildRange(ref strGrid, ref gridList, sheetRow, 'A', 7);
                        //strGrid += "A" + sheetRow.ToString() + ":H" + sheetRow.ToString() + ",";
                        //string s = "A" + sheetRow.ToString() + ":H" + sheetRow.ToString();
                        //oR = oSheet.get_Range(s, Missing.Value);
                        //oR.NumberFormat = "@";
                        oData[ndx++, 0] = dr1["Criteria"].ToString();
                        sheetRow++;
                    }
                }
                ndx++;
                sheetRow++;

                // Costs
                titles.Add("Cost" + treatNum.ToString() + "grp", new Titles("Cost", sheetRow, 1, false, "GROUP", 0));
                oData[ndx++, 0] = "Cost";
                sheetRow++;
                count = DBMgr.ExecuteScalar("SELECT Count(*) FROM Costs WHERE TreatmentID = " + dr["TreatmentID"].ToString());
                titles.Add("Cost" + treatNum.ToString() + "col", new Titles("Cost", sheetRow, 3, true, "COLUMN", count));
                oData[ndx, 0] = "Cost";
                oData[ndx, 1] = "Units";
                oData[ndx++, 2] = "Criteria";
                sheetRow++;

                if (count == 0)
                {
                    ndx++;
                    sheetRow++;
                }
                else
                {
                    strFilter = "TreatmentID = " + dr["TreatmentID"].ToString();
                    foreach (DataRow dr1 in dsCosts.Tables[0].Select(strFilter))
                    {
                        Report.BuildRange(ref strGrid, ref gridList, sheetRow, 'A', 7);
                        //strGrid += "A" + sheetRow.ToString() + ":H" + sheetRow.ToString() + ",";
                        //string s = "A" + sheetRow.ToString() + ":H" + sheetRow.ToString();
                        //oR = oSheet.get_Range(s, Missing.Value);
                        //oR.NumberFormat = "@";
                        oData[ndx, 0] = dr1["Cost_"];
                        oData[ndx, 1] = dr1["Unit"].ToString();
                        oData[ndx++, 2] = dr1["Criteria"].ToString();
                        sheetRow++;
                    }
                }
                ndx++;
                sheetRow++;

                // Consequences
                titles.Add("Consequence" + treatNum.ToString() + "grp", new Titles("Consequence", sheetRow, 1, false, "GROUP", 0));
                oData[ndx++, 0] = "Consequence";
                sheetRow++;
                count = DBMgr.ExecuteScalar("SELECT Count(*) FROM Consequences WHERE TreatmentID = " + dr["TreatmentID"].ToString());
                titles.Add("Consequence" + treatNum.ToString() + "col", new Titles("Consequence", sheetRow, 3, true, "COLUMN", count));
                oData[ndx, 0] = "Attribute";
                oData[ndx, 1] = "Change";
                oData[ndx++, 2] = "Criteria";
                sheetRow++;

                if (count == 0)
                {
                    ndx++;
                    sheetRow++;
                }
                else
                {
                    strFilter = "TreatmentID = " + dr["TreatmentID"].ToString();
                    foreach (DataRow dr1 in dsConsequences.Tables[0].Select(strFilter))
                    {
                        Report.BuildRange(ref strGrid, ref gridList, sheetRow, 'A', 7);
                        //strGrid += "A" + sheetRow.ToString() + ":H" + sheetRow.ToString() + ",";
                        string s = "B" + sheetRow.ToString() + ":B" + sheetRow.ToString();
                        oR = oSheet.get_Range(s, Missing.Value);
                        oR.NumberFormat = "@";
                        oData[ndx, 0] = dr1["Attribute_"].ToString();
                        oData[ndx, 1] = dr1["Change_"].ToString();
                        oData[ndx++, 2] = dr1["Criteria"].ToString();
                        sheetRow++;
                    }
                }
                ndx++;
                sheetRow++;
                treatNum++;
            }   // end foreach


            oEndCell = "A" + Convert.ToString(Report.GetRowNumber(oEndCell.ToString()) + 2);
            oEndCell = Report.WriteObjectArrayToExcel(oData, oSheet, oEndCell, false, false);
            #endregion

            // Apply formatting
            List<string> sectionList = new List<string>();
            List<string> groupList = new List<string>();
            List<string> columnList = new List<string>();
            List<string> criteriaList = new List<string>();
            List<string> boldList = new List<string>();
            //List<string> gridList = new List<string>();
            string strSection = "", strGroup = "", strColumn = "", strCriteria = "", strBold = "";

            #region pageheader
            // PAGEHEADER
            string strRange = "B1:H1";
            DataRow drPgHdr = dsPage.Tables[0].Rows[0];
            Report.FormatHeaders(oR, drPgHdr, oSheet, "ph", strRange);
            #endregion

            #region bold
            // Set bold text at known locations
            object o = titles["Investments"];
            if (o != null)
            {
                Titles bold = (Titles)o;
                int nTmp = bold.Row;
                strRange = "A" + (nTmp + 1).ToString() + ":A" + (nTmp + 4).ToString();
                strRange += ",A1:A11,A14:A19";
                oR = oSheet.get_Range(strRange, Missing.Value);
                oR.Font.Bold = true;
                oR.Font.Size = 11;
            }
            #endregion

            #region Format headers
            // Format headers
            foreach (DictionaryEntry de in titles)
            {
                Titles t = (Titles)de.Value;
                int nSize = t.NumberOfColumns < 9 ? 8 : t.NumberOfColumns;
                if (t.TitleType == "SECTION")
                {
                    Report.BuildRange(ref strSection, ref sectionList, t.Row, 'A', nSize - 1);
                }
                else if (t.TitleType == "GROUP")
                {
                    if ((string)de.Key == "Budget") nSize = t.NumberOfColumns;
                    Report.BuildRange(ref strGroup, ref groupList, t.Row, 'A', nSize - 1);
                }
                else if (t.TitleType == "BOLD")
                {
                    strBold += ("A" + t.Row.ToString() + ":A" + (t.Row + t.NumberOfDataRows).ToString() + ",");

                }
                else if (t.TitleType == "COLUMN")
                {
                    if ((string)de.Key == "Years") nSize = t.NumberOfColumns;
                    Report.BuildRange(ref strColumn, ref columnList, t.Row, 'A', nSize - 1);
                    if (t.Criteria)
                    {
                        string s = Report.GetColumnLetter(t.NumberOfColumns);
                        char[] chr = s.ToCharArray(0, 1);

                        char asciiChar = chr[0];
                        int asciiValue = (int)asciiChar;
                        asciiValue += nSize - t.NumberOfColumns;
                        asciiChar = (char)asciiValue;

                        strCriteria = Report.Left(s, 1) + t.Row.ToString() + ":" + asciiChar.ToString() + t.Row.ToString();
                        criteriaList.Add(strCriteria);

                        for (int i = 1; i <= t.NumberOfDataRows; i++)
                        {
                            int gridrow = t.Row + i;
                            strCriteria = Report.Left(s, 1) + gridrow.ToString() + ":" + asciiChar.ToString() + gridrow.ToString();
                            criteriaList.Add(strCriteria);

                        }
                    }
                }
            }
            Report.EndRangeList(ref strSection, ref sectionList);
            Report.EndRangeList(ref strGroup, ref groupList);
            Report.EndRangeList(ref strColumn, ref columnList);
            Report.EndRangeList(ref strCriteria, ref criteriaList);
            Report.EndRangeList(ref strBold, ref boldList);
            Report.EndRangeList(ref strGrid, ref gridList);
            foreach (string s in boldList)
            {
                oR = oSheet.get_Range(s, Missing.Value);
                oR.Font.Bold = true;
            }
            foreach (string s in sectionList)
            {
                oR = oSheet.get_Range(s, Missing.Value);
                oR.MergeCells = true;
                oR.HorizontalAlignment = XlHAlign.xlHAlignCenter;
                oR.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Navy);
                oR.Font.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.White);
                oR.Font.Size = 13;
                oR.Font.Bold = true;
            }
            foreach (string s in groupList)
            {
                oR = oSheet.get_Range(s, Missing.Value);
                oR.MergeCells = true;
                oR.HorizontalAlignment = XlHAlign.xlHAlignCenter;
                oR.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Navy);
                oR.Font.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.White);
                oR.Font.Size = 11;
                oR.Font.Bold = true;
                oR.Borders.get_Item(XlBordersIndex.xlEdgeTop).LineStyle = XlLineStyle.xlContinuous;
                oR.Borders.get_Item(XlBordersIndex.xlEdgeBottom).LineStyle = XlLineStyle.xlContinuous;
                oR.Borders.get_Item(XlBordersIndex.xlEdgeLeft).LineStyle = XlLineStyle.xlContinuous;
                oR.Borders.get_Item(XlBordersIndex.xlEdgeRight).LineStyle = XlLineStyle.xlContinuous;
            }
            foreach (string s in columnList)
            {
                oR = oSheet.get_Range(s, Missing.Value);
                oR.HorizontalAlignment = XlHAlign.xlHAlignCenter;
                oR.VerticalAlignment = XlVAlign.xlVAlignBottom;
                oR.WrapText = true;
                oR.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.LightGray);
                oR.Font.Size = 11;
                oR.Font.Bold = true;
                oR.Borders.get_Item(XlBordersIndex.xlEdgeTop).LineStyle = XlLineStyle.xlContinuous;
                oR.Borders.get_Item(XlBordersIndex.xlEdgeBottom).LineStyle = XlLineStyle.xlContinuous;
                oR.Borders.get_Item(XlBordersIndex.xlInsideHorizontal).LineStyle = XlLineStyle.xlContinuous;
                oR.Borders.get_Item(XlBordersIndex.xlInsideVertical).LineStyle = XlLineStyle.xlContinuous;
            }
            foreach (string s in criteriaList)
            {
                oR = oSheet.get_Range(s, Missing.Value);
                oR.MergeCells = true;
                oR.HorizontalAlignment = XlHAlign.xlHAlignCenter;
                oR.VerticalAlignment = XlVAlign.xlVAlignBottom;
                oR.WrapText = true;
                oR.Borders.LineStyle = XlLineStyle.xlContinuous;
                oR.Borders.Weight = XlBorderWeight.xlThin;
                oR.Borders.get_Item(XlBordersIndex.xlEdgeTop).LineStyle = XlLineStyle.xlContinuous;
                oR.Borders.get_Item(XlBordersIndex.xlEdgeBottom).LineStyle = XlLineStyle.xlContinuous;
                oR.Borders.get_Item(XlBordersIndex.xlInsideHorizontal).LineStyle = XlLineStyle.xlContinuous;
                oR.Borders.get_Item(XlBordersIndex.xlInsideVertical).LineStyle = XlLineStyle.xlContinuous;
            }
            foreach (string s in gridList)
            {
                oR = oSheet.get_Range(s, Missing.Value);
                oR.Borders.LineStyle = XlLineStyle.xlContinuous;
                oR.Borders.Weight = XlBorderWeight.xlThin;
                oR.Borders.get_Item(XlBordersIndex.xlInsideHorizontal).LineStyle = XlLineStyle.xlContinuous;
                oR.Borders.get_Item(XlBordersIndex.xlInsideVertical).LineStyle = XlLineStyle.xlContinuous;
                oR.Borders.get_Item(XlBordersIndex.xlEdgeBottom).LineStyle = XlLineStyle.xlContinuous;
            }
            #endregion

            #region Wide cells
            // Format wide cells
            strRange = "B6:H6,B7:H7,B8:H8,B9:H9,B10:H10,B19:H19";
            oR = oSheet.get_Range(strRange, Missing.Value);
            oR.MergeCells = true;
            oR.WrapText = true;
            oR.VerticalAlignment = XlVAlign.xlVAlignTop;
            #endregion

            Report.XL.Visible = true;
            Report.XL.UserControl = true;
        }
    }   // end class

    internal class Titles
    {
        public string Name { get; private set; }

        public int Row { get; private set; }

        public int NumberOfColumns { get; private set; }

        public bool Criteria { get; private set; }

        public string TitleType { get; private set; }

        public int NumberOfDataRows { get; private set; }

        public Titles(
            string name,
            int row,
            int cols,
            bool criteria,
            string type,
            int datarows)
        {
            this.Name = name;
            this.Row = row;
            this.NumberOfColumns = cols;
            this.Criteria = criteria;
            this.TitleType = type;
            this.NumberOfDataRows = datarows;
        }
    }
}   // end namespace
