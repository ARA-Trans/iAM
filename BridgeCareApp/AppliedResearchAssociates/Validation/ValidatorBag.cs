using System.Collections;
using System.Collections.Generic;

namespace AppliedResearchAssociates.Validation
{
    public sealed class ValidatorBag : IReadOnlyCollection<IValidator>
    {
        public int Count => ((IReadOnlyCollection<IValidator>)Validators).Count;

        public ValidatorBag Add(IValidator validator)
        {
            if (validator != null)
            {
                _ = Validators.Add(validator);
            }

            return this;
        }

        public ValidatorBag Add(IEnumerable<IValidator> validators)
        {
            if (validators != null)
            {
                foreach (var validator in validators)
                {
                    _ = Add(validator);
                }
            }

            return this;
        }

        public IEnumerator<IValidator> GetEnumerator() => ((IEnumerable<IValidator>)Validators).GetEnumerator();

        public ValidatorBag Remove(IValidator validator)
        {
            _ = Validators.Remove(validator);
            return this;
        }

        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)Validators).GetEnumerator();

        private readonly HashSet<IValidator> Validators = new HashSet<IValidator>();
    }
}
