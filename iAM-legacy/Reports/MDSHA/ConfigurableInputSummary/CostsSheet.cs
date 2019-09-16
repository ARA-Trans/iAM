namespace Reports.MDSHA.ConfigurableInputSummary
{
    using System.Collections.Generic;
    using System.Data;

    public class CostsSheet : Report.Sheet
    {
        public CostsSheet()
        {
            this.Caption = "Treatment Costs";
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
            view.Sort = data.Columns["treatment"].ColumnName;

            // Create formatted table
            var table = new FormattedTable(view.ToTable());
            table.AddTableFormatting(true);

            // Column renaming
            table.AddColumnRenaming("cost_", "COST");
            table.AddColumnRenaming("beforeany", "YEARS BEFORE ANY TREATMENT");
            table.AddColumnRenaming("beforesame", "YEARS BEFORE SAME TREATMENT");

            table.AddColumnRenaming("treatmentid", "TREATMENT ID");
            table.AddColumnRenaming("simulationid", "SIMULATION ID");

            return table;
        }

        private DataTable GetMainData()
        {
            var queryTemplate =
                "SELECT cost_," +
                "       criteria," +
                "       t.treatmentid," +
                "       simulationid," +
                "       treatment," +
                "       beforeany," +
                "       beforesame," +
                "       budget," +
                "       description" +
                "  FROM treatments t JOIN costs c" +
                "       ON t.treatmentid = c.treatmentid" +
                "  WHERE simulationid = {0}";

            return this.GetResultTable(queryTemplate);
        }
    }
}
