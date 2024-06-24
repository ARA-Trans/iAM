using AppliedResearchAssociates.iAM.DataMiner.Attributes;

namespace AppliedResearchAssociates.iAM.DataMiner
{
    public static class iAMConfiguration
    {
        private static string ConnectionString = "data source=52.177.117.86,56242\\SQL2014;initial catalog=DbBackup;persist security info=True;user id=sa;password=20Pikachu^;MultipleActiveResultSets=True";
        public static TextAttribute B { get; } = new TextAttribute("B", new SqlAttributeConnection(ConnectionString, "SELECT * FROM B") , "DEFAULT");
        public static NumericAttribute C { get; } = new NumericAttribute("C", new SqlAttributeConnection(ConnectionString, "SELECT * FROM C"), 1, 100, 0);
        public static NumericAttribute D { get; } = new NumericAttribute("D", new SqlAttributeConnection(ConnectionString, "SELECT * FROM D"), 1, 100, 0);
    }
}
