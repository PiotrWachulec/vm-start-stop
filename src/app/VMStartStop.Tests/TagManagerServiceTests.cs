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
        var configuration = new ConfigurationBuilder().Build();
        var tagsRepository = new TagsRepository(logger, configuration);
        _tagManagerService = new TagManagerService(logger, tagsRepository);
    }

    [Test]
    public void ShouldReturnOmit_WhenTagContainsOff()
    {
        // Arrange
        var tagValue = new VMStartStopTagValue("OFF;08:00-16:00;CET;WEEK");
        var triggerTimestamp = TimeOnly.FromTimeSpan(TimeSpan.Parse("08:00"));

        // Act
        var result = _tagManagerService.IsCurrentTag(tagValue, triggerTimestamp);

        // Assert
        result.Should().Be(VMStates.Omit);
    }

    [Test]
    public void ShouldNotReturnOmit_WhenTagContainsOn()
    {
        // Arrange
        var tagValue = new VMStartStopTagValue("ON;08:00-16:00;CET;WEEK");
        var triggerTimestamp = TimeOnly.FromTimeSpan(TimeSpan.Parse("08:00"));

        // Act
        var result = _tagManagerService.IsCurrentTag(tagValue, triggerTimestamp);

        // Assert
        result.Should().NotBe(VMStates.Omit);
    }

    [Test]
    public void ShouldReturnOmit_WhenTagNotContainsOn()
    {
        // Arrange
        var tagValue = new VMStartStopTagValue("OFF;08:00-16:00;CET;WEEK");
        var triggerTimestamp = TimeOnly.FromTimeSpan(TimeSpan.Parse("08:00"));


        // Act
        var result = _tagManagerService.IsCurrentTag(tagValue, triggerTimestamp);

        // Assert
        result.Should().Be(VMStates.Omit);
    }

    [Test]
    public void ShouldReturnTurningOn_WhenTagContainsOnAndTriggerTimeIsAtStartTime()
    {
        // Arrange
        var tagValue = new VMStartStopTagValue("ON;08:00-16:00;CET;WEEK");
        var triggerTimestamp = TimeOnly.FromTimeSpan(TimeSpan.Parse("08:00"));

        // Act
        var result = _tagManagerService.IsCurrentTag(tagValue, triggerTimestamp);

        // Assert
        result.Should().Be(VMStates.TurningOn);
    }

    [Test]
    public void ShouldReturnRunning_WhenTagContainsOnAndTriggerTimeIsBetweenStartTimeAndEndTime()
    {
        // Arrange
        var tagValue = new VMStartStopTagValue("ON;08:00-16:00;CET;WEEK");
        var triggerTimestamp = TimeOnly.FromTimeSpan(TimeSpan.Parse("12:00"));

        // Act
        var result = _tagManagerService.IsCurrentTag(tagValue, triggerTimestamp);

        // Assert
        result.Should().Be(VMStates.Running);
    }

    [Test]
    public void ShouldReturnTurningOff_WhenTagContainsOnAndTriggerTimeIsAtEndTime()
    {
        // Arrange
        var tagValue = new VMStartStopTagValue("ON;08:00-16:00;CET;WEEK");
        var triggerTimestamp = TimeOnly.FromTimeSpan(TimeSpan.Parse("16:00"));

        // Act
        var result = _tagManagerService.IsCurrentTag(tagValue, triggerTimestamp);

        // Assert
        result.Should().Be(VMStates.TurningOff);
    }

    [Test]
    public void ShouldReturnStopped_WhenTagContainsOnAndTriggerTimeIsAfterEndTime()
    {
        // Arrange
        var tagValue = new VMStartStopTagValue("ON;08:00-16:00;CET;WEEK");
        var triggerTimestamp = TimeOnly.FromTimeSpan(TimeSpan.Parse("17:00"));

        // Act
        var result = _tagManagerService.IsCurrentTag(tagValue, triggerTimestamp);

        // Assert
        result.Should().Be(VMStates.Stopped);
    }
}