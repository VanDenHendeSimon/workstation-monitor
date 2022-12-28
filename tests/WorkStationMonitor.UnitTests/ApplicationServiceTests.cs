using System;
using System.ComponentModel;
using System.Diagnostics;
using FluentAssertions;
using Moq;
using WorkStationMonitor.Core;
using Xunit;

namespace WorkStationMonitor.UnitTests;

public class ApplicationServiceTests
{
    private Mock<IProcessModuleProvider> _processModuleProviderMock;
    private ApplicationService _applicationService;

    [Fact]
    public void GetActiveApplication_WhenActiveProcessModuleIsNull_ShouldReturnNull()
    {
        // Arrange
        _processModuleProviderMock = new Mock<IProcessModuleProvider>();
        _applicationService = new ApplicationService(_processModuleProviderMock.Object);

        _processModuleProviderMock
            .Setup(p => p.GetActiveProcessModule())
            .Returns((ProcessModule?)null);

        // Act
        var application = _applicationService.GetActiveApplication();

        // Assert
        application.Should().BeNull();
    }

    [Fact]
    public void GetActiveApplication_WhenActiveProcessModuleFileNameIsNull_ShouldReturnNull()
    {
        // Arrange
        _processModuleProviderMock = new Mock<IProcessModuleProvider>();
        _applicationService = new ApplicationService(_processModuleProviderMock.Object);

        _processModuleProviderMock
            .Setup(p => p.GetActiveProcessModule())
            .Returns(Data.ProcessModuleWithoutFileName);

        // Act
        var application = _applicationService.GetActiveApplication();

        // Assert
        application.Should().BeNull();
    }

    [Fact]
    public void GetActiveApplication_WhenGetActiveProcessModuleThrowsWin32Exception_ShouldReturnNull()
    {
        // Arrange
        _processModuleProviderMock = new Mock<IProcessModuleProvider>();
        _applicationService = new ApplicationService(_processModuleProviderMock.Object);

        _processModuleProviderMock
            .Setup(p => p.GetActiveProcessModule())
            .Throws<Win32Exception>();

        // Act
        var application = _applicationService.GetActiveApplication();

        // Assert
        application.Should().BeNull();
    }

    [Fact]
    public void GetActiveApplication_WhenGetActiveProcessModuleThrowsSomeException_ShouldBeThrownUp()
    {
        // Arrange
        _processModuleProviderMock = new Mock<IProcessModuleProvider>();
        _applicationService = new ApplicationService(_processModuleProviderMock.Object);

        _processModuleProviderMock
            .Setup(p => p.GetActiveProcessModule())
            .Throws<Exception>();

        // Act
        var action = () => _applicationService.GetActiveApplication();

        // Assert
        action.Should().Throw<Exception>();
    }

    [Fact]
    public void GetActiveApplication_WhenActiveProcessModuleFileNameIsValid_ShouldReturnTheCorrectApplication()
    {
        // Arrange
        _processModuleProviderMock = new Mock<IProcessModuleProvider>();
        _applicationService = new ApplicationService(_processModuleProviderMock.Object);

        _processModuleProviderMock
            .Setup(p => p.GetActiveProcessModule())
            .Returns(Data.BlenderProcessModule);

        // Act
        var application = _applicationService.GetActiveApplication();

        // Assert
        application.Should().NotBeNull();
        application!.Name.Should().Be("Blender");
    }

    private static class Data
    {
        public static readonly ProcessModule? ProcessModuleWithoutFileName = GetProcessModuleWithSpecificFileName(null);

        public static readonly ProcessModule? BlenderProcessModule = GetProcessModuleWithSpecificFileName("C:\\Program Files\\Blender Foundation\\Blender 3.2\\blender.exe");

        private static ProcessModule? GetProcessModuleWithSpecificFileName(string? fileName)
        {
            var processModule = Process.GetCurrentProcess().MainModule;

            var type = typeof(ProcessModule);
            var property = type.GetProperty("FileName")
                ?? throw new Exception("Could not locate the property FileName on type ProcessModule");

            property.SetValue(processModule, fileName);
            return processModule;
        }
    }
}
