using Simulation.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simulation
{
    public class SplitTreatmentLimit : ISplitTreatmentLimit
    {
        public int Rank { get; }

        public float Amount { get; }

        public List<float> Percentages { get; }


        public SplitTreatmentLimit(string rank, string amount, string percentages)
        {
            Rank = Convert.ToInt32(rank);
            if(string.IsNullOrWhiteSpace(amount))
            {
                Amount = float.MaxValue;
            }
            else
            {
                Amount = Convert.ToSingle(amount);
            }
            
            Percentages = new List<float>();

            var values = percentages.Split('/');
            if (Rank != values.Length)
            {
                float value = 100 / (float)Rank;
                for (int i = 0; i < Rank; i++)
                {
                    Percentages.Add(value);
                }
            }
            else
            {
                try
                {
                    for (int i = 0; i < Rank; i++)
                    {
                        float value = Convert.ToSingle(values[i]);
                        Percentages.Add(value);
                    }
                }
                catch
                {
                    float value = 100 / (float) Rank;
                    for (int i = 0; i < Rank; i++)
                    {
                        Percentages.Add(value);
                    }
                }
            }
        }
    }
}
