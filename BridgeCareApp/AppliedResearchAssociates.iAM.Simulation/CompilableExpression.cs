namespace AppliedResearchAssociates.iAM.Simulation
{
    public class CompilableExpression
    {
        public string Expression
        {
            get => _Expression;
            set
            {
                if (_Expression != value)
                {
                    _Expression = value;
                    IsCompiled = false;
                }
            }
        }

        private protected CompilableExpression()
        {
        }

        protected bool IsCompiled { get; set; }

        private string _Expression;
    }
}
