using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MyCo.TagManager;
using MyCo.VMStartStop;
using FluentAssertions;

namespace VMStartStop.Tests;

[TestFixture]
public class TagManagerServiceTests
{
    private TagManagerService _tagManagerService;

    [SetUp]
    public void SetUp()
    {
        var serviceProvider = new ServiceCollection().AddLogging().BuildServiceProvider();
        var logger = serviceProvider.GetService<ILoggerFactory>();
        _tagManagerService = new TagManagerService(logger);
    }

    [Test]
    public void ShouldReturnOmit_WhenTagContainsOff()
    {
        // Arrange
        string tagValue = "OFF;08:00-16:00;CET;WEEK";

        // Act
        var result = _tagManagerService.IsCurrentTag(tagValue);

        // Assert
        result.Should().Be(VMStates.Omit);
    }

    [Test]
    public void ShouldNotReturnOmit_WhenTagContainsOn()
    {
        // Arrange
        string tagValue = "OFF;08:00-16:00;CET;WEEK";

        // Act
        var result = _tagManagerService.IsCurrentTag(tagValue);

        // Assert
        result.Should().NotBe(VMStates.Omit);
    }

    [Test]
    public void ShouldReturnOmit_WhenTagNotContainsOn()
    {
        // Arrange
        string tagValue = "OFF;08:00-16:00;CET;WEEK";

        // Act
        var result = _tagManagerService.IsCurrentTag(tagValue);

        // Assert
        result.Should().Be(VMStates.Omit);
    }
}