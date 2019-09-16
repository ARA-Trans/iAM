namespace Reports.MDSHA.ConfigurableInputSummary
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;

    public class PrioritySheet : Report.Sheet
    {
        public PrioritySheet()
        {
            this.Caption = "Priority";
        }

        protected override List<FormattedTable> GetContentItems()
        {
            var content = new List<FormattedTable>();
            content.Add(this.GetHeadingTable());
            content.Add(this.GetMainTable());

            return content;
        }

        private FormattedTable GetHeadingTable()
        {
            var data = GetHeadingData();

            // Create formatted table
            var table = new FormattedTable(data);
            table.AddTableFormatting(false);

            // Column renaming
            table.AddColumnRenaming("analysis", "OPTIMIZATION METHOD");
            table.AddColumnRenaming("budget_constraint", "BUDGET");
            table.AddColumnRenaming("benefit_variable", "BENEFIT");
            table.AddColumnRenaming("benefit_limit", "BENEFIT LIMIT");
            table.AddColumnRenaming("comments", "DESCRIPTION");
            table.AddColumnRenaming("jurisdiction", "JURISDICTION CRITERIA");

            return table;
        }

        private DataTable GetHeadingData()
        {
            var queryTemplate =
                "SELECT analysis," +
                "       budget_constraint," +
                "       weighting," +
                "       benefit_variable," +
                "       benefit_limit," +
                "       comments," +
                "       jurisdiction" +
                "  FROM simulations" +
                "  WHERE simulationid = {0}";

            return this.GetResultTable(queryTemplate);
        }

        private FormattedTable GetMainTable()
        {
            var data = this.GetMainData();

            // Default sorting
            var view = data.DefaultView;
            view.Sort = data.Columns["prioritylevel"].ColumnName;

            // Create formatted table
            var table = new FormattedTable(view.ToTable());
            table.AddTableFormatting(true);

            // Column renaming
            table.AddColumnRenaming("prioritylevel", "PRIORITY LEVEL");

            // Other formatting
            table.AddFormatting(new RangeFormatting(
                Format.Currency,
                0,
                2,
                data.Rows.Count,
                data.Columns.Count - 2));

            return table;
        }

        private DataTable GetMainData()
        {
            // Query the database for the raw content data
            var queryTemplate =
                "SELECT prioritylevel, criteria, budget, funding" +
                "  FROM priority p JOIN priorityfund pf" +
                "       ON p.priorityid = pf.priorityid" +
                "  WHERE simulationid = {0}";

            var rawData = this.GetResultTable(queryTemplate);

            // Group this data by prioritylevel and criteria
            var groupedData =
                from r in rawData.AsEnumerable()
                group Tuple.Create(r["budget"], r["funding"])
                by Tuple.Create(r["prioritylevel"], r["criteria"]) into g
                orderby g.Key
                select g;

            // Get the set of all lists of budgets
            // (should have exactly one distinct list)
            var budgetLists =
                groupedData.Select(g => g.Select(bf => bf.Item1));

            // Construct new columns for the result table
            var copyNames = new string[] { "prioritylevel", "criteria" };
            var columns = copyNames.Select(n =>
            {
                var c = rawData.Columns[n];
                return new DataColumn(c.ColumnName, c.DataType);
            }).ToList();

            var fundingType = rawData.Columns["funding"].DataType;
            var budgets = budgetLists.First().Cast<string>();
            foreach (var b in budgets)
            {
                columns.Add(new DataColumn(b, fundingType));
            }

            // Construct and fill the result table with the grouped data
            var data = new DataTable();
            data.Columns.AddRange(columns.ToArray());
            foreach (var g in groupedData)
            {
                var r = new ArrayList();
                r.Add(g.Key.Item1);
                r.Add(g.Key.Item2);
                r.AddRange(g.Select(bf => bf.Item2).ToArray());
                data.Rows.Add(r.ToArray());
            }

            return data;
        }
    }
}
