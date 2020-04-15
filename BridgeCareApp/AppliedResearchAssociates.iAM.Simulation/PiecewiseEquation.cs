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
            Data.Clear();
            var match = Pattern.Match(Expression);
            foreach (var group in match.Groups.Cast<Group>().Skip(1))
            {
                var datum = group.Value.Split(',');
                var input = int.Parse(datum[0]);
                var output = double.Parse(datum[1]);
                Data.Add(input, output);
            }
        }

        private static readonly Regex Pattern = new Regex(@"\A(?:\((.*?)\))*\z");

        private readonly Dictionary<int, double> Data = new Dictionary<int, double>();
    }
}
