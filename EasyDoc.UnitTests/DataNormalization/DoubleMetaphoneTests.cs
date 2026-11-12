using EasyDoc.Infrastructure.Services.DataNormalization;
using Xunit;

namespace EasyDoc.UnitTests.DataNormalization;

public class DoubleMetaphoneTests
{
    [Theory]
    [InlineData("Mohamed")]
    [InlineData("Mohammed")]
    [InlineData("Mohamad")]
    public void GetKeys_ReturnsTheSamePhoneticKeyForMohamedVariants(string name)
    {
        Assert.Contains("MHMT", DoubleMetaphone.GetKeys(name));
    }

    [Fact]
    public void GetKeys_ReturnsKeysForAFullLatinName()
    {
        var keys = DoubleMetaphone.GetKeys("Kareem Mohamed").ToList();

        Assert.Contains("KRM", keys);
        Assert.Contains("MHMT", keys);
    }

    [Fact]
    public void GetKeys_ReturnsNoMeaningfulKeysForArabicOnlyInput()
    {
        var keys = DoubleMetaphone.GetKeys("أحمد");

        Assert.All(keys, key => Assert.True(string.IsNullOrWhiteSpace(key)));
    }

    [Fact]
    public void GetKeys_IgnoresShortTokens()
    {
        Assert.Empty(DoubleMetaphone.GetKeys("Al"));
    }
}
