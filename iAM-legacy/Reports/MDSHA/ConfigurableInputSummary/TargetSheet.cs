namespace Reports.MDSHA.ConfigurableInputSummary
{
    using System.Collections.Generic;
    using System.Data;

    public class TargetSheet : Report.Sheet
    {
        public TargetSheet()
        {
            this.Caption = "Target";
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
            table.AddColumnRenaming("targetmean", "TARGET MEAN");
            table.AddColumnRenaming("targetname", "TARGET NAME");

            return table;
        }

        private DataTable GetMainData()
        {
            // Query the database for the content data
            var queryTemplate =
                "SELECT attribute_, targetmean, targetname, criteria" +
                "  FROM targets" +
                "  WHERE simulationid = {0}";

            return this.GetResultTable(queryTemplate);
        }
    }
}
