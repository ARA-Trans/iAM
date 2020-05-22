namespace AppliedResearchAssociates.iAM
{
    public sealed class TextAttribute : Attribute<string>
    {
        public TextAttribute(Explorer explorer) : base(explorer)
        {
        }
    }
}
