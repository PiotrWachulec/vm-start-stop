using MyCo.TagManager;
using MyCo.VMStartStop;
using NUnit.Framework;

namespace VMStartStop.Tests;

[TestFixture]
public class TagManagerServiceTests
{
    private TagManagerService _tagManagerService;

    [SetUp]
    public void SetUp(TagManagerService tagManagerService)
    {
        _tagManagerService = tagManagerService;
    }

    [Test]
    public void IsOmit_TagIsOff_ReturnsOmit()
    {
        // Arrange
        string tagValue = "OFF;08:00-16:00;CET;WEEK";

        // Act
        var result = _tagManagerService.IsCurrentTag(tagValue);

        // Assert
        Assert.That(result, Is.EqualTo(VMStates.Omit));
    }
}