using BridgeCare.Models;
using System.Collections;
using System.Linq;

namespace BridgeCare.DataAccessLayer
{
    public class TargetsMetDAL
    {
        /// <summary>
        /// Gets the deficient report results data and transforms it into a hash table
        /// </summary>
        /// <param name="deficientResults">DeficientReportModel IQueryable</param>
        /// <returns>Hashtable</returns>
        public Hashtable GetData(IQueryable<DeficientReportModel> deficientResults)
        {
            var targetAndYear = new Hashtable();
            Hashtable metTarget;

            foreach(var deficientResult in deficientResults)
            {
                if (targetAndYear.ContainsKey(deficientResult.TargetID))
                    metTarget = (Hashtable)targetAndYear[deficientResult.TargetID];
                else
                {
                    metTarget = new Hashtable();
                    targetAndYear.Add(deficientResult.TargetID, new Hashtable());
                }

                metTarget.Add(deficientResult.Years, deficientResult.TargetMet);
            }

            return targetAndYear;
        }
    }
}