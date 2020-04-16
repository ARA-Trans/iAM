using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using MathNet.Numerics;
using MathNet.Numerics.Interpolation;

namespace AppliedResearchAssociates.iAM.Simulation
{
    public class PiecewiseEquation : CompilableExpression
    {
        public double this[double t]
        {
            get
            {
                EnsureCompiled();
                return Spline.Interpolate(t);
            }
        }

        protected override void Compile()
        {
            var xs = new List<double>();
            var ys = new List<double>();
            var match = Pattern.Match(Expression);

            foreach (var group in match.Groups.Cast<Group>().Skip(1))
            {
                var datum = group.Value.Split(',');
                var x = double.Parse(datum[0]);
                var y = double.Parse(datum[1]);
                xs.Add(x);
                ys.Add(y);
            }

            Spline = Interpolate.Linear(xs, ys);
        }

        private static readonly Regex Pattern = new Regex(@"\A(?:\((.*?)\))*\z");

        private IInterpolation Spline;
    }
}
