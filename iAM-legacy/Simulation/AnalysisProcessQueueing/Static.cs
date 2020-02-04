using System;
using System.Collections.Generic;
using System.Threading;

namespace Simulation.AnalysisProcessQueueing
{
    internal static class Static
    {
        public static T Clip<T>(this T value, T limit1, T limit2, IComparer<T> comparer = null) where T : IComparable<T>
        {
            if (comparer?.Compare(limit1, limit2) > 0)
            {
                Swap(ref limit1, ref limit2);
            }

            return _Clip(value, limit1, limit2, comparer);
        }

        public static void Swap<T>(ref T value1, ref T value2)
        {
            var value = value1;
            value1 = value2;
            value2 = value;
        }

        private static T _Clip<T>(T value, T limitLow, T limitHigh, IComparer<T> comparer = null) where T : IComparable<T>
        {
            if (comparer == null)
            {
                comparer = Comparer<T>.Default;
            }

            if (comparer.Compare(value, limitLow) < 0)
            {
                return limitLow;
            }
            else if (comparer.Compare(value, limitHigh) > 0)
            {
                return limitHigh;
            }
            else
            {
                return value;
            }
        }

        public static void TryLock(object o, Action a)
        {
            var lockTaken = false;
            try
            {
                Monitor.TryEnter(o, ref lockTaken);
                if (lockTaken)
                {
                    a();
                }
            }
            finally
            {
                if (lockTaken)
                {
                    Monitor.Exit(o);
                }
            }
        }
    }
}
