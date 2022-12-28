using System;
using FluentAssertions;
using WorkStationMonitor.Core;
using Xunit;

namespace WorkStationMonitor.UnitTests;

public class ApplicationRecordTests
{
    [Theory]
    [InlineData("A:\\PROGRAM.exe", "Program")]
    [InlineData("B:\\SubFolder\\Software.exe", "Software")]
    [InlineData("C:\\SubFolderOne\\SubFolderTwo\\tool.exe", "Tool")]
    [InlineData("D:\\SubFolderOne\\SubFolderTwo\\SubFolderThree\\somethingElse.exe", "Somethingelse")]
    [InlineData("ThisIsAlsoAValidFile", "Thisisalsoavalidfile")]
    public void InstanceOfApplication_WhenFileNameIsCorrect_NameShouldBeCorrect(string fileName, string expectedApplicationName)
    {
        // Act
        var application = new Application(fileName);

        // Assert
        application.Name.Should().Be(expectedApplicationName);
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public void InstanceOfApplication_WhenFileNameIsNotCorrect_ShouldThrowArgumentException(string? fileName)
    {
        // Arrange + Act
        var action = () => new Application(fileName);

        // Assert
        action.Should().Throw<ArgumentException>();
    }
}
