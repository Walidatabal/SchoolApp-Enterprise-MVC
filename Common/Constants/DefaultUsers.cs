namespace SchoolApp.Common.Constants
{
    // Default system users — used only during database seeding.
    // Credentials must be supplied via environment variables or user-secrets.
    // They are never stored in source code.
    public static class DefaultUsers
    {
        public const string AdminEmail = "admin@school.com";  // constant means this value is fixed and cannot be changed at runtime. It's used as the default email for the admin user during database seeding.

        // Reads from:
        //   Development  → dotnet user-secrets set "Seed:AdminPassword" "YourDevPass123!"
        //   Production   → environment variable  Seed__AdminPassword
        public static string GetAdminPassword(IConfiguration config)
        {
            var password = config["Seed:AdminPassword"];

            if (string.IsNullOrWhiteSpace(password))  // If the password is not set or is empty, throw an exception to alert the developer or administrator that the admin password must be configured.
                throw new InvalidOperationException(
                    "Seed:AdminPassword is not configured. " +
                    "Set it via user-secrets (dev) or the Seed__AdminPassword " +
                    "environment variable (production).");

            return password;
        }
    }
}