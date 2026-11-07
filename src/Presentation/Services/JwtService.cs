using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Options;
using TaskManagement.src.Infrastructure.Options;
using TaskManagement.src.Shared.Dtos.Users;
using JwtPayload = TaskManagement.src.Presentation.Models.JwtPayload;

namespace TaskManagement.src.Presentation.Services;

public class JwtService(IOptions<JwtOptions> options)
{
    private readonly IOptions<JwtOptions> _options = options;

    public TokenValidationParameters GetTokenValidationParameters()
    {
        var encodedSigningKey = Encoding.UTF8.GetBytes(_options.Value.SigningKey);

        return new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(encodedSigningKey),
            ValidAlgorithms = [SecurityAlgorithms.HmacSha256],
            RequireSignedTokens = true,
            RequireExpirationTime = true,
            ValidateLifetime = true
        };
    }

    public string Generate(UserDetailDto user)
    {
        var encodedSigningKey = Encoding.UTF8.GetBytes(_options.Value.SigningKey);
        var securityKey = new SymmetricSecurityKey(encodedSigningKey);

        var issuedAt = DateTimeOffset.UtcNow;
        var unixTimeSeconds = issuedAt.ToUnixTimeSeconds().ToString(CultureInfo.InvariantCulture);

        var token = new JwtSecurityToken(
            claims:
            [
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Name, user.Name),
                new Claim(JwtRegisteredClaimNames.Iat, unixTimeSeconds, ClaimValueTypes.Integer64)
            ],
            expires: issuedAt.AddHours(1).UtcDateTime,
            signingCredentials: new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256)
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public JwtPayload GetPayload(ClaimsPrincipal principal)
    {
        if (principal.Identity?.IsAuthenticated != true)
        {
            throw new InvalidOperationException("User is not authenticated.");
        }

        var value = principal.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!Guid.TryParse(value, out var id) || id == Guid.Empty)
        {
            throw new InvalidOperationException("Invalid user ID.");
        }

        return new JwtPayload(id);
    }
}
