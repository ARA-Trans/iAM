namespace Reports.MDSHA.ConfigurableInputSummary
{
    using System.Collections.Generic;
    using System.Data;

    public class TreatmentsSheet : Report.Sheet
    {
        public TreatmentsSheet()
        {
            this.Caption = "Treatments";
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
            table.AddColumnRenaming("beforeany", "YEARS BEFORE ANY TREATMENT");
            table.AddColumnRenaming("beforesame", "YEARS BEFORE SAME TREATMENT");

            return table;
        }

        private DataTable GetMainData()
        {
            var queryTemplate =
                "SELECT treatment," +
                "       beforeany," +
                "       beforesame," +
                "       budget," +
                "       description" +
                "  FROM treatments" +
                "  WHERE simulationid = {0}";

            return this.GetResultTable(queryTemplate);
        }
    }
}
