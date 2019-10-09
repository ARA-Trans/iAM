using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BridgeCare.EntityClasses;

namespace BridgeCare.Models
{
    public class RemainingLifeLimitModel : CrudModel
    {
        public string Id { get; set; }
        public string Attribute { get; set; }
        public double Limit { get; set; }
        public string Criteria { get; set; }

        public RemainingLifeLimitModel() { }

        public RemainingLifeLimitModel(RemainingLifeLimitsEntity entity)
        {
            Id = entity.REMAINING_LIFE_ID.ToString();
            Attribute = entity.ATTRIBUTE_;
            Limit = entity.REMAINING_LIFE_LIMIT;
            Criteria = entity.CRITERIA;
        }

        public void Update(RemainingLifeLimitsEntity entity)
        {
            entity.ATTRIBUTE_ = Attribute;
            entity.REMAINING_LIFE_LIMIT = Limit;
            entity.CRITERIA = Criteria;
        }
    }
}