using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace AppliedResearchAssociates.iAM.Simulation
{
    public class PiecewiseEquation
    {
        public string Expression
        {
            get => _Expression;
            set
            {
                if (_Expression != value)
                {
                    _Expression = value;

                    Data.Clear();
                    var match = Pattern.Match(_Expression);
                    foreach (var group in match.Groups.Cast<Group>().Skip(1))
                    {
                        var datum = group.Value.Split(',');
                        var input = int.Parse(datum[0]);
                        var output = double.Parse(datum[1]);
                        Data.Add(input, output);
                    }
                }
            }
        }

        public double this[int index] => Data[index];

        private static readonly Regex Pattern = new Regex(@"\A(?:\((.*?)\))*\z");

        private readonly Dictionary<int, double> Data = new Dictionary<int, double>();

        private string _Expression;
    }
}
