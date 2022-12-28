using FluentAssertions;
using WorkStationMonitor.Core.Extensions;
using Xunit;

namespace WorkStationMonitor.UnitTests;

public class StringExtensionTests
{
    [Theory]
    [InlineData("", "")]
    [InlineData("a", "A")]
    [InlineData("A", "A")]
    [InlineData("Aa", "Aa")]
    [InlineData("aA", "Aa")]
    [InlineData("aa", "Aa")]
    [InlineData("AA", "Aa")]
    public void Capitalize_WithSpecificInputString_ShouldBeCapitalizedCorrectly(
        string inputStr,
        string expectedOutputStr)
    {
        // Act
        var actualOutputStr = inputStr.Capitalize();

        // Assert
        actualOutputStr.Should().Be(expectedOutputStr);
    }
}
