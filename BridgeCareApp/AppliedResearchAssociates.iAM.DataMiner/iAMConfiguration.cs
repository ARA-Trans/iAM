using AppliedResearchAssociates.iAM.DataMiner.Attributes;

namespace AppliedResearchAssociates.iAM.DataMiner
{
    public static class iAMConfiguration
    {
        public static TextAttribute B { get; } = new TextAttribute("B", new SqlAttributeConnection("sa", "20Pikachu", "52.177.117.86,1433", "DbBackup") , "DEFAULT");
        public static NumericAttribute C { get; } = new NumericAttribute("C", new SqlAttributeConnection("sa", "20Pikachu", "52.177.117.86,1433", "DbBackup"), 1, 100, 0);
        public static NumericAttribute D { get; } = new NumericAttribute("D", new SqlAttributeConnection("sa", "20Pikachu", "52.177.117.86,1433", "DbBackup"), 1, 100, 0);
    }
}
