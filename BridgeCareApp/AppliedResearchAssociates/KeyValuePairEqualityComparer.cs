using System.Collections.Generic;

namespace AppliedResearchAssociates
{
    public sealed class KeyValuePairEqualityComparer<TKey, TValue> : EqualityComparer<KeyValuePair<TKey, TValue>>
    {
        public KeyValuePairEqualityComparer(IEqualityComparer<TKey> keyEqualityComparer = null, IEqualityComparer<TValue> valueEqualityComparer = null)
        {
            KeyEqualityComparer = keyEqualityComparer ?? EqualityComparer<TKey>.Default;
            ValueEqualityComparer = valueEqualityComparer ?? EqualityComparer<TValue>.Default;
        }

        public override bool Equals(KeyValuePair<TKey, TValue> x, KeyValuePair<TKey, TValue> y) => KeyEqualityComparer.Equals(x.Key, y.Key) && ValueEqualityComparer.Equals(x.Value, y.Value);

        public override int GetHashCode(KeyValuePair<TKey, TValue> obj)
        {
            var result = new HashCode();

            result.Add(obj.Key, KeyEqualityComparer);
            result.Add(obj.Value, ValueEqualityComparer);

            return result.ToHashCode();
        }

        private readonly IEqualityComparer<TKey> KeyEqualityComparer;

        private readonly IEqualityComparer<TValue> ValueEqualityComparer;
    }
}
