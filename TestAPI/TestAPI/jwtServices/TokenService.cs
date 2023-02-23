using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TestAPI.Models;

namespace TestAPI.jwtServices
{
    public static class TokenService
    {
        public static string CreateToken(UserTable user)
        {
            var key = Encoding.UTF8.GetBytes(Settings.Secret);
            var descriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]     {
                    new Claim(ClaimTypes.Name, user.UserName.ToString()),
                    new Claim(ClaimTypes.Role, user.UserId.ToString()) //群組腳色
                }),
                Expires = DateTime.UtcNow.AddHours(1.0), 
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(descriptor);
            return tokenHandler.WriteToken(token); 
        }
    }
}
