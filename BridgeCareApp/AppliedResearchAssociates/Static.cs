using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace AppliedResearchAssociates
{
    public static class Static
    {
        public static IDisposable AsDisposable(this IEnumerable<IDisposable> source) => new AggregateDisposable(source);

        public static IEnumerable<T> Distinct<T>(params T[] values) => values.Distinct();

        public static T Identity<T>(T value) => value;

        public static IEnumerable<IEnumerable<T>> SequenceCast<T>(this IEnumerable<IEnumerable> sequences) => sequences.Select(sequence => sequence.Cast<T>());

        private sealed class AggregateDisposable : IDisposable
        {
            public AggregateDisposable(IEnumerable<IDisposable> disposables)
            {
                Disposables = disposables.ToList() ?? new List<IDisposable>();
            }

            public void Dispose()
            {
                List<Exception> exceptions = null;

                foreach (var disposable in Disposables)
                {
                    try
                    {
                        disposable?.Dispose();
                    }
                    catch (Exception e)
                    {
                        if (exceptions == null)
                        {
                            exceptions = new List<Exception>();
                        }

                        exceptions.Add(e);
                    }
                }

                if (exceptions != null)
                {
                    throw new AggregateException(exceptions);
                }
            }

            private readonly List<IDisposable> Disposables;
        }
    }
}
