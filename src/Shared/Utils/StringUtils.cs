using System.Text.RegularExpressions;

namespace TaskManagement.src.Shared.Utils;

public static class StringUtils
{
    public static bool NormalizedEquals(this string value, string other)
    {
        var normalizedValue = Regex.Replace(value.Trim(), @"\s+", " ");
        var normalizedOther = Regex.Replace(other.Trim(), @"\s+", " ");

        return string.Equals(normalizedValue, normalizedOther, StringComparison.OrdinalIgnoreCase);
    }
}
