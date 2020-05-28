using AppliedResearchAssociates.iAM.DataMiner.Attributes;

namespace AppliedResearchAssociates.iAM.DataMiner
{
    public static class iAMConfiguration
    {
        public static TextAttribute B { get; } = new TextAttribute("B", new SqlConnection("sa", "20Pikachu", "40.121.5.125,1433", "DbBackup"), "DEFAULT");

        public static NumericAttribute C { get; } = new NumericAttribute("C", new SqlConnection("sa", "20Pikachu", "40.121.5.125,1433", "DbBackup"), 1, 100, 0);

        public static NumericAttribute D { get; } = new NumericAttribute("D", new SqlConnection("sa", "20Pikachu", "40.121.5.125,1433", "DbBackup"), 1, 100, 0);
    }
}
