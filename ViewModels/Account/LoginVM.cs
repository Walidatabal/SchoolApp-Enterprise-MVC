namespace SchoolApp.ViewModels.Account
{
    public class LoginVM
    {
        public string Email { get; set; } = string.Empty;
        // Stores the email entered in the login form

        public string Password { get; set; } = string.Empty;
        // Stores the password entered in the login form

        public bool RememberMe { get; set; }
        // Optional: keeps the user signed in
    }
}