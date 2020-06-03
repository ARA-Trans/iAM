using System;
using AppliedResearchAssociates.Validation;

namespace AppliedResearchAssociates.iAM
{
    public abstract class CompilableExpression : IValidator
    {
        public virtual ValidationResultBag DirectValidationResults
        {
            get
            {
                var results = new ValidationResultBag();

                try
                {
                    EnsureCompiled();
                }
                catch (MalformedInputException e)
                {
                    results.Add(ValidationStatus.Error, e.Message, this, nameof(Expression));
                }

                return results;
            }
        }

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

        public virtual ValidatorBag Subvalidators => new ValidatorBag();

        protected CompilableExpression() => _EnsureCompiled = _Compile;

        protected static MalformedInputException ExpressionCouldNotBeCompiled(Exception innerException = null) => new MalformedInputException("Expression could not be compiled.", innerException);

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
