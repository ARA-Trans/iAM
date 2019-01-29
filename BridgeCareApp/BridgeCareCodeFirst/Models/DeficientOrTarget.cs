using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BridgeCare.Models
{
    public class DeficientOrTarget
    {
        public Hashtable DeficientTargetList(IQueryable<DeficientResult> deficientOrTarget)
        {
            var targetAndYear = new Hashtable();
            Hashtable MetTarget;
            foreach (var item in deficientOrTarget)
            {
                if (targetAndYear.ContainsKey(item.TargetID))
                {
                    MetTarget = (Hashtable)targetAndYear[item.TargetID];
                }
                else
                {
                    MetTarget = new Hashtable();
                    targetAndYear.Add(item.TargetID, MetTarget);
                }
                MetTarget.Add(item.Years, item.TargetMet);
            }
            return targetAndYear;
        }
    }
}