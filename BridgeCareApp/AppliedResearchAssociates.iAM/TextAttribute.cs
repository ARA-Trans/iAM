namespace AppliedResearchAssociates.iAM
{
    public sealed class TextAttribute : Attribute<string>
    {
        internal TextAttribute(Explorer explorer) : base(explorer)
        {
        }
    }
}
