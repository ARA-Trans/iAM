namespace Reports.MDSHA.ConfigurableInputSummary
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;

    public class InvestmentSheet : Report.Sheet
    {
        private string[] BudgetOrder;

        public InvestmentSheet()
        {
            this.Caption = "Investment";
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
            var data = this.GetHeadingData();

            // Create formatted table w/o autofiltering
            var table = new FormattedTable(data);
            table.AddTableFormatting(false);

            // Column renaming
            table.AddColumnRenaming("firstyear", "START YEAR");
            table.AddColumnRenaming("numberyears", "ANALYSIS PERIOD");
            table.AddColumnRenaming("inflationrate", "INFLATION RATE (%)");
            table.AddColumnRenaming("discountrate", "DISCOUNT RATE (%)");

            return table;
        }

        private DataTable GetHeadingData()
        {
            var queryTemplate =
                "SELECT firstyear," +
                "       numberyears," +
                "       inflationrate," +
                "       discountrate," +
                "       budgetorder" +
                "  FROM investments" +
                "  WHERE simulationid = {0}";

            var data = this.GetResultTable(queryTemplate);

            // Set BudgetOrder instance member
            this.BudgetOrder =
                data.Rows[0]["budgetorder"].ToString().Split(',');
            data.Columns.Remove("budgetorder");

            return data;
        }

        private FormattedTable GetMainTable()
        {
            var data = this.GetMainData();

            // Default sorting
            var view = data.DefaultView;
            view.Sort = data.Columns["year_"].ColumnName;

            // Create formatted table
            var table = new FormattedTable(view.ToTable());
            table.AddTableFormatting(true);

            // Column renaming
            table.AddColumnRenaming("year_", "YEAR");

            // Other formatting
            table.AddFormatting(new RangeFormatting(
                Format.Currency,
                0,
                1,
                data.Rows.Count,
                data.Columns.Count - 1));

            return table;
        }

        private DataTable GetMainData()
        {
            var queryTemplate =
                "SELECT year_," +
                "       budgetname," +
                "       amount" +
                "  FROM yearlyinvestment" +
                "  WHERE simulationid = {0}";

            var rawData = this.GetResultTable(queryTemplate);

            var yearColumn = rawData.Columns["year_"];
            var columns = new List<DataColumn>
            {
                new DataColumn(yearColumn.ColumnName, yearColumn.DataType)
            };

            var amountType = rawData.Columns["amount"].DataType;
            foreach (var b in this.BudgetOrder)
            {
                columns.Add(new DataColumn(b, amountType));
            }

            var data = new DataTable();
            data.Columns.AddRange(columns.ToArray());

            var years =
                rawData.AsEnumerable()
                .Select(r => r["year_"])
                .Distinct()
                .OrderBy(y => y);

            var budgetAmounts = this.BudgetOrder.Select(b =>
                from r in rawData.AsEnumerable()
                where r["budgetname"].ToString() == b
                orderby r["year_"]
                select r["amount"]);

            var rows = budgetAmounts.Aggregate(
                years.Select(y => new ArrayList { y }),
                (accu, curr) =>
                    accu.Zip(
                        curr,
                        (record, field) =>
                        {
                            record.Add(field);
                            return record;
                        })).Select(r => r.ToArray());

            foreach (var r in rows)
            {
                data.Rows.Add(r);
            }

            return data;
        }
    }
}
