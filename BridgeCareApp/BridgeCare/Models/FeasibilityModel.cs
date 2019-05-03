using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BridgeCare.Models
{
    public class FeasibilityModel
    {
        public FeasibilityModel()
        {
            FeasibilityId = -1;
            Criteria = "";
        }
        public int FeasibilityId { get; set; }
        public string Criteria { get; set; }

        public void Agregate(FeasibilityModel model)
        {
            if (FeasibilityId < 0) 
                FeasibilityId= model.FeasibilityId;

            if (Criteria.Length <= 0)
            {
                Criteria = model.Criteria;
            }
            else
            {
                Criteria += " AND " + model.Criteria;
            }
        }
    }
}