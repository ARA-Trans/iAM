using System;
using System.Collections.Generic;
using AppliedResearchAssociates.Validation;

namespace AppliedResearchAssociates.iAM
{
    public abstract class CompilableExpression : IValidator
    {
        public virtual ICollection<ValidationResult> DirectValidationResults
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
                    results.Add(ValidationResult.Create(ValidationStatus.Error, this, e.Message));
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

        public ICollection<IValidator> Subvalidators => new List<IValidator>();

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
