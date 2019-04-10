using System.Collections;

namespace BridgeCare.Services
{
    public class CheckEquations
    {
        private ArrayList _mathMembers = new ArrayList();
        private Hashtable _mathMembersMap = new Hashtable();

        public bool isValidEquation(string equation)
        {
            string originalInput = equation.Replace("[$", "[").Replace("[@", "[");

            return true;
        }
    }
}