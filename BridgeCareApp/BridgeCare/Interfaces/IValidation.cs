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
        void ValidateEquation(ValidateEquationModel data, BridgeCareContext db);
        string ValidateCriteria(string data, BridgeCareContext db);
    }
}
