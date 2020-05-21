using System;

namespace AppliedResearchAssociates
{
    public class Box<T>
    {
        public Box(Action onSet = null) => OnSet = onSet;

        public T Value
        {
            get => _Value;
            set
            {
                _Value = value;
                OnSet?.Invoke();
            }
        }

        public static implicit operator T(Box<T> box) => box.Value;

        private readonly Action OnSet;

        private T _Value;
    }
}
