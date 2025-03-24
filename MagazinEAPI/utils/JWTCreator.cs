using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MagazinEAPI.utils
{
    public class JWTCreator
    {
        public JWTCreator() { }
        public string CreateJWTToken(string email, string role)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("sdfshdfjhdsfkjsdhfksdjssdjfhsdkjfhdsfjkhdsfjkhdsfkjsdfhkjsdhsdfjkhfskjfhdsjkh")); // TODO: move to secrets and change to real one
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, email),
            new Claim(ClaimTypes.Email, email),
            new Claim(ClaimTypes.Role, role)
        };

            var token = new JwtSecurityToken(
                issuer: "MagazinEServer",
                audience: "MagazinEClient",
                claims: claims,
                expires: DateTime.UtcNow.AddHours(2),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
