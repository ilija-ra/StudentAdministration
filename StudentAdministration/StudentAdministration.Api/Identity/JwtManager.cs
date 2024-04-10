using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using StudentAdministration.Communication.Accounts.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace StudentAdministration.Api.Identity
{
    public class JwtManager
    {
        private readonly JWTSettings _jwtSettings;

        public JwtManager(IOptions<JWTSettings> jwtSettings)
        {
            _jwtSettings = jwtSettings.Value;
        }

        public async Task<string> GenerateToken(AccountLoginResponseModel user)
        {
            return new JwtSecurityTokenHandler().WriteToken(await GenerateJWToken(user));
        }

        private async Task<JwtSecurityToken> GenerateJWToken(AccountLoginResponseModel user)
        {
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, "User"),
                new Claim(JwtRegisteredClaimNames.Jti, user.Id!),
                new Claim(CustomClaimTypes.UserId, user.Id!),
                new Claim(CustomClaimTypes.FirstName, user.FirstName!),
                new Claim(CustomClaimTypes.LastName, user.LastName!),
                new Claim(CustomClaimTypes.Index, user.Index!),
                new Claim(CustomClaimTypes.EmailAddress, user.EmailAddress!),
                new Claim(CustomClaimTypes.Role, user.Role!),
                new Claim(CustomClaimTypes.PartitionKey, user.PartitionKey!)
            };

            if (_jwtSettings.Key == null)
            {
                return new JwtSecurityToken();
            }

            var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));
            var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);

            var jwtSecurityToken = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_jwtSettings.DurationInMinutes.HasValue ? _jwtSettings.DurationInMinutes.Value : 0),
                signingCredentials: signingCredentials);

            return jwtSecurityToken;
        }
    }
}
