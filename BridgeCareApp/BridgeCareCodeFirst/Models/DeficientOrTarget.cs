using BridgeCare.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BridgeCareCodeFirst.Models
{
    public class DeficientOrTarget
    {
        public Hashtable DeficientTargetList(IQueryable<DeficientResult> deficientOrTargetList)
        {
            var targetAndYear = new Hashtable();
            Hashtable MetTarget;
            foreach (var item in deficientOrTargetList)
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