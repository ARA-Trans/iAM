using System;

namespace AppliedResearchAssociates.PciDistress
{
    [Obsolete("Use System.Math instead.")]
    internal static class Math
    {
        public static Func<double, double> floor => System.Math.Floor;

        public static Func<double, double, double> pow => System.Math.Pow;

        public static Func<double, double, double> min => System.Math.Min;
    }
}
