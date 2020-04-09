using System;

namespace AppliedResearchAssociates
{
    public sealed class FinalActor<T>
    {
        internal FinalActor(T value, Action<FinalActor<T>> finalAction)
        {
            Value = value;
            FinalAction = finalAction;
        }

        public T Value { get; }

        private readonly Action<FinalActor<T>> FinalAction;

        ~FinalActor()
        {
            FinalAction(this);
        }
    }
}
