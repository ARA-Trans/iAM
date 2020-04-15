using System;

namespace AppliedResearchAssociates.iAM.Simulation
{
    public abstract class CompilableExpression
    {
        protected CompilableExpression() => _Prepare = _Compile;

        public string Expression
        {
            get => _Expression;
            set
            {
                if (_Expression != value)
                {
                    _Expression = value;
                    _Prepare = _Compile;
                }
            }
        }

        protected abstract void Compile();

        protected void Prepare() => _Prepare();

        private string _Expression;

        private Action _Prepare;

        private void _Compile()
        {
            Compile();
            _Prepare = Static.DoNothing;
        }
    }
}
