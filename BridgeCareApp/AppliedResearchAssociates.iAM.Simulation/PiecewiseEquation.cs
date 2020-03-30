using System.Collections.Generic;
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
                    Data.Clear();

                    foreach (Match match in CoordinatePattern.Matches(_Expression = value))
                    {
                        var coordinate = match.Groups[1].Value.Split(',');
                        var input = int.Parse(coordinate[0]);
                        var output = double.Parse(coordinate[1]);
                        Data.Add(input, output);
                    }
                }
            }
        }

        public double this[int index] => Data[index];

        private static readonly Regex CoordinatePattern = new Regex(@"\((.*?)\)");

        private readonly Dictionary<int, double> Data = new Dictionary<int, double>();

        private string _Expression;
    }
}
