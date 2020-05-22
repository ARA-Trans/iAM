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
            set => _ = UpdateExpression(value);
        }

        public bool ExpressionIsBlank => string.IsNullOrWhiteSpace(Expression);

        public virtual ValidatorBag Subvalidators => new ValidatorBag();

        protected CompilableExpression() => _EnsureCompiled = _Compile;

        protected static Exception ExpressionCouldNotBeCompiled(Exception innerException = null) => new MalformedInputException("Expression could not be compiled.", innerException);

        protected abstract void Compile();

        protected void EnsureCompiled() => _EnsureCompiled?.Invoke();

        protected bool UpdateExpression(string value)
        {
            if (Expression == value)
            {
                return false;
            }

            _Expression = value;
            _EnsureCompiled = _Compile;
            return true;
        }

        private Action _EnsureCompiled;

        private string _Expression;

        private void _Compile()
        {
            Compile();
            _EnsureCompiled = null;
        }
    }
}
