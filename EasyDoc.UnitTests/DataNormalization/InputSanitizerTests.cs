using EasyDoc.Infrastructure.Services.DataNormalization;
using Xunit;

namespace EasyDoc.UnitTests.DataNormalization;

public class InputSanitizerTests
{
    [Theory]
    [InlineData("  أحمد, حسن!  ", "أحمد حسن")]
    [InlineData("Mohamed--Ali", "Mohamed Ali")]
    [InlineData("a  b   c", "a b c")]
    [InlineData("", "")]
    [InlineData("   ", "")]
    public void Sanitize_RemovesSyntaxAndCollapsesWhitespace(string input, string expected)
    {
        Assert.Equal(expected, InputSanitizer.Sanitize(input));
    }

    [Fact]
    public void Sanitize_KeepsArabicLettersAndDigits()
    {
        Assert.Equal("أحمد 123", InputSanitizer.Sanitize("أحمد 123"));
    }
}
