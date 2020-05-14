using System;

namespace AppliedResearchAssociates.iAM
{
    public abstract class CompilableExpression
    {
        public string Expression
        {
            get => _Expression;
            set
            {
                if (_Expression != value)
                {
                    _Expression = value;
                    _EnsureCompiled = _Compile;
                }
            }
        }

        public void EnsureCompiled() => _EnsureCompiled();

        protected CompilableExpression() => _EnsureCompiled = _Compile;

        protected static Exception ExpressionCouldNotBeCompiled(Exception innerException = null) => new MalformedInputException("Expression could not be compiled.", innerException);

        protected abstract void Compile();

        private Action _EnsureCompiled;

        private string _Expression;

        private void _Compile()
        {
            Compile();
            _EnsureCompiled = Inaction.Delegate;
        }
    }
}
