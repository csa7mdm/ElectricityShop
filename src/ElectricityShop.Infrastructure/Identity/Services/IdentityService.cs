using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using ElectricityShop.Application.Common.Interfaces;
using ElectricityShop.Application.Common.Models;
using ElectricityShop.Domain.Interfaces;
using ElectricityShop.Infrastructure.Identity.Context;
using ElectricityShop.Infrastructure.Identity.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace ElectricityShop.Infrastructure.Identity.Services
{
    public class IdentityService : IIdentityService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;
        private readonly AppIdentityDbContext _identityContext;
        private readonly IUnitOfWork _unitOfWork;

        public IdentityService(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IConfiguration configuration,
            AppIdentityDbContext identityContext,
            IUnitOfWork unitOfWork)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
            _identityContext = identityContext;
            _unitOfWork = unitOfWork;
        }

        public async Task<AuthenticationResult> RegisterAsync(string email, string password, string firstName, string lastName)
        {
            var existingUser = await _userManager.FindByEmailAsync(email);

            if (existingUser != null)
            {
                return new AuthenticationResult
                {
                    Success = false,
                    Errors = new[] { "User with this email already exists" }
                };
            }

            var newUser = new ApplicationUser
            {
                Email = email,
                UserName = email,
                FirstName = firstName,
                LastName = lastName,
                CreatedAt = DateTime.UtcNow
            };

            var createdUser = await _userManager.CreateAsync(newUser, password);

            if (!createdUser.Succeeded)
            {
                return new AuthenticationResult
                {
                    Success = false,
                    Errors = createdUser.Errors.Select(e => e.Description)
                };
            }

            await _userManager.AddToRoleAsync(newUser, "Customer");

            // Create corresponding domain User entity
            var domainUser = new Domain.Entities.User
            {
                Id = Guid.NewGuid(),
                Email = email,
                FirstName = firstName,
                LastName = lastName,
                PhoneNumber = newUser.PhoneNumber,
                Role = Domain.Enums.UserRole.Customer,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Users.AddAsync(domainUser);
            await _unitOfWork.CompleteAsync();

            return await GenerateAuthenticationResultForUserAsync(newUser);
        }

        public async Task<AuthenticationResult> LoginAsync(string email, string password)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user == null)
            {
                return new AuthenticationResult
                {
                    Success = false,
                    Errors = new[] { "User does not exist" }
                };
            }

            var userHasValidPassword = await _userManager.CheckPasswordAsync(user, password);

            if (!userHasValidPassword)
            {
                return new AuthenticationResult
                {
                    Success = false,
                    Errors = new[] { "Invalid credentials" }
                };
            }

            return await GenerateAuthenticationResultForUserAsync(user);
        }

        public async Task<AuthenticationResult> RefreshTokenAsync(string token, string refreshToken)
        {
            var validatedToken = GetPrincipalFromToken(token);

            if (validatedToken == null)
            {
                return new AuthenticationResult
                {
                    Success = false,
                    Errors = new[] { "Invalid token" }
                };
            }

            var expiryDateUnix = long.Parse(validatedToken.Claims.Single(x => x.Type == JwtRegisteredClaimNames.Exp).Value);
            var expiryDateTimeUtc = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                .AddSeconds(expiryDateUnix);

            if (expiryDateTimeUtc > DateTime.UtcNow)
            {
                return new AuthenticationResult
                {
                    Success = false,
                    Errors = new[] { "This token hasn't expired yet" }
                };
            }

            var jti = validatedToken.Claims.Single(x => x.Type == JwtRegisteredClaimNames.Jti).Value;
            var storedRefreshToken = await _identityContext.RefreshTokens.SingleOrDefaultAsync(x => x.Token == refreshToken);

            if (storedRefreshToken == null)
            {
                return new AuthenticationResult
                {
                    Success = false,
                    Errors = new[] { "This refresh token does not exist" }
                };
            }

            if (DateTime.UtcNow > storedRefreshToken.ExpiryDate)
            {
                return new AuthenticationResult
                {
                    Success = false,
                    Errors = new[] { "This refresh token has expired" }
                };
            }

            if (storedRefreshToken.IsUsed)
            {
                return new AuthenticationResult
                {
                    Success = false,
                    Errors = new[] { "This refresh token has been used" }
                };
            }

            if (storedRefreshToken.IsRevoked)
            {
                return new AuthenticationResult
                {
                    Success = false,
                    Errors = new[] { "This refresh token has been revoked" }
                };
            }

            if (storedRefreshToken.JwtId != jti)
            {
                return new AuthenticationResult
                {
                    Success = false,
                    Errors = new[] { "This refresh token does not match this JWT" }
                };
            }

            storedRefreshToken.IsUsed = true;
            _identityContext.RefreshTokens.Update(storedRefreshToken);
            await _identityContext.SaveChangesAsync();

            var user = await _userManager.FindByIdAsync(validatedToken.Claims.Single(x => x.Type == "id").Value);
            return await GenerateAuthenticationResultForUserAsync(user);
        }

        public async Task<bool> IsInRoleAsync(string userId, string role)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null) return false;

            return await _userManager.IsInRoleAsync(user, role);
        }

        public async Task<bool> AuthorizeAsync(string userId, string policyName)
        {
            // This is a simplified implementation
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null) return false;

            // For demo purposes we'll just check if the user is an admin
            return await _userManager.IsInRoleAsync(user, "Admin");
        }

        private async Task<AuthenticationResult> GenerateAuthenticationResultForUserAsync(ApplicationUser user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["JwtSettings:Secret"]);

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim("id", user.Id)
            };

            var userRoles = await _userManager.GetRolesAsync(user);
            foreach (var userRole in userRoles)
            {
                claims.Add(new Claim(ClaimTypes.Role, userRole));
            }

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.Add(TimeSpan.Parse(_configuration["JwtSettings:TokenLifetime"])),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);

            var refreshToken = new RefreshToken
            {
                Id = Guid.NewGuid(),
                JwtId = token.Id,
                UserId = user.Id,
                CreatedAt = DateTime.UtcNow,
                ExpiryDate = DateTime.UtcNow.AddMonths(6),
                Token = Guid.NewGuid().ToString()
            };

            await _identityContext.RefreshTokens.AddAsync(refreshToken);
            await _identityContext.SaveChangesAsync();

            var domainUser = (await _unitOfWork.Users.ListAsync(u => u.Email == user.Email)).FirstOrDefault();

            return new AuthenticationResult
            {
                Success = true,
                Token = tokenHandler.WriteToken(token),
                RefreshToken = refreshToken.Token,
                User = domainUser != null ? new UserDto
                {
                    Id = domainUser.Id,
                    Email = domainUser.Email,
                    FirstName = domainUser.FirstName,
                    LastName = domainUser.LastName,
                    PhoneNumber = domainUser.PhoneNumber,
                    Role = domainUser.Role.ToString()
                } : null
            };
        }

        private ClaimsPrincipal GetPrincipalFromToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["JwtSettings:Secret"]);

            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false,
                RequireExpirationTime = false,
                ValidateLifetime = false
            };

            try
            {
                var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out var validatedToken);
                if (!IsJwtWithValidSecurityAlgorithm(validatedToken))
                {
                    return null;
                }

                return principal;
            }
            catch
            {
                return null;
            }
        }

        private bool IsJwtWithValidSecurityAlgorithm(SecurityToken validatedToken)
        {
            return validatedToken is JwtSecurityToken jwtSecurityToken &&
                   jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256,
                       StringComparison.InvariantCultureIgnoreCase);
        }
    }
}