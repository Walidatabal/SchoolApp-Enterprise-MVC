using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using SchoolApp.Services.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SchoolApp.Services.Implementations
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _config;

        public TokenService(IConfiguration config)
        {
            _config = config;
        }

        public string CreateToken(ApplicationUser user, IList<string> roles)
        {
            var claims = new List<Claim>  // ✅ FIXED: was List<Claim> claims = new() — not supported in .NET 10 clame is a class, not a record, so target-typed new() is not allowed we use claims variable name instead of var to avoid confusion with the claims variable in the loop like in the original code
            {
                new Claim(ClaimTypes.Email, user.Email ?? string.Empty),  // different claim are used in the original code, but email is more common and useful for authentication/authorization purposes what are claim types and how they are used in JWT tokens? claim types are predefined constants that represent common types of claims, such as name, email, role, etc. they are used in JWT tokens to identify the type of information being stored in the claim and to allow applications to easily access and use that information for authentication and authorization purposes. for example, the ClaimTypes.Email claim can be used to store the user's email address in the token, which can then be accessed by the application to identify the user and determine their permissions based on their email domain or other criteria.
                new Claim(ClaimTypes.NameIdentifier, user.Id)
            };

            foreach (var role in roles)
                claims.Add(new Claim(ClaimTypes.Role, role));

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_config["Jwt:Key"]!)
            );

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                // ✅ FIXED: was DateTime.Now (local time) — JWT spec requires UTC
                expires: DateTime.UtcNow.AddMinutes(
                    Convert.ToDouble(_config["Jwt:DurationInMinutes"])
                ),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
