using System;
using System.Collections.Generic;
using AppliedResearchAssociates.Validation;

namespace AppliedResearchAssociates.iAM
{
    public abstract class CompilableExpression : IValidator
    {
        public virtual string Expression
        {
            get => _Expression;
            set
            {
                if (Expression != value)
                {
                    _Expression = value;
                    _EnsureCompiled = _Compile;
                }
            }
        }

        public bool ExpressionIsBlank => string.IsNullOrWhiteSpace(Expression);

        public virtual ICollection<ValidationResult> ValidationResults
        {
            get
            {
                var results = new List<ValidationResult>();

                try
                {
                    EnsureCompiled();
                }
                catch (MalformedInputException e)
                {
                    results.Add(ValidationStatus.Error.Describe(e.Message));
                }

                return results;
            }
        }

        protected CompilableExpression() => _EnsureCompiled = _Compile;

        protected static Exception ExpressionCouldNotBeCompiled(Exception innerException = null) => new MalformedInputException("Expression could not be compiled.", innerException);

        protected abstract void Compile();

        protected void EnsureCompiled() => _EnsureCompiled?.Invoke();

        private Action _EnsureCompiled;

        private string _Expression;

        private void _Compile()
        {
            Compile();
            _EnsureCompiled = null;
        }
    }
}
