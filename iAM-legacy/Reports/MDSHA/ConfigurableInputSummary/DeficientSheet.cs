namespace Reports.MDSHA.ConfigurableInputSummary
{
    using System.Collections.Generic;
    using System.Data;

    public class DeficientSheet : Report.Sheet
    {
        public DeficientSheet()
        {
            this.Caption = "Deficient";
        }

        protected override List<FormattedTable> GetContentItems()
        {
            var content = new List<FormattedTable>();
            content.Add(this.GetMainTable());

            return content;
        }

        private FormattedTable GetMainTable()
        {
            var data = this.GetMainData();

            // Default sorting
            var view = data.DefaultView;
            view.Sort = data.Columns["attribute_"].ColumnName;

            // Create formatted table
            var table = new FormattedTable(view.ToTable());
            table.AddTableFormatting(true);

            // Column renaming
            table.AddColumnRenaming("attribute_", "ATTRIBUTE");
            table.AddColumnRenaming("deficientname", "DEFICIENT NAME");
            table.AddColumnRenaming("deficient", "DEFICIENT LEVEL");
            table.AddColumnRenaming("percentdeficient", "ALLOWED DEFICIENT (%)");

            return table;
        }

        private DataTable GetMainData()
        {
            // Query the database for the content data
            var queryTemplate =
                "SELECT attribute_," +
                "       deficientname," +
                "       deficient," +
                "       percentdeficient," +
                "       criteria" +
                "  FROM deficients" +
                "  WHERE simulationid = {0}";

            return this.GetResultTable(queryTemplate);
        }
    }
}
