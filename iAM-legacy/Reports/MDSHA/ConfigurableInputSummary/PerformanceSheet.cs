namespace Reports.MDSHA.ConfigurableInputSummary
{
    using System.Collections.Generic;
    using System.Data;

    public class PerformanceSheet : Report.Sheet
    {
        public PerformanceSheet()
        {
            this.Caption = "Performance";
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
            table.AddColumnRenaming("equationname", "EQUATION NAME");

            return table;
        }

        private DataTable GetMainData()
        {
            var queryTemplate =
                "SELECT attribute_, equationname, equation, criteria" +
                "  FROM performance" +
                "  WHERE simulationid = {0}";

            return this.GetResultTable(queryTemplate);
        }
    }
}
