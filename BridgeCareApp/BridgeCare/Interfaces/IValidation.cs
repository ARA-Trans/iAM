using BridgeCare.Models;

namespace BridgeCare.Interfaces
{
    public interface IValidation
    {
        void ValidateEquation(ValidateEquationModel data, BridgeCareContext db);

        CriteriaValidationResult ValidateCriteria(string data, BridgeCareContext db);
    }
}
