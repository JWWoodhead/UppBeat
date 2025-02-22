using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Uppbeat.Api.Data;
using Uppbeat.Api.Models.Auth;

namespace Uppbeat.Api.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<UppbeatUser> _userManager;
        private readonly string? _authSecret;
        private readonly string? _authIssuer;
        private readonly string? _authAudience;

        public UserService(UserManager<UppbeatUser> userManager, IConfiguration configuration)
        {
            _userManager = userManager;

            _authSecret = configuration["Auth:Secret"];
            _authIssuer = configuration["Auth:ValidIssuer"];
            _authAudience = configuration["Auth:ValidAudience"];
        }

        public async Task<LoginUserResponse> LoginUserAsync(UppbeatUser user, CancellationToken cancellationToken)
        {
            var userRoles = await _userManager.GetRolesAsync(user);

            var authClaims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.NameIdentifier, user.Id)
                // Need to add Claim for "ArtistId" but Artist data doesn't exist yet
            };

            foreach (var userRole in userRoles)
            {
                authClaims.Add(new Claim(ClaimTypes.Role, userRole));
            }

            var token = CreateToken(authClaims);

            await _userManager.UpdateAsync(user);

            return new LoginUserResponse
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                Expiration = token.ValidTo,
                Username = user.UserName,
                // Refresh token endpoint still needs to be implemented
                // RefreshToken = refreshToken
            };
        }

        private JwtSecurityToken CreateToken(List<Claim> authClaims)
        {
            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_authSecret));

            var token = new JwtSecurityToken(
                issuer: _authIssuer,
                audience: _authAudience,
                expires: DateTime.Now.AddHours(3),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256));

            return token;
        }
    }
}
