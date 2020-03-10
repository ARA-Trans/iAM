namespace BridgeCare.Security
{
    public static class Role
    {
        public const string ADMINISTRATOR = "PD-BAMS-Administrator";
        public const string DISTRICT_ENGINEER = "PD-BAMS-DBEngineer";
        public const string PLANNING_PARTNER = "PD-BAMS-PlanningPartner";
        public const string CWOPA = "PD-BAMS-CWOPA";
        public static string[] AllValidRoles = { ADMINISTRATOR, DISTRICT_ENGINEER, PLANNING_PARTNER, CWOPA };
    }
}
