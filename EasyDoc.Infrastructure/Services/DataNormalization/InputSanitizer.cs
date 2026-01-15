using System.Text.RegularExpressions;

namespace EasyDoc.Infrastructure.Services.DataNormalization;

internal static class InputSanitizer
{
    // The pattern is: Match any character that is NOT a Letter (\p{L}), 
    // a Number (\p{N}), or a space (\s).
    private static readonly Regex SyntaxCleaner =
        new Regex(@"[^\p{L}\p{N}\s]", RegexOptions.Compiled);

    public static string Sanitize(string rawInput)
    {
        if (string.IsNullOrWhiteSpace(rawInput))
        {
            return string.Empty;
        }

        // 1. Replace all dangerous FTS syntax and non-alphanumeric characters with a single space.
        string cleaned = SyntaxCleaner.Replace(rawInput, " ");

        // 2. Normalize and trim multiple spaces down to a single space.
        string normalized = Regex.Replace(cleaned, @"\s+", " ").Trim();

        return normalized;
    }
}
