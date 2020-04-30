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

        protected CompilableExpression() => _EnsureCompiled = _Compile;

        protected abstract void Compile();

        protected void EnsureCompiled() => _EnsureCompiled();

        private Action _EnsureCompiled;

        private string _Expression;

        private void _Compile()
        {
            Compile();
            _EnsureCompiled = Static.DoNothing;
        }
    }
}
