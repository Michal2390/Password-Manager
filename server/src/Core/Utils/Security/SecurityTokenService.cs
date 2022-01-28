using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

#pragma warning disable 8601

namespace Application.Utils.Security
{
    public interface ISecurityTokenService
    {
        AccessToken GenerateAccessTokenForUser(long userId, string username);
    }

    public record AccessToken(string Token, long ExpirationTimestamp);

    public record RefreshToken(string Token, Guid TokenGuid, long ExpirationTimestamp);

    public class SecurityTokenService : ISecurityTokenService
    {
        private readonly ApplicationSettings _applicationSettings;

        public SecurityTokenService(ApplicationSettings applicationSettings)
        {
            _applicationSettings = applicationSettings;
        }

        public AccessToken GenerateAccessTokenForUser(long userId, string username)
        {
            var claims = new ClaimsIdentity(new[]
            {
                new Claim("type", "access"),
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                new Claim(ClaimTypes.Name, username)
            });

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_applicationSettings.AccessTokenSettings.Key));
            var signingCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expiresAt = DateTime.Now.AddMinutes(_applicationSettings.AccessTokenSettings.ExpiryTimeInMinutes);

            var tokenDescriptor = new SecurityTokenDescriptor
                { Subject = claims, Expires = expiresAt, SigningCredentials = signingCredentials };
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return new AccessToken(tokenHandler.WriteToken(token), ((DateTimeOffset)expiresAt).ToUnixTimeSeconds());
        }
    }
}