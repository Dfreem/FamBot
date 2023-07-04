

using DSharpPlus.Entities;
using Serilog;

namespace FamBotTests;

[TestFixture]
public class SeanceTests
{
    [Test]
    public void Test_DiscordUtility_ButtonsFromGPTResponse_ReturnsButtons()
    {
        // Arrange

        string testResponse = "This is a test response. The following lines should math the Regex pattern and return buttons.\n\n1. [button 1] \n2. [button 2] \n3. [button 3] \n4. [button 4]";
        string[] buttonOptions = new string[4];

        // Act

        DiscordComponent[] buttons = DiscordUtility.ButtonsFromGPTResponse(buttonOptions);

        // Assert

        Assert.That(buttons, Is.Not.Empty);
        foreach (var button in buttons)
        {
            Assert.That(button, Is.InstanceOf<DiscordComponent>());
            Log.Debug(((DiscordButtonComponent)button).Label);

            Assert.That(((DiscordButtonComponent)button).Label, Is.Not.Empty);
        }
    }
}

