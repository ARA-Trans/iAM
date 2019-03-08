using BridgeCare.Models;
using System.Collections;
using System.Linq;

namespace BridgeCare.DataAccessLayer
{
    public class TargetsMet
    {
        public Hashtable GetData(IQueryable<DeficientModel> deficientResults)
        {
            var targetAndYear = new Hashtable();
            Hashtable metTarget;
            foreach (var item in deficientResults)
            {
                if (targetAndYear.ContainsKey(item.TargetID))
                {
                    metTarget = (Hashtable)targetAndYear[item.TargetID];
                }
                else
                {
                    metTarget = new Hashtable();
                    targetAndYear.Add(item.TargetID, metTarget);
                }
                metTarget.Add(item.Years, item.TargetMet);
            }
            return targetAndYear;
        }
    }
}