using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BridgeCare.Models;

namespace BridgeCare.Interfaces
{
    public interface IValidation
    {
        bool ValidateEquation(ValidateModel data, BridgeCareContext db);
        bool ValidateCriteria(ValidateModel data, BridgeCareContext db);
    }
}
