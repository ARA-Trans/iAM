namespace BridgeCare.Security
{
    public static class Role
    {
        public const string ADMINISTRATOR = "PD-BAMS-Administrator";
        public const string DISTRICT_ENGINEER = "PD-BAMS-DBEngineer";
        public const string GENERAL_USERS = "PD-BAMSUSERS";
        public const string CWOPA = "PD-BAMS-CWOPA";
        public static string[] AllValidRoles = { ADMINISTRATOR, DISTRICT_ENGINEER, GENERAL_USERS, CWOPA };
    }
}
