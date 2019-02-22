using BridgeCare.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BridgeCare.Interfaces
{
    public interface IAttributeNames
    {
        IQueryable<AttributeNameModel> GetAttributeNames(BridgeCareContext db);
    }
}
