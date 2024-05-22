using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MyCo.TagManager.Application;
using MyCo.TagManager.Domain;
using MyCo.TagManager.Infrastrucutre;

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
        var tagsRepository = new TagsRepository(logger);
        _tagManagerService = new TagManagerService(logger, tagsRepository);
    }

    [Test]
    public void ShouldReturnOmit_WhenTagContainsOff()
    {
        // Arrange
        var tagValue = new VMStartStopTagValue("OFF;08:00-16:00;CET;WEEK");

        // Act
        var result = _tagManagerService.IsCurrentTag(tagValue);

        // Assert
        result.Should().Be(VMStates.Omit);
    }

    [Test]
    public void ShouldNotReturnOmit_WhenTagContainsOn()
    {
        // Arrange
        var tagValue = new VMStartStopTagValue("ON;08:00-16:00;CET;WEEK");

        // Act
        var result = _tagManagerService.IsCurrentTag(tagValue);

        // Assert
        result.Should().NotBe(VMStates.Omit);
    }

    [Test]
    public void ShouldReturnOmit_WhenTagNotContainsOn()
    {
        // Arrange
        var tagValue = new VMStartStopTagValue("OFF;08:00-16:00;CET;WEEK");

        // Act
        var result = _tagManagerService.IsCurrentTag(tagValue);

        // Assert
        result.Should().Be(VMStates.Omit);
    }
}