using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
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

        var clock = new Mock<IClock>();

        clock.Setup(c => c.ConvertUtcToLocalTime(It.IsAny<TimeOnly>(), "CET"))
            .Returns<TimeOnly, string>((time, timeZone) => time.AddHours(1));
        clock.Setup(c => c.ConvertUtcToLocalTime(It.IsAny<TimeOnly>(), "EET"))
            .Returns<TimeOnly, string>((time, timeZone) => time.AddHours(2));
        clock.Setup(c => c.ConvertUtcToLocalTime(It.IsAny<TimeOnly>(), "AST"))
            .Returns<TimeOnly, string>((time, timeZone) => time.AddHours(-4));

        var configuration = new ConfigurationBuilder().Build();
        var tagsRepository = new TagsRepository(logger, configuration);
        _tagManagerService = new TagManagerService(logger, tagsRepository, clock.Object);
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
    [TestCase("ON;08:00-16:00;CET;WEEK", "07:00", VMStates.TurningOn)]
    [TestCase("ON;08:00-16:00;CET;WEEK", "11:00", VMStates.Running)]
    [TestCase("ON;08:00-16:00;CET;WEEK", "15:00", VMStates.TurningOff)]
    [TestCase("ON;08:00-16:00;CET;WEEK", "16:00", VMStates.Stopped)]
    [TestCase("ON;22:00-06:00;CET;WEEK", "23:00", VMStates.Running)]
    [TestCase("ON;22:00-06:00;CET;WEEK", "01:00", VMStates.Running)]
    [TestCase("ON;22:00-06:00;CET;WEEK", "07:00", VMStates.Stopped)]
    [TestCase("ON;22:00-06:00;CET;WEEK", "20:00", VMStates.Stopped)]
    [TestCase("ON;22:00-06:00;CET;WEEK", "21:00", VMStates.TurningOn)]
    [TestCase("ON;22:00-06:00;CET;WEEK", "05:00", VMStates.TurningOff)]
    public void ShouldReturnCorrectState_WhenTagContainsOnAndTriggerTimeIs(string tagValue, string triggerTime, VMStates expectedState)
    {
        // Arrange
        var tag = new VMStartStopTagValue(tagValue);
        var triggerTimestamp = TimeOnly.FromTimeSpan(TimeSpan.Parse(triggerTime));

        // Act
        var result = _tagManagerService.IsCurrentTag(tag, triggerTimestamp);

        // Assert
        result.Should().Be(expectedState);
    }
}