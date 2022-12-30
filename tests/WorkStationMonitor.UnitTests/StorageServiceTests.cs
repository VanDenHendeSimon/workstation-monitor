using System;
using System.Collections.Immutable;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using InfluxDB.Client;
using InfluxDB.Client.Writes;
using Moq;
using Snapshooter.Xunit;
using WorkStationMonitor.Core;
using WorkStationMonitor.Storage;
using Xunit;

namespace WorkStationMonitor.UnitTests;

public class StorageServiceTests
{
    private const string Bucket = "Bucket";
    private const string Organisation = "MyOrg";

    private static StorageService GetSut(IMock<IWriteApiAsync> writeApiAsyncMock)
    {
        Environment.SetEnvironmentVariable("DOCKER_INFLUXDB_INIT_PORT", "8086");
        Environment.SetEnvironmentVariable("DOCKER_INFLUXDB_INIT_ADMIN_TOKEN", "token");
        Environment.SetEnvironmentVariable("DOCKER_INFLUXDB_APPLICATION_BUCKET", Bucket);
        Environment.SetEnvironmentVariable("DOCKER_INFLUXDB_INIT_ORG", Organisation);

        var mockClient = new Mock<IInfluxDBClient>();

        // Make sure the Mock Write API is used
        mockClient
            .Setup(x => x.GetWriteApiAsync(It.IsAny<IDomainObjectMapper>()))
            .Returns(writeApiAsyncMock.Object);

        // Assign our Mock Client to the private readonly field (not passed via ctor bcus no DI...)
        using var sut = new StorageService();
        var field = sut.GetType().GetField("_client", BindingFlags.Instance | BindingFlags.NonPublic);
        field!.SetValue(sut, mockClient.Object);

        return sut;
    }

    [Fact]
    public async Task WriteApplicationAsync_WritesSinglePointIntoCorrectBucketAndOrg()
    {
        // Arrange
        var application = new Application("C:\\Program Files\\Blender Foundation\\Blender 3.2\\blender.exe");

        var writeApiAsyncMock = new Mock<IWriteApiAsync>();
        var sut = GetSut(writeApiAsyncMock);

        // Act
        await sut.WriteApplicationAsync(application);

        // Assert
        writeApiAsyncMock.Verify(
            x => x.WritePointAsync(
                It.IsAny<PointData>(),
                Bucket,
                Organisation,
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task WriteApplicationAsync_WritesPointWithCorrectValue()
    {
        // Arrange
        var application = new Application("C:\\Program Files\\Blender Foundation\\Blender 3.2\\blender.exe");

        var writeApiAsyncMock = new Mock<IWriteApiAsync>();
        var sut = GetSut(writeApiAsyncMock);

        PointData? capturedPointData = null;
        writeApiAsyncMock.Setup(
                x => x.WritePointAsync(
                    It.IsAny<PointData>(),
                    Bucket,
                    Organisation,
                    It.IsAny<CancellationToken>()))
            .Callback<PointData, string, string, CancellationToken>((pointData, _, _, _) =>
                capturedPointData = pointData);

        // Act
        await sut.WriteApplicationAsync(application);

        // Assert
        capturedPointData.Should().NotBeNull();

        // Have to use reflection to get private readonly fields for validation
        var measurementName = GetNonPublicFieldValue<PointData, string>(capturedPointData, "_measurementName");
        var tags = GetNonPublicFieldValue<PointData, ImmutableSortedDictionary<string, string>>(capturedPointData, "_tags");
        var fields = GetNonPublicFieldValue<PointData, ImmutableSortedDictionary<string, object>>(capturedPointData, "_fields");

        // Creating a anonymous object to use in Snapshot validation (rather than asserting each field manually with the expected value)
        var data = new
        {
            capturedPointData!.Precision, // Only public property on the PointData class
            measurementName,
            tags,
            fields,
        };
        data.MatchSnapshot();
    }

    private TProperty? GetNonPublicFieldValue<TClass, TProperty>(TClass instance, string fieldName)
    {
        if (instance is null) return default;

        var type = instance.GetType();
        var property = type.GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic);
        
        if (property is null) return default;
        return (TProperty)property.GetValue(instance)!;
    }
}
