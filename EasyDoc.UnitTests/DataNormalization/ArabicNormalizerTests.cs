using EasyDoc.Infrastructure.Services.DataNormalization;
using Xunit;

namespace EasyDoc.UnitTests.DataNormalization;

public class ArabicNormalizerTests
{
    [Theory]
    [InlineData("أحمد", "احمد")]
    [InlineData("إبراهيم", "ابراهيم")]
    [InlineData("آمنة", "امنه")]
    public void Normalize_CollapsesAlefVariants(string input, string expected)
    {
        Assert.Equal(expected, ArabicNormalizer.Normalize(input));
    }

    [Theory]
    [InlineData("ليلى", "ليلي")]
    [InlineData("فاطمة", "فاطمه")]
    public void Normalize_CollapsesAlefMaksuraAndTehMarbuta(string input, string expected)
    {
        Assert.Equal(expected, ArabicNormalizer.Normalize(input));
    }

    [Theory]
    [InlineData("مُحَمَّد", "محمد")]
    [InlineData("مـحـمـد", "محمد")]
    [InlineData("كَتَبَ", "كتب")]
    public void Normalize_RemovesDiacriticsAndTatweel(string input, string expected)
    {
        Assert.Equal(expected, ArabicNormalizer.Normalize(input));
    }

    [Fact]
    public void Normalize_LeavesLatinTextUntouched()
    {
        Assert.Equal("Mohamed Ali", ArabicNormalizer.Normalize("Mohamed Ali"));
    }
}
