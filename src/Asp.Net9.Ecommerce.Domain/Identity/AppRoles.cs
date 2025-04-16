namespace Asp.Net9.Ecommerce.Domain.Identity
{
    public static class AppRoles
    {
        public const string Admin = "Admin";
        public const string Customer = "Customer";

        // Define constant GUIDs and creation date for roles to ensure consistency
        private static readonly Guid AdminRoleId = new("308660dc-ae51-480f-824d-7dca6714c3e2");
        private static readonly Guid CustomerRoleId = new("d7be43da-622c-4cfe-98a9-5a5161120d24");
        private static readonly DateTime RolesCreationDate = new(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public static readonly List<AppRole> DefaultRoles = new()
        {
            AppRole.Create(Admin, "Full access to all features", AdminRoleId, RolesCreationDate),
            AppRole.Create(Customer, "Standard customer access", CustomerRoleId, RolesCreationDate)
        };

        public static bool IsValidRole(string roleName)
        {
            return roleName == Admin || roleName == Customer;
        }
    }
} 