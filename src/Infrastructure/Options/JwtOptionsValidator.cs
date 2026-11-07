using System.Text;
using Microsoft.Extensions.Options;

namespace TaskManagement.src.Infrastructure.Options;

public class JwtOptionsValidator : IValidateOptions<JwtOptions>
{
    public ValidateOptionsResult Validate(string? name, JwtOptions options)
    {
        if (string.IsNullOrWhiteSpace(options.SigningKey))
        {
            return ValidateOptionsResult.Fail(
                "Jwt:SigningKey must be configured and cannot be empty or whitespace.");
        }

        if (Encoding.UTF8.GetByteCount(options.SigningKey) < 32)
        {
            return ValidateOptionsResult.Fail(
                "Jwt:SigningKey must contain at least 32 bytes (256 bits) for HS256.");
        }

        return ValidateOptionsResult.Success;
    }
}
