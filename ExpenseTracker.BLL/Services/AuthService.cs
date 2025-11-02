using ExpenseTracker.BLL.Interfaces;
using ExpenseTracker.DAL.Data;
using ExpenseTracker.DAL.Models.AuthModels;
using ExpenseTracker.DAL.Models.AuthModels.DTO_s;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace ExpenseTracker.BLL.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly jwtSettings _jwtSettings;
        private readonly UserDbContext _userContext;

        public AuthService(
            UserManager<AppUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IOptions<jwtSettings> jwtSettings,
            UserDbContext userContext)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _jwtSettings = jwtSettings.Value;
            _userContext = userContext;
        }

        public async Task<AuthResponseDto> RegisterAsync(RegisterDto model)
        {
            if (await _userManager.FindByEmailAsync(model.Email) != null)
                return new AuthResponseDto { Message = "Email is already registered!" };

            if (await _userManager.FindByNameAsync(model.Username) != null)
                return new AuthResponseDto { Message = "Username is already taken!" };

            var user = new AppUser
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Email,
                UserName = model.Username
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                return new AuthResponseDto { Message = errors };
            }

            // Add default role
            await _userManager.AddToRoleAsync(user, "User");

            var jwtSecurityToken = await CreateJwtToken(user);
            var refreshToken = GenerateRefreshToken();

            user.RefreshTokens.Add(refreshToken);
            await _userManager.UpdateAsync(user);

            return new AuthResponseDto
            {
                IsAuthenticated = true,
                Email = user.Email!,
                Username = user.UserName!,
                Roles = new List<string> { "User" },
                AccessToken = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken),
                RefreshToken = refreshToken.Token,
                RefreshTokenExpiration = refreshToken.ExpiresOn
            };
        }

        public async Task<AuthResponseDto> LoginAsync(LoginDto model)
        {
            var authResponse = new AuthResponseDto();

            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user == null || !await _userManager.CheckPasswordAsync(user, model.Password))
            {
                authResponse.Message = "Email or Password is incorrect!";
                return authResponse;
            }

            var jwtSecurityToken = await CreateJwtToken(user);
            var rolesList = await _userManager.GetRolesAsync(user);

            authResponse.IsAuthenticated = true;
            authResponse.AccessToken = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
            authResponse.Email = user.Email!;
            authResponse.Username = user.UserName!;
            authResponse.Roles = rolesList.ToList();

            if (user.RefreshTokens.Any(t => t.IsActive))
            {
                var activeRefreshToken = user.RefreshTokens.FirstOrDefault(t => t.IsActive);
                authResponse.RefreshToken = activeRefreshToken!.Token;
                authResponse.RefreshTokenExpiration = activeRefreshToken.ExpiresOn;
            }
            else
            {
                var refreshToken = GenerateRefreshToken();
                authResponse.RefreshToken = refreshToken.Token;
                authResponse.RefreshTokenExpiration = refreshToken.ExpiresOn;
                user.RefreshTokens.Add(refreshToken);
                await _userManager.UpdateAsync(user);
            }

            return authResponse;
        }

        public async Task<AuthResponseDto> RefreshTokenAsync(string token)
        {
            var authResponse = new AuthResponseDto();

            var user = await _userContext.Users
                .Include(u => u.RefreshTokens)
                .FirstOrDefaultAsync(u => u.RefreshTokens.Any(t => t.Token == token));

            if (user == null)
            {
                authResponse.Message = "Invalid token";
                return authResponse;
            }

            var refreshToken = user.RefreshTokens.Single(t => t.Token == token);

            if (!refreshToken.IsActive)
            {
                authResponse.Message = "Inactive token";
                return authResponse;
            }

            // Revoke current refresh token
            refreshToken.RevokedOn = DateTime.UtcNow;

            // Generate new refresh token
            var newRefreshToken = GenerateRefreshToken();
            user.RefreshTokens.Add(newRefreshToken);
            await _userContext.SaveChangesAsync();

            // Generate new JWT
            var jwtToken = await CreateJwtToken(user);
            var roles = await _userManager.GetRolesAsync(user);

            authResponse.IsAuthenticated = true;
            authResponse.AccessToken = new JwtSecurityTokenHandler().WriteToken(jwtToken);
            authResponse.Email = user.Email!;
            authResponse.Username = user.UserName!;
            authResponse.Roles = roles.ToList();
            authResponse.RefreshToken = newRefreshToken.Token;
            authResponse.RefreshTokenExpiration = newRefreshToken.ExpiresOn;

            return authResponse;
        }

        public async Task<bool> RevokeTokenAsync(string token)
        {
            var user = await _userContext.Users
                .Include(u => u.RefreshTokens)
                .FirstOrDefaultAsync(u => u.RefreshTokens.Any(t => t.Token == token));

            if (user == null)
                return false;

            var refreshToken = user.RefreshTokens.Single(t => t.Token == token);

            if (!refreshToken.IsActive)
                return false;

            refreshToken.RevokedOn = DateTime.UtcNow;
            await _userContext.SaveChangesAsync();

            return true;
        }

        private async Task<JwtSecurityToken> CreateJwtToken(AppUser user)
        {
            var userClaims = await _userManager.GetClaimsAsync(user);
            var roles = await _userManager.GetRolesAsync(user);
            var roleClaims = roles.Select(role => new Claim(ClaimTypes.Role, role)).ToList();

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName!),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email!),
                new Claim("uid", user.Id)
            }
            .Union(userClaims)
            .Union(roleClaims);

            var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Secret));
            var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);

            var jwtSecurityToken = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_jwtSettings.AccessTokenExpirationMinutes),
                signingCredentials: signingCredentials);

            return jwtSecurityToken;
        }

        private RefreshToken GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);

            return new RefreshToken
            {
                Token = Convert.ToBase64String(randomNumber),
                ExpiresOn = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpirationDays),
                CreatedOn = DateTime.UtcNow
            };
        }
    }
}
