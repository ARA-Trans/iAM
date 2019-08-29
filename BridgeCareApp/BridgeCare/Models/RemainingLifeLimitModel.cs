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

        public RemainingLifeLimitModel(RemainingLifeLimitsEntity remainingLifeLimitsEntity)
        {
            Id = remainingLifeLimitsEntity.REMAINING_LIFE_ID.ToString();
            Attribute = remainingLifeLimitsEntity.ATTRIBUTE_;
            Limit = remainingLifeLimitsEntity.REMAINING_LIFE_LIMIT;
            Criteria = remainingLifeLimitsEntity.CRITERIA;
        }

        public void Update(RemainingLifeLimitsEntity remainingLifeLimitsEntity)
        {
            remainingLifeLimitsEntity.ATTRIBUTE_ = Attribute;
            remainingLifeLimitsEntity.REMAINING_LIFE_LIMIT = Limit;
            remainingLifeLimitsEntity.CRITERIA = Criteria;
        }
    }
}