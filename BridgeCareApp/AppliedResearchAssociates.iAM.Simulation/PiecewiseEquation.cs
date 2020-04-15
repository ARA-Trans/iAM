using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace AppliedResearchAssociates.iAM.Simulation
{
    public class PiecewiseEquation : CompilableExpression
    {
        public double this[int index]
        {
            get
            {
                Prepare();
                return Data[index];
            }
        }

        protected override void Compile()
        {
            var rawData = new SortedDictionary<int, double>();
            var match = Pattern.Match(Expression);
            foreach (var group in match.Groups.Cast<Group>().Skip(1))
            {
                var datum = group.Value.Split(',');
                var input = int.Parse(datum[0]);
                var output = double.Parse(datum[1]);
                rawData.Add(input, output);
            }

            if (rawData.Count == 0)
            {
                throw new InvalidOperationException("Expression has no data.");
            }

            var firstIndex = 0;
            var lastIndex = rawData.Count - 1;

            var keys = rawData.Keys.ToArray();
            var values = rawData.Values.ToArray();

            foreach (var key in Static.BoundRange(keys[firstIndex], keys[lastIndex]))
            {
                var index = Array.BinarySearch(keys, key);
                if (index >= 0)
                {
                    Data.Add(key, values[index]);
                }
                else
                {
                    var upperIndex = ~index;
                    var lowerIndex = upperIndex - 1;

                    var upperValue = values[upperIndex];
                    var lowerValue = values[lowerIndex];

                    var rise = upperValue - lowerValue;
                    var run = upperIndex - lowerIndex;

                    var m = rise / run;
                    var x = key - lowerIndex;
                    var b = lowerValue;

                    var y = m * x + b;

                    Data.Add(key, y);
                }
            }
        }

        private static readonly Regex Pattern = new Regex(@"\A(?:\((.*?)\))*\z");

        private readonly Dictionary<int, double> Data = new Dictionary<int, double>();
    }
}
