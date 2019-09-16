using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataAccessLayer.MSSQL;
using System.Data.SqlClient;

namespace DataAccessLayer
{
    /// <summary>
    /// This class contains functions to check and modify the necessary DE database schema.
    /// </summary>
    public static class SchemaBuilder
    {
        /// <summary>
        /// This is an example of how to dynamically create a table and modify its contents.
        /// </summary>
        /// <returns></returns>
        public static bool CheckExampleTable()
        {
            try
            {
                List<SQLColumn> columnList;
                //See if table exists
                if (DB.CheckIfTableExists("EXAMPLE_TABLE") != 1)
                {
                    columnList = new List<SQLColumn>();
                    columnList.Add(new SQLColumn("EXAMPLE_ID", "int", false));
                    columnList.Add(new SQLColumn("DISPLAY_NAME", "VARCHAR(50)", true));
                    if (!DB.CreateTable("EXAMPLE_TABLE", columnList)) return false;
                }

                columnList = DB.GetTableColumns("EXAMPLE_TABLE");
                SQLColumn column = columnList.Find(delegate(SQLColumn sqlColumn) { return sqlColumn.name == "PRIVATE_NAME"; });//Example of adding new column
                if (column == null)
                {
                    if (DB.AlterTable("EXAMPLE_TABLE ADD PRIVATE_NAME VARCHAR(50)"))
                    {
                        columnList = DB.GetTableColumns("EXAMPLE_TABLE");//Get the columns again
                        column = columnList.Find(delegate(SQLColumn sqlColumn) { return sqlColumn.name == "PRIVATE_NAME"; });//Make sure column add was sucessful.  If not return.
                    }
                    else
                    {
                        return false;
                    }
                }

                //Example of changing a column later.
                if (column != null && column.max_length != 255)
                {
                    if (!DB.AlterTable("EXAMPLE_TABLE ALTER COLUMN PRIVATE_NAME VARCHAR(255)")) return false;
                }

            }
            catch (Exception e)
            {
                Utility.ExceptionHandling.DataAccessExceptionHandler.HandleException(e, "Error in example table check", false);
                return false;
            }
            return true;
        }

        /// <summary>
        /// Generates the table to accept the results of the simulation.
        /// </summary>
        /// <param name="simulationID">Assigned when creating a new simulation.  From simulation table</param>
        /// <param name="columnList">This is a list of assets and properties returned from cartegraph.  Need name and type in SQLColumn</param>
        /// <returns></returns>
        public static bool CreateSimulationResultTable(int simulationID, List<SQLColumn> columnList)
        {
            string table = "rc_SIMULATION_" + simulationID.ToString();//Name of table which is rc_SIMULATION + simulationID
            if (DB.CheckIfTableExists(table) == 1)//When creating table is necessary. Drop old table.  It isn't any good.
            {
                if (!DB.DropTable(table)) return false;//If attempting to drop old table and fails.  Cannot create new table.
            }//if table does not exist, go ahead and create a new table.

            columnList.Add(new SQLColumn("UID", "int", false));//UniqueID of OMS asset.
            columnList.Add(new SQLColumn("SCENARIO_ID", "int", false));//Running multiple what-ifs.  All saved in same table.
            columnList.Add(new SQLColumn("TIME_STEP", "int", false));//Time step, usually years, but this allows sub-year periods.
            if (!DB.CreateTable(table, columnList)) return false;//If table is not created cannot proceed.
            return true;
        }

        /// <summary>
        /// Checks and updates the SIMULATIONS table. Creates if doesn't find.
        /// </summary>
        /// <returns>True if successful, false if failed</returns>
        public static bool CheckSimulationTable()
        {
            List<SQLColumn> columnList = new List<SQLColumn>();
            string table =  DB.TablePrefix + "SIMULATIONS";
            if (DB.CheckIfTableExists(table) == 0)
            {
                columnList.Add(new SQLColumn("SIMULATIONID", "int", false,true));
                if (!DB.CreateTable(table, columnList)) return false;//If table is not created cannot proceed.
            }
            //Get list of existing columns
            columnList = DB.GetTableColumns(table);
            DB.CheckSqlColumn(table, "NETWORKID", "int", -1, true, columnList);
            DB.CheckSqlColumn(table, "SIMULATION", "varchar", 50, false, columnList);
            DB.CheckSqlColumn(table, "COMMENTS", "varchar", 255, true, columnList);
            DB.CheckSqlColumn(table, "DATECREATED", "datetime", -1, true, columnList);
            DB.CheckSqlColumn(table, "CREATORID", "varchar", 50, true, columnList);
            DB.CheckSqlColumn(table, "USERNAME", "varchar", 200, true, columnList);
            DB.CheckSqlColumn(table, "PERMISSIONS", "int", -1, true, columnList);
            DB.CheckSqlColumn(table, "JURISDICTION", "varchar", 4000, true, columnList);
            DB.CheckSqlColumn(table, "ANALYSIS", "varchar", 50, true, columnList);
            DB.CheckSqlColumn(table, "BUDGET_CONSTRAINT", "varchar", 50, true, columnList);
            DB.CheckSqlColumn(table, "WEIGHTING", "varchar", 50, true, columnList);
            DB.CheckSqlColumn(table, "BENEFIT_VARIABLE", "varchar", 50, true, columnList);
            DB.CheckSqlColumn(table, "BENEFIT_LIMIT", "float", -1, true, columnList);
            DB.CheckSqlColumn(table, "COMMITTED_START", "int", -1, true, columnList);
            DB.CheckSqlColumn(table, "COMMITTED_PERIOD", "int", -1, true, columnList);
            DB.CheckSqlColumn(table, "SIMULATION_VARIABLES", "varchar", 4000, true, columnList);
            //Beginning of OMS specific inputs.
            DB.CheckSqlColumn(table, "RUN_TIME", "float", -1, true, columnList);
            DB.CheckSqlColumn(table, "ASSET_TYPE", "varchar", 50, true, columnList);
            DB.CheckSqlColumn(table, "USE_TARGET", "bit", -1, true, columnList);
            DB.CheckSqlColumn(table, "TARGET_OCI", "float", -1, true, columnList);
            DB.CheckSqlColumn(table, "DEFICIENT", "float", -1, true, columnList);
            DB.CheckSqlColumn(table, "PERCENT_DEFICIENT", "float", -1, true, columnList);
            DB.CheckSqlColumn(table, "SIMULATION_AREA", "varchar", 4000, true, columnList);

            string primaryKey = DB.GetPrimaryKey(table);
            if (primaryKey == null)
            {
                if (!DB.AlterTable(table + " ADD CONSTRAINT [PK_SIMULATIONS] PRIMARY KEY (SIMULATIONID)")) return false;
            }
            return true;
        }

        /// <summary>
        /// Add/update PERFORMANCE table in Decision Engine table structure.
        /// </summary>
        /// <returns></returns>
        public static bool CheckPerformanceTable()
        {
            List<SQLColumn> columnList = new List<SQLColumn>();
            string table = DB.TablePrefix + "PERFORMANCE";
            if (DB.CheckIfTableExists(table) == 0)
            {
                columnList.Add(new SQLColumn("PERFORMANCEID", "int", false,true));
                if (!DB.CreateTable(table, columnList)) return false;//If table is not created cannot proceed.
            }
            //Get list of existing columns
            columnList = DB.GetTableColumns(table);
            DB.CheckSqlColumn(table, "SIMULATIONID", "int", -1, false, columnList);
            DB.CheckSqlColumn(table, "ATTRIBUTE_", "varchar", 50, false, columnList);
            DB.CheckSqlColumn(table, "EQUATIONNAME", "varchar", 50, true, columnList);
            DB.CheckSqlColumn(table, "CRITERIA", "varchar", 4000, true, columnList);
            DB.CheckSqlColumn(table, "EQUATION", "varchar", 4000, true, columnList);
            DB.CheckSqlColumn(table, "SHIFT", "bit", -1, true, columnList);
            DB.CheckSqlColumn(table, "BINARY_EQUATION", "varbinary", int.MaxValue, true, columnList);
            DB.CheckSqlColumn(table, "BINARY_CRITERIA", "varbinary", int.MaxValue, true, columnList);
            DB.CheckSqlColumn(table, "PIECEWISE", "bit", -1, true, columnList);

            SPHelp spHelp = new SPHelp(table);

            if (spHelp.PrimaryKey == null)
            {
                if (!DB.AlterTable(table + " ADD CONSTRAINT [PK_" + DB.TablePrefix + "PERFORMANCE] PRIMARY KEY (PERFORMANCEID)")) return false;
            }

            Constraint constraintSimulation = null;
            if(spHelp.ForeignKeys != null) constraintSimulation = spHelp.ForeignKeys.Find(delegate(Constraint co) { return co.constraint_keys == "SIMULATIONID"; });
            if(constraintSimulation == null)
            {
                if (!DB.AlterTable(table + " WITH CHECK ADD CONSTRAINT [FK_" + DB.TablePrefix + "PERFORMANCE_" + DB.TablePrefix + "SIMULATIONS] FOREIGN KEY([SIMULATIONID])"
                    + " REFERENCES [dbo].[" + DB.TablePrefix +"SIMULATIONS] ([SIMULATIONID]) ON UPDATE CASCADE ON DELETE CASCADE"))return false;
            }

            return true;
        }



        /// <summary>
        /// Add/update TREATEMENTS table in Decision Engine table structure.
        /// </summary>
        /// <returns></returns>
        public static bool CheckTreatmentTable()
        {
            List<SQLColumn> columnList = new List<SQLColumn>();
            string table = DB.TablePrefix + "TREATMENTS";
            if (DB.CheckIfTableExists(table) == 0)
            {
                columnList.Add(new SQLColumn("TREATMENTID", "int", false, true));
                if (!DB.CreateTable(table, columnList)) return false;//If table is not created cannot proceed.
            }
            //Get list of existing columns
            columnList = DB.GetTableColumns(table);
            DB.CheckSqlColumn(table, "SIMULATIONID", "int", -1, false, columnList);
            DB.CheckSqlColumn(table, "TREATMENT", "varchar", 50, false, columnList);
            DB.CheckSqlColumn(table, "BEFOREANY", "int", -1, false, columnList);
            DB.CheckSqlColumn(table, "BEFORESAME", "int", -1, false, columnList);
            DB.CheckSqlColumn(table, "BUDGET", "varchar", 512, true, columnList);
            DB.CheckSqlColumn(table, "DESCRIPTION", "varchar", 4000, true, columnList);
            DB.CheckSqlColumn(table, "OMS_IS_REPEAT", "bit", -1, true, columnList);
            DB.CheckSqlColumn(table, "OMS_REPEAT_START", "int", -1, true, columnList);
            DB.CheckSqlColumn(table, "OMS_REPEAT_INTERVAL", "int", -1, true, columnList);
            DB.CheckSqlColumn(table, "OMS_OID", "varchar", 50, true, columnList);
            DB.CheckSqlColumn(table, "OMS_IS_SELECTED", "bit", -1, true, columnList);
            DB.CheckSqlColumn(table, "OMS_IS_EXCLUSIVE", "bit", -1, true, columnList);


            SPHelp spHelp = new SPHelp(table);

            if (spHelp.PrimaryKey == null)
            {
                if (!DB.AlterTable(table + " ADD CONSTRAINT [PK_" + DB.TablePrefix + "TREATMENTS] PRIMARY KEY (TREATMENTID)")) return false;
            }

            Constraint constraintSimulation = null;
            if (spHelp.ForeignKeys != null) constraintSimulation = spHelp.ForeignKeys.Find(delegate(Constraint co) { return co.constraint_keys == "SIMULATIONID"; });
            if (constraintSimulation == null)
            {
                if (!DB.AlterTable(table + " WITH CHECK ADD CONSTRAINT [FK_" + DB.TablePrefix + "TREATMENTS_" + DB.TablePrefix + "SIMULATIONS] FOREIGN KEY([SIMULATIONID])"
                    + " REFERENCES [dbo].[" + DB.TablePrefix + "SIMULATIONS] ([SIMULATIONID]) ON UPDATE CASCADE ON DELETE CASCADE")) return false;
            }

            return true;
        }

        /// <summary>
        /// Add/update FEASIBILITY table in Decision Engine table structure.
        /// </summary>
        /// <returns></returns>
        public static bool CheckFeasibilityTable()
        {
            List<SQLColumn> columnList = new List<SQLColumn>();
            string table = DB.TablePrefix + "FEASIBILITY";
            if (DB.CheckIfTableExists(table) == 0)
            {
                columnList.Add(new SQLColumn("FEASIBILITYID", "int", false, true));
                if (!DB.CreateTable(table, columnList)) return false;//If table is not created cannot proceed.
            }
            //Get list of existing columns
            columnList = DB.GetTableColumns(table);
            DB.CheckSqlColumn(table, "TREATMENTID", "int", -1, false, columnList);
            DB.CheckSqlColumn(table, "CRITERIA", "varchar", 4000, true, columnList);
            DB.CheckSqlColumn(table, "BINARY_CRITERIA", "varbinary", int.MaxValue, true, columnList);
          
            SPHelp spHelp = new SPHelp(table);

            if (spHelp.PrimaryKey == null)
            {
                if (!DB.AlterTable(table + " ADD CONSTRAINT [PK_" + DB.TablePrefix + "FEASIBILITY] PRIMARY KEY (FEASIBILITYID)")) return false;
            }

            Constraint constraintSimulation = null;
            if (spHelp.ForeignKeys != null) constraintSimulation = spHelp.ForeignKeys.Find(delegate(Constraint co) { return co.constraint_keys == "TREATMENTID"; });
            if (constraintSimulation == null)
            {
                if (!DB.AlterTable(table + " WITH CHECK ADD CONSTRAINT [FK_" + DB.TablePrefix + "FEASIBILITY_" + DB.TablePrefix + "TREATMENTS] FOREIGN KEY([TREATMENTID])"
                    + " REFERENCES [dbo].[" + DB.TablePrefix + "TREATMENTS] ([TREATMENTID]) ON UPDATE CASCADE ON DELETE CASCADE")) return false;
            }

            return true;
        }


        /// <summary>
        /// Add/update COSTS table in Decision Engine table structure.
        /// </summary>
        /// <returns></returns>
        public static bool CheckCostsTable()
        {
            List<SQLColumn> columnList = new List<SQLColumn>();
            string table = DB.TablePrefix + "COSTS";
            if (DB.CheckIfTableExists(table) == 0)
            {
                columnList.Add(new SQLColumn("COSTID", "int", false, true));
                if (!DB.CreateTable(table, columnList)) return false;//If table is not created cannot proceed.
            }
            //Get list of existing columns
            columnList = DB.GetTableColumns(table);
            DB.CheckSqlColumn(table, "TREATMENTID", "int", -1, false, columnList);
            DB.CheckSqlColumn(table, "COST_", "varchar", 4000, true, columnList);
            DB.CheckSqlColumn(table, "UNIT", "varchar", 50, true, columnList);
            DB.CheckSqlColumn(table, "CRITERIA", "varchar", 4000, true, columnList);
            DB.CheckSqlColumn(table, "BINARY_CRITERIA", "varbinary", int.MaxValue, true, columnList);
            DB.CheckSqlColumn(table, "BINARY_COST", "varbinary", int.MaxValue, true, columnList);

            SPHelp spHelp = new SPHelp(table);

            if (spHelp.PrimaryKey == null)
            {
                if (!DB.AlterTable(table + " ADD CONSTRAINT [PK_" + DB.TablePrefix + "COSTS] PRIMARY KEY (COSTID)")) return false;
            }

            Constraint constraintSimulation = null;
            if (spHelp.ForeignKeys != null) constraintSimulation = spHelp.ForeignKeys.Find(delegate(Constraint co) { return co.constraint_keys == "TREATMENTID"; });
            if (constraintSimulation == null)
            {
                if (!DB.AlterTable(table + " WITH CHECK ADD CONSTRAINT [FK_" + DB.TablePrefix + "COSTS_" + DB.TablePrefix + "TREATMENTS] FOREIGN KEY([TREATMENTID])"
                    + " REFERENCES [dbo].[" + DB.TablePrefix + "TREATMENTS] ([TREATMENTID]) ON UPDATE CASCADE ON DELETE CASCADE")) return false;
            }

            return true;
        }


        /// <summary>
        /// Add/update CONSEQUENCES table in Decision Engine table structure.
        /// </summary>
        /// <returns></returns>
        public static bool CheckConsequencesTable()
        {
            List<SQLColumn> columnList = new List<SQLColumn>();
            string table = DB.TablePrefix + "CONSEQUENCES";
            if (DB.CheckIfTableExists(table) == 0)
            {
                columnList.Add(new SQLColumn("CONSEQUENCEID", "int", false, true));
                if (!DB.CreateTable(table, columnList)) return false;//If table is not created cannot proceed.
            }
            //Get list of existing columns
            columnList = DB.GetTableColumns(table);
            DB.CheckSqlColumn(table, "TREATMENTID", "int", -1, false, columnList);
            DB.CheckSqlColumn(table, "ATTRIBUTE_", "varchar", 50, true, columnList);
            DB.CheckSqlColumn(table, "CHANGE_", "varchar", 4000, true, columnList);
            DB.CheckSqlColumn(table, "CRITERIA", "varchar", 4000, true, columnList);
            DB.CheckSqlColumn(table, "BINARY_CRITERIA", "varbinary", int.MaxValue, true, columnList);
            DB.CheckSqlColumn(table, "EQUATION", "varchar", 4000, true, columnList);

            SPHelp spHelp = new SPHelp(table);

            if (spHelp.PrimaryKey == null)
            {
                if (!DB.AlterTable(table + " ADD CONSTRAINT [PK_" + DB.TablePrefix + "CONSEQUENCES] PRIMARY KEY (CONSEQUENCEID)")) return false;
            }

            Constraint constraintSimulation = null;
            if (spHelp.ForeignKeys != null) constraintSimulation = spHelp.ForeignKeys.Find(delegate(Constraint co) { return co.constraint_keys == "TREATMENTID"; });
            if (constraintSimulation == null)
            {
                if (!DB.AlterTable(table + " WITH CHECK ADD CONSTRAINT [FK_" + DB.TablePrefix + "CONSEQUENCES_" + DB.TablePrefix + "TREATMENTS] FOREIGN KEY([TREATMENTID])"
                    + " REFERENCES [dbo].[" + DB.TablePrefix + "TREATMENTS] ([TREATMENTID]) ON UPDATE CASCADE ON DELETE CASCADE")) return false;
            }

            return true;
        }

        /// <summary>
        /// Add/update COMMITTED_ table in Decision Engine table structure.
        /// </summary>
        /// <returns></returns>
        public static bool CheckCommittedTable()
        {
            List<SQLColumn> columnList = new List<SQLColumn>();
            string table = DB.TablePrefix + "COMMITTED_";
            if (DB.CheckIfTableExists(table) == 0)
            {
                columnList.Add(new SQLColumn("COMMITID", "int", false, true));
                if (!DB.CreateTable(table, columnList)) return false;//If table is not created cannot proceed.
            }
            //Get list of existing columns
            columnList = DB.GetTableColumns(table);
            DB.CheckSqlColumn(table, "SIMULATIONID", "int", -1, false, columnList);
            DB.CheckSqlColumn(table, "SECTIONID", "int", -1, false, columnList);
            DB.CheckSqlColumn(table, "YEARS", "int", -1, false, columnList);
            DB.CheckSqlColumn(table, "TREATMENTNAME", "varchar", 50, true, columnList);
            DB.CheckSqlColumn(table, "YEARSAME", "int", -1, true, columnList);
            DB.CheckSqlColumn(table, "YEARANY", "int", -1, true, columnList);
            DB.CheckSqlColumn(table, "BUDGET", "varchar", 512, true, columnList);
            DB.CheckSqlColumn(table, "COST_", "float", -1, true, columnList);
            DB.CheckSqlColumn(table, "OMS_IS_REPEAT", "bit", -1, true, columnList);//This allows deletion of values added by repeat
            DB.CheckSqlColumn(table, "OMS_IS_EXCLUSIVE", "bit", -1, true, columnList);//This determines if a committed project can share with non-committed.
            DB.CheckSqlColumn(table, "OMS_IS_NOT_ALLOWED", "bit", -1, true, columnList);
            SPHelp spHelp = new SPHelp(table);

            if (spHelp.PrimaryKey == null)
            {
                if (!DB.AlterTable(table + " ADD CONSTRAINT [PK_" + DB.TablePrefix + "COMMITTED] PRIMARY KEY (COMMITID)")) return false;
            }

            Constraint constraintSimulation = null;
            if (spHelp.ForeignKeys != null) constraintSimulation = spHelp.ForeignKeys.Find(delegate(Constraint co) { return co.constraint_keys == "SIMULATIONID"; });
            if (constraintSimulation == null)
            {
                if (!DB.AlterTable(table + " WITH CHECK ADD CONSTRAINT [FK_" + DB.TablePrefix + "COMMITTED_" + DB.TablePrefix + "SIMULATIONS] FOREIGN KEY([SIMULATIONID])"
                    + " REFERENCES [dbo].[" + DB.TablePrefix + "SIMULATIONS] ([SIMULATIONID]) ON UPDATE CASCADE ON DELETE CASCADE")) return false;
            }

            return true;
        }


        /// <summary>
        /// Add/update COMMIT_CONSEQUENCES table in Decision Engine table structure.
        /// </summary>
        /// <returns></returns>
        public static bool CheckCommitConsequencesTable()
        {
            List<SQLColumn> columnList = new List<SQLColumn>();
            string table = DB.TablePrefix + "COMMIT_CONSEQUENCES";
            if (DB.CheckIfTableExists(table) == 0)
            {
                columnList.Add(new SQLColumn("ID_", "int", false, true));
                if (!DB.CreateTable(table, columnList)) return false;//If table is not created cannot proceed.
            }
            //Get list of existing columns
            columnList = DB.GetTableColumns(table);
            DB.CheckSqlColumn(table, "COMMITID", "int", -1, false, columnList);
            DB.CheckSqlColumn(table, "ATTRIBUTE_", "varchar", 50, false, columnList);
            DB.CheckSqlColumn(table, "CHANGE_", "varchar", 4000, true, columnList);

            SPHelp spHelp = new SPHelp(table);

            if (spHelp.PrimaryKey == null)
            {
                if (!DB.AlterTable(table + " ADD CONSTRAINT [PK_" + DB.TablePrefix + "COMMIT_CONSEQUENCES] PRIMARY KEY (ID_)")) return false;
            }

            Constraint constraintSimulation = null;
            if (spHelp.ForeignKeys != null) constraintSimulation = spHelp.ForeignKeys.Find(delegate(Constraint co) { return co.constraint_keys == "COMMITID"; });
            if (constraintSimulation == null)
            {
                if (!DB.AlterTable(table + " WITH CHECK ADD CONSTRAINT [FK_" + DB.TablePrefix + "COMMIT_CONSEQUENCES_" + DB.TablePrefix + "COMMITTED] FOREIGN KEY([COMMITID])"
                    + " REFERENCES [dbo].[" + DB.TablePrefix + "COMMITTED_] ([COMMITID]) ON UPDATE CASCADE ON DELETE CASCADE")) return false;
            }

            return true;
        }



        /// <summary>
        /// Add/update DEFICIENT table in Decision Engine table structure.
        /// </summary>
        /// <returns></returns>
        public static bool CheckDeficientTable()
        {
            List<SQLColumn> columnList = new List<SQLColumn>();
            string table = DB.TablePrefix + "DEFICIENTS";
            if (DB.CheckIfTableExists(table) == 0)
            {
                columnList.Add(new SQLColumn("ID_", "int", false, true));
                if (!DB.CreateTable(table, columnList)) return false;//If table is not created cannot proceed.
            }
            //Get list of existing columns
            columnList = DB.GetTableColumns(table);
            DB.CheckSqlColumn(table, "SIMULATIONID", "int", -1, false, columnList);
            DB.CheckSqlColumn(table, "ATTRIBUTE_", "varchar", 50, true, columnList);
            DB.CheckSqlColumn(table, "DEFICIENTNAME", "varchar", 4000, true, columnList);
            DB.CheckSqlColumn(table, "DEFICIENT", "float", -1, true, columnList);
            DB.CheckSqlColumn(table, "PERCENTDEFICIENT", "float", -1, true, columnList);
            DB.CheckSqlColumn(table, "CRITERIA", "varchar", 4000, true, columnList);
            DB.CheckSqlColumn(table, "BINARY_CRITERIA", "varbinary", int.MaxValue, true, columnList);

            SPHelp spHelp = new SPHelp(table);

            if (spHelp.PrimaryKey == null)
            {
                if (!DB.AlterTable(table + " ADD CONSTRAINT [PK_" + DB.TablePrefix + "DEFICIENTS] PRIMARY KEY (ID_)")) return false;
            }

            Constraint constraintSimulation = null;
            if (spHelp.ForeignKeys != null) constraintSimulation = spHelp.ForeignKeys.Find(delegate(Constraint co) { return co.constraint_keys == "SIMULATIONID"; });
            if (constraintSimulation == null)
            {
                if (!DB.AlterTable(table + " WITH CHECK ADD CONSTRAINT [FK_" + DB.TablePrefix + "DEFICIENTS_" + DB.TablePrefix + "SIMULATIONS] FOREIGN KEY([SIMULATIONID])"
                    + " REFERENCES [dbo].[" + DB.TablePrefix + "SIMULATIONS] ([SIMULATIONID]) ON UPDATE CASCADE ON DELETE CASCADE")) return false;
            }

            return true;
        }

        /// <summary>
        /// Add/update Priority table in Decision Engine table structure.
        /// </summary>
        /// <returns></returns>
        public static bool CheckPriorityTable()
        {
            List<SQLColumn> columnList = new List<SQLColumn>();
            string table = DB.TablePrefix + "PRIORITY";
            if (DB.CheckIfTableExists(table) == 0)
            {
                columnList.Add(new SQLColumn("PRIORITYID", "int", false, true));
                if (!DB.CreateTable(table, columnList)) return false;//If table is not created cannot proceed.
            }
            //Get list of existing columns
            columnList = DB.GetTableColumns(table);
            DB.CheckSqlColumn(table, "SIMULATIONID", "int", -1, false, columnList);
            DB.CheckSqlColumn(table, "PRIORITYLEVEL", "int", -1, true, columnList);
            DB.CheckSqlColumn(table, "CRITERIA", "varchar", 4000, true, columnList);
            DB.CheckSqlColumn(table, "BINARY_CRITERIA", "varbinary", int.MaxValue, true, columnList);
            DB.CheckSqlColumn(table, "YEARS", "int", -1, true, columnList);

            SPHelp spHelp = new SPHelp(table);

            if (spHelp.PrimaryKey == null)
            {
                if (!DB.AlterTable(table + " ADD CONSTRAINT [PK_" + DB.TablePrefix + "PRIORITY] PRIMARY KEY (PRIORITYID)")) return false;
            }

            Constraint constraintSimulation = null;
            if (spHelp.ForeignKeys != null) constraintSimulation = spHelp.ForeignKeys.Find(delegate(Constraint co) { return co.constraint_keys == "SIMULATIONID"; });
            if (constraintSimulation == null)
            {
                if (!DB.AlterTable(table + " WITH CHECK ADD CONSTRAINT [FK_" + DB.TablePrefix + "PRIORITY_" + DB.TablePrefix + "SIMULATIONS] FOREIGN KEY([SIMULATIONID])"
                    + " REFERENCES [dbo].[" + DB.TablePrefix + "SIMULATIONS] ([SIMULATIONID]) ON UPDATE CASCADE ON DELETE CASCADE")) return false;
            }
            return true;
        }

        /// <summary>
        /// Add/update TARGETS table in Decision Engine table structure.
        /// </summary>
        /// <returns></returns>
        public static bool CheckTargetsTable()
        {
            List<SQLColumn> columnList = new List<SQLColumn>();
            string table = DB.TablePrefix + "TARGETS";
            if (DB.CheckIfTableExists(table) == 0)
            {
                columnList.Add(new SQLColumn("ID_", "int", false, true));
                if (!DB.CreateTable(table, columnList)) return false;//If table is not created cannot proceed.
            }
            //Get list of existing columns
            columnList = DB.GetTableColumns(table);
            DB.CheckSqlColumn(table, "SIMULATIONID", "int", -1, false, columnList);
            DB.CheckSqlColumn(table, "ATTRIBUTE_", "varchar", 50, true, columnList);
            DB.CheckSqlColumn(table, "YEARS", "int", -1, true, columnList);
            DB.CheckSqlColumn(table, "TARGETMEAN", "float", -1, true, columnList);
            DB.CheckSqlColumn(table, "TARGETNAME", "varchar", 4000, true, columnList);
            DB.CheckSqlColumn(table, "CRITERIA", "varchar", 4000, true, columnList);
            DB.CheckSqlColumn(table, "BINARY_CRITERIA", "varbinary", int.MaxValue, true, columnList);
            DB.CheckSqlColumn(table, "IS_OMS_PRIORITY", "bit", -1, true, columnList);

            SPHelp spHelp = new SPHelp(table);

            if (spHelp.PrimaryKey == null)
            {
                if (!DB.AlterTable(table + " ADD CONSTRAINT [PK_" + DB.TablePrefix + "TARGETS] PRIMARY KEY (ID_)")) return false;
            }

            Constraint constraintSimulation = null;
            if (spHelp.ForeignKeys != null) constraintSimulation = spHelp.ForeignKeys.Find(delegate(Constraint co) { return co.constraint_keys == "SIMULATIONID"; });
            if (constraintSimulation == null)
            {
                if (!DB.AlterTable(table + " WITH CHECK ADD CONSTRAINT [FK_" + DB.TablePrefix + "TARGETS_" + DB.TablePrefix + "SIMULATIONS] FOREIGN KEY([SIMULATIONID])"
                    + " REFERENCES [dbo].[" + DB.TablePrefix + "SIMULATIONS] ([SIMULATIONID]) ON UPDATE CASCADE ON DELETE CASCADE")) return false;
            }

            return true;
        }

        /// <summary>
        /// Add/update PRIORITYFUND table in Decision Engine table structure.
        /// </summary>
        /// <returns></returns>
        public static bool CheckPriorityFundTable()
        {
            List<SQLColumn> columnList = new List<SQLColumn>();
            string table = DB.TablePrefix + "PRIORITYFUND";
            if (DB.CheckIfTableExists(table) == 0)
            {
                columnList.Add(new SQLColumn("PRIORITYFUNDID", "int", false, true));
                if (!DB.CreateTable(table, columnList)) return false;//If table is not created cannot proceed.
            }
            //Get list of existing columns
            columnList = DB.GetTableColumns(table);
            DB.CheckSqlColumn(table, "PRIORITYID", "int", -1, false, columnList);
            DB.CheckSqlColumn(table, "BUDGET", "varchar", 512, true, columnList);
            DB.CheckSqlColumn(table, "FUNDING", "float", -1, true, columnList);

            SPHelp spHelp = new SPHelp(table);

            if (spHelp.PrimaryKey == null)
            {
                if (!DB.AlterTable(table + " ADD CONSTRAINT [PK_" + DB.TablePrefix + "PRIORITYFUND] PRIMARY KEY (PRIORITYFUNDID)")) return false;
            }

            Constraint constraintSimulation = null;
            if (spHelp.ForeignKeys != null) constraintSimulation = spHelp.ForeignKeys.Find(delegate(Constraint co) { return co.constraint_keys == "PRIORITYID"; });
            if (constraintSimulation == null)
            {
                if (!DB.AlterTable(table + " WITH CHECK ADD CONSTRAINT [FK_" + DB.TablePrefix + "PRIORITYFUND_" + DB.TablePrefix + "PRIORITY] FOREIGN KEY([PRIORITYID])"
                    + " REFERENCES [dbo].[" + DB.TablePrefix + "PRIORITY] ([PRIORITYID]) ON UPDATE CASCADE ON DELETE CASCADE")) return false;
            }

            return true;
        }


        /// <summary>
        /// Add/update INVESTMENTS table in Decision Engine table structure.
        /// </summary>
        /// <returns></returns>
        public static bool CheckInvestmentsTable()
        {
            List<SQLColumn> columnList = new List<SQLColumn>();
            string table = DB.TablePrefix + "INVESTMENTS";
            if (DB.CheckIfTableExists(table) == 0)
            {
                columnList.Add(new SQLColumn("SIMULATIONID", "int", false, false));
                if (!DB.CreateTable(table, columnList)) return false;//If table is not created cannot proceed.
            }
            //Get list of existing columns
            columnList = DB.GetTableColumns(table);
            DB.CheckSqlColumn(table, "FIRSTYEAR", "int", -1, true, columnList);
            DB.CheckSqlColumn(table, "NUMBERYEARS", "int", -1, true, columnList);
            DB.CheckSqlColumn(table, "INFLATIONRATE", "float", -1, true, columnList);
            DB.CheckSqlColumn(table, "DISCOUNTRATE", "float", -1, true, columnList);
            DB.CheckSqlColumn(table, "BUDGETORDER", "varchar", 4000, true, columnList);
            DB.CheckSqlColumn(table, "SIMULATION_START_DATE", "DateTime", -1, true, columnList);
            DB.CheckSqlColumn(table, "BUDGET_NAME", "varchar", 512, true, columnList);
            
            SPHelp spHelp = new SPHelp(table);

            if (spHelp.PrimaryKey == null)
            {
                if (!DB.AlterTable(table + " ADD CONSTRAINT [PK_" + DB.TablePrefix + "INVESTMENTS] PRIMARY KEY (SIMULATIONID)")) return false;
            }

            Constraint constraintSimulation = null;
            if (spHelp.ForeignKeys != null) constraintSimulation = spHelp.ForeignKeys.Find(delegate(Constraint co) { return co.constraint_keys == "SIMULATIONID"; });
            if (constraintSimulation == null)
            {
                if (!DB.AlterTable(table + " WITH CHECK ADD CONSTRAINT [FK_" + DB.TablePrefix + "INVESTMENTS_" + DB.TablePrefix + "SIMULATIONS] FOREIGN KEY([SIMULATIONID])"
                    + " REFERENCES [dbo].[" + DB.TablePrefix + "SIMULATIONS] ([SIMULATIONID]) ON UPDATE CASCADE ON DELETE CASCADE")) return false;
            }

            return true;
        }


        /// <summary>
        /// Add/update YEARLYINVESTMENT table in Decision Engine table structure.
        /// </summary>
        /// <returns></returns>
        public static bool CheckYearlyInvestmentTable()
        {
            List<SQLColumn> columnList = new List<SQLColumn>();
            string table = DB.TablePrefix + "YEARLYINVESTMENT";
            if (DB.CheckIfTableExists(table) == 0)
            {
                columnList.Add(new SQLColumn("YEARID", "int", false, true));
                if (!DB.CreateTable(table, columnList)) return false;//If table is not created cannot proceed.
            }
            //Get list of existing columns
            columnList = DB.GetTableColumns(table);
            DB.CheckSqlColumn(table, "SIMULATIONID", "int", -1, false, columnList);
            DB.CheckSqlColumn(table, "YEAR_", "int", -1, false, columnList);
            DB.CheckSqlColumn(table, "BUDGETNAME", "varchar", 50, false, columnList);
            DB.CheckSqlColumn(table, "AMOUNT", "float", -1, true, columnList);

            SPHelp spHelp = new SPHelp(table);

            if (spHelp.PrimaryKey == null)
            {
                if (!DB.AlterTable(table + " ADD CONSTRAINT [PK_" + DB.TablePrefix + "YEARLYINVESTMENT] PRIMARY KEY (YEARID)")) return false;
            }

            Constraint constraintSimulation = null;
            if (spHelp.ForeignKeys != null) constraintSimulation = spHelp.ForeignKeys.Find(delegate(Constraint co) { return co.constraint_keys == "SIMULATIONID"; });
            if (constraintSimulation == null)
            {
                if (!DB.AlterTable(table + " WITH CHECK ADD CONSTRAINT [FK_" + DB.TablePrefix + "YEARLYINVESTMENT_" + DB.TablePrefix + "BUDGET] FOREIGN KEY([SIMULATIONID])"
                    + " REFERENCES [dbo].[" + DB.TablePrefix + "INVESTMENTS] ([SIMULATIONID]) ON UPDATE CASCADE ON DELETE CASCADE")) return false;
            }
            return true;
        }

        /// <summary>
        /// Add/update ATTRIBUTES_CALCULATED table in Decision Engine table structure.
        /// </summary>
        /// <returns>True if successful</returns>
        public static bool CheckAttributesCalculated()
        {
            List<SQLColumn> columnList = new List<SQLColumn>();
            string table = DB.TablePrefix + "ATTRIBUTES_CALCULATED";
            if (DB.CheckIfTableExists(table) == 0)
            {
                columnList.Add(new SQLColumn("ID_", "int", false, true));
                if (!DB.CreateTable(table, columnList)) return false;//If table is not created cannot proceed.
            }
            //Get list of existing columns
            columnList = DB.GetTableColumns(table);
            DB.CheckSqlColumn(table, "ATTIBUTE_", "varchar",50, false, columnList);
            DB.CheckSqlColumn(table, "EQUATION", "varchar", 4000, true, columnList);
            DB.CheckSqlColumn(table, "CRITERIA", "varchar", 4000, true, columnList);
            DB.CheckSqlColumn(table, "BINARY_EQUATION", "varbinary", int.MaxValue, true, columnList);
            DB.CheckSqlColumn(table, "BINARY_CRITERIA", "varbinary", int.MaxValue, true, columnList);

            SPHelp spHelp = new SPHelp(table);

            if (spHelp.PrimaryKey == null)
            {
                if (!DB.AlterTable(table + " ADD CONSTRAINT [PK_" + DB.TablePrefix + "ATTRIBUTES_CALCULATED] PRIMARY KEY (ID_)")) return false;
            }
            return true;
        }

        /// <summary>
        /// Create PCI Distress table
        /// </summary>
        /// <returns>True if successful</returns>
        public static bool CheckPCIDistress()
        {
            List<SQLColumn> columnList = new List<SQLColumn>();
            string table = DB.TablePrefix + "PCI_DISTRESS";
            if (DB.CheckIfTableExists(table) == 0)
            {
                columnList.Add(new SQLColumn("DISTRESSNUMBER", "int", false, false));
                if (!DB.CreateTable(table, columnList)) return false;//If table is not created cannot proceed.
            }
            //Get list of existing columns
            columnList = DB.GetTableColumns(table);
            DB.CheckSqlColumn(table, "DISTRESSNAME", "varchar", 50, false, columnList);
            DB.CheckSqlColumn(table, "METHOD_", "varchar", 50, false, columnList);
            DB.CheckSqlColumn(table, "METRIC_CONVERSION", "float", -1, true, columnList);
            DB.CheckSqlColumn(table, "ATTIBUTE_", "varchar", 50, true, columnList);
            return true;
        }

        /// <summary>
        /// Create Options table
        /// </summary>
        /// <returns>True if successful</returns>
        public static bool CheckOptions()
        {
            List<SQLColumn> columnList = new List<SQLColumn>();
            string table = DB.TablePrefix + "OPTIONS";
            if (DB.CheckIfTableExists(table) == 0)
            {
                columnList.Add(new SQLColumn("OPTION_NAME", "varchar", true, false));
                if (!DB.CreateTable(table, columnList)) return false;//If table is not created cannot proceed.
            }
            //Get list of existing columns
            columnList = DB.GetTableColumns(table);
            DB.CheckSqlColumn(table, "OPTION_NAME", "varchar", 4000, true, columnList);
            DB.CheckSqlColumn(table, "OPTION_VALUE", "varchar", 4000, true, columnList);
            if(InitializeOptions() == 0) return false;
            return true;
        }

        private static int InitializeOptions()
        {
            int rowCount = 0;
            using (SqlConnection connection = new SqlConnection(DB.ConnectionString))
            {
                connection.Open();
                SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM " + DB.TablePrefix + "OPTIONS",connection);
                rowCount = (int)cmd.ExecuteScalar();
                if (rowCount == 0)
                {
                    cmd = new SqlCommand("INSERT INTO " + DB.TablePrefix + "OPTIONS (OPTION_NAME,OPTION_VALUE) VALUES('AREA_CALCULATION','[AREA]')", connection);
                    rowCount += cmd.ExecuteNonQuery();

                    cmd = new SqlCommand("INSERT INTO " + DB.TablePrefix + "OPTIONS (OPTION_NAME,OPTION_VALUE) VALUES('PCI_UNITS','US')", connection);
                    rowCount += cmd.ExecuteNonQuery();
                }
            }
            return rowCount;
        }


        public static bool CheckOCIWeights()
        {
            List<SQLColumn> columnList = new List<SQLColumn>();
            string table = DB.TablePrefix + "OCI_WEIGHTS";
            if (DB.CheckIfTableExists(table) == 0)
            {
                columnList.Add(new SQLColumn("OCIID", "int", false, true));
                if (!DB.CreateTable(table, columnList)) return false;//If table is not created cannot proceed.
            }
            //Get list of existing columns
            columnList = DB.GetTableColumns(table);
            DB.CheckSqlColumn(table, "SIMULATIONID", "int", -1, false, columnList);
            DB.CheckSqlColumn(table, "CONDITION_CATEGORY", "varchar", 255, false, columnList);
            DB.CheckSqlColumn(table, "WEIGHT", "int", -1, false, columnList);
            DB.CheckSqlColumn(table, "CRITERIA", "varchar", 4000, true, columnList);
            SPHelp spHelp = new SPHelp(table);

            if (spHelp.PrimaryKey == null)
            {
                if (!DB.AlterTable(table + " ADD CONSTRAINT [PK_" + table + "] PRIMARY KEY (OCIID)")) return false;
            }

            Constraint constraintSimulation = null;
            if (spHelp.ForeignKeys != null) constraintSimulation = spHelp.ForeignKeys.Find(delegate(Constraint co) { return co.constraint_keys == "SIMULATIONID"; });
            if (constraintSimulation == null)
            {
                if (!DB.AlterTable(table + " WITH CHECK ADD CONSTRAINT [FK_" + table + "_" + DB.TablePrefix + "_SIMULATIONS] FOREIGN KEY([SIMULATIONID])"
                    + " REFERENCES [dbo].[" + DB.TablePrefix + "SIMULATIONS] ([SIMULATIONID]) ON UPDATE CASCADE ON DELETE CASCADE")) return false;
            }
            return true;
        }
        /// <summary>
        /// "SELECT ATTRIBUTE_,TYPE_,DEFAULT_VALUE,MINIMUM_,MAXIMUM,ASCENDING,FORMAT FROM ATTRIBUTES_";
        /// </summary>
        /// <returns></returns>
        public static bool CheckOMSAttributes()
        {
            List<SQLColumn> columnList = new List<SQLColumn>();
            string table = DB.TablePrefix + "OMS_ATTRIBUTES";
            if (DB.CheckIfTableExists(table) == 0)
            {
                columnList.Add(new SQLColumn("ATTRIBUTEID", "int", false, true));
                if (!DB.CreateTable(table, columnList)) return false;//If table is not created cannot proceed.
            }
            //Get list of existing columns
            columnList = DB.GetTableColumns(table);
            DB.CheckSqlColumn(table, "SIMULATIONID", "int", -1, false, columnList);
            DB.CheckSqlColumn(table, "ATTRIBUTE_", "varchar", 255, false, columnList);
            DB.CheckSqlColumn(table, "TYPE_", "varchar", 50, false, columnList);
            DB.CheckSqlColumn(table, "DEFAULT_VALUE", "varchar", 255, true, columnList);
            DB.CheckSqlColumn(table, "MINIMUM_", "float", -1, true, columnList);
            DB.CheckSqlColumn(table, "MAXIMUM", "float", -1, true, columnList);
            DB.CheckSqlColumn(table, "ASCENDING", "bit", -1, true, columnList);
            DB.CheckSqlColumn(table, "FORMAT", "varchar", 255, true, columnList);
            DB.CheckSqlColumn(table, "IS_CONDITION_INDEX", "bit", -1, true, columnList);
            DB.CheckSqlColumn(table, "ATTRIBUTE_OMS", "varchar", 255, true, columnList);

            SPHelp spHelp = new SPHelp(table);

            if (spHelp.PrimaryKey == null)
            {
                if (!DB.AlterTable(table + " ADD CONSTRAINT [PK_" + table + "] PRIMARY KEY (ATTRIBUTEID)")) return false;
            }

            Constraint constraintSimulation = null;
            if (spHelp.ForeignKeys != null) constraintSimulation = spHelp.ForeignKeys.Find(delegate(Constraint co) { return co.constraint_keys == "SIMULATIONID"; });
            if (constraintSimulation == null)
            {
                if (!DB.AlterTable(table + " WITH CHECK ADD CONSTRAINT [FK_" + table + "_" + DB.TablePrefix + "_SIMULATIONS] FOREIGN KEY([SIMULATIONID])"
                    + " REFERENCES [dbo].[" + DB.TablePrefix + "SIMULATIONS] ([SIMULATIONID]) ON UPDATE CASCADE ON DELETE CASCADE")) return false;
            }
            return true;
        }


        /// <summary>
        /// This table stores a snapshot of OMS Asset data 
        /// </summary>
        /// <returns></returns>
        public static bool CheckOMSAssetData()
        {
            List<SQLColumn> columnList = new List<SQLColumn>();
            string table = DB.TablePrefix + "OMS_ASSETS";
            if (DB.CheckIfTableExists(table) == 0)
            {
                columnList.Add(new SQLColumn("OMS_ASSET_ID", "bigint", false, true));
                if (!DB.CreateTable(table, columnList)) return false;//If table is not created cannot proceed.
            }
            //Get list of existing columns
            columnList = DB.GetTableColumns(table);
            DB.CheckSqlColumn(table, "SIMULATIONID", "int", -1, false, columnList);
            DB.CheckSqlColumn(table, "OID", "int", -1, false, columnList);
            DB.CheckSqlColumn(table, "ATTRIBUTE_", "varchar", 255, false, columnList);
            DB.CheckSqlColumn(table, "ASSET_DATE", "datetime", -1, false, columnList);
            DB.CheckSqlColumn(table, "ASSET_VALUE", "varchar", 255, true, columnList);

            SPHelp spHelp = new SPHelp(table);

            if (spHelp.PrimaryKey == null)
            {
                if (!DB.AlterTable(table + " ADD CONSTRAINT [PK_" + table + "] PRIMARY KEY (OMS_ASSET_ID)")) return false;
            }

            Constraint constraintSimulation = null;
            if (spHelp.ForeignKeys != null) constraintSimulation = spHelp.ForeignKeys.Find(delegate(Constraint co) { return co.constraint_keys == "SIMULATIONID"; });
            if (constraintSimulation == null)
            {
                if (!DB.AlterTable(table + " WITH CHECK ADD CONSTRAINT [FK_" + table + "_" + DB.TablePrefix + "_SIMULATIONS] FOREIGN KEY([SIMULATIONID])"
                    + " REFERENCES [dbo].[" + DB.TablePrefix + "SIMULATIONS] ([SIMULATIONID]) ON UPDATE CASCADE ON DELETE CASCADE")) return false;
            }
            return true;
        }



        /// <summary>
        /// Checks Decision Engine database for completeness.
        /// </summary>
        /// <returns></returns>
        public static bool CheckDatabase()
        {
            CheckSimulationTable();
            CheckPerformanceTable();
            CheckTreatmentTable();
            CheckFeasibilityTable();
            CheckCostsTable();
            CheckConsequencesTable();
            CheckCommittedTable();
            CheckCommitConsequencesTable();
            CheckDeficientTable();
            CheckPriorityTable();
            CheckTargetsTable();
            CheckPriorityFundTable();
            CheckInvestmentsTable();
            CheckYearlyInvestmentTable();
            CheckAttributesCalculated();
            CheckPCIDistress();
            CheckOptions();
            CheckOCIWeights();
            CheckOMSAttributes();
            CheckOMSAssetData();
            return true;
        }

        /// <summary>
        /// Deletes all traces of simulation for database
        /// </summary>
        /// <param name="simulationID">SimulationID to remove</param>
        /// <returns>True if successful</returns>
        public static bool RemoveSimulation(string simulationID)
        {
            string networkID = null;
            using (SqlConnection connection = new SqlConnection(DB.ConnectionString))
            {
                try
                {
                    connection.Open();
                    string sql = "SELECT NETWORKID FROM SIMULATIONS WHERE SIMULATIONID=@simulationID";
                    SqlCommand cmd = new SqlCommand(sql, connection);
                    cmd.Parameters.Add(new SqlParameter("simulationID",simulationID));
                    SqlDataReader dr = cmd.ExecuteReader();
                    if(dr.Read())
                    {
                        networkID = dr["NETWORKID"].ToString();
                    }
                }
                catch (Exception e)
                {
                    Utility.ExceptionHandling.DataAccessExceptionHandler.HandleException(e, "Error getting NetworkID", false);
                    return false;
                }
            }
            if (networkID == null) return false;
            
            RemoveSimulationData(simulationID);
            RemoveSimulationTables(simulationID, networkID);

            return true;
        }

        /// <summary>
        /// Removes the data portion from Simulation.  Cascades through all simulation tables. 
        /// </summary>
        /// <param name="simulationID">SimulationID for which to delete data</param>
        /// <returns>Number of simulations removed (1 is expected)</returns>
        public static int RemoveSimulationData(string simulationID)
        {
            int rowsRemoved = 0;
            using (SqlConnection connection = new SqlConnection(DB.ConnectionString))
            {
                try
                {
                    connection.Open();
                    string sql = "DELETE FROM SIMULATIONS WHERE SIMULATIONID=@simulationID";
                    SqlCommand cmd = new SqlCommand(sql, connection);
                    rowsRemoved = cmd.ExecuteNonQuery();
                }
                catch (Exception e)
                {
                    Utility.ExceptionHandling.DataAccessExceptionHandler.HandleException(e, "Error deleting SimulationID=" + simulationID, false);
                    return 0;
                }
            }
            return rowsRemoved;
        }

        /// <summary>
        /// Remove all DecisionEngine generated tables associated with the simulationID
        /// </summary>
        /// <param name="simulationID">SimulationID to delete</param>
        /// <param name="networkID">NetworkID that the simulationID belongs to</param>
        /// <returns>Returns true if successful</returns>
        public static bool RemoveSimulationTables(string simulationID, string networkID)
        {
            string benefitCostTable = DB.TablePrefix + "BENEFIT_COST_" + networkID.ToString() + "_" + simulationID.ToString();
            string simulationTable = DB.TablePrefix + "SIMULATION_" + networkID.ToString() + "_" + simulationID.ToString();
            string targetTable = DB.TablePrefix + "TARGET_" + networkID.ToString() + "_" + simulationID.ToString();
            string reportTable = DB.TablePrefix + "REPORT_" + networkID.ToString() + "_" + simulationID.ToString();
            try
            {
                if (DB.CheckIfTableExists(benefitCostTable) == 1) DB.DropTable(benefitCostTable);
                if (DB.CheckIfTableExists(simulationTable) == 1) DB.DropTable(simulationTable);
                if (DB.CheckIfTableExists(targetTable) == 1) DB.DropTable(targetTable);
                if (DB.CheckIfTableExists(reportTable) == 1) DB.DropTable(reportTable);
            }
            catch (Exception e)
            {
                Utility.ExceptionHandling.DataAccessExceptionHandler.HandleException(e, "Error dropping tables on Simulation delete",false);
                return false;
            }
            return true;
        }
    }
}
