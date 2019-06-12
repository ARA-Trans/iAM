using BridgeCare.ApplicationLog;
using BridgeCare.Interfaces;
using BridgeCare.Models;
using System;
using System.Data.SqlClient;
using System.Linq;

namespace BridgeCare.DataAccessLayer
{
    public class Validation : IValidation
    {

        public bool ValidateEquation(ValidateModel data, BridgeCareContext db)
        {
            CalculateEvaluate calcEval;
            calcEval = new CalculateEvaluate();

            try
            {
                if (data.isFunction)
                {
                    calcEval.BuildClass(data.equation, true);
                }
                else
                {
                    calcEval.BuildClass(data.equation, false);
                }
                calcEval.CompileAssembly();
            }
            catch (Exception exc)
            {
                return false;
            }
            return true;
        }
        public bool ValidateCriteria(ValidateModel data, BridgeCareContext db)
        {
            return true;
        }
    }
}