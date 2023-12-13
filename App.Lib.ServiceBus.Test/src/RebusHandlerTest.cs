using App.Lib.ServiceBus.Test.Messages;
using App.Lib.Test;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace App.Lib.ServiceBus.Test;

public class RebusHandlerTest
{
    private readonly ILoggerProvider _loggerProvider;

    public RebusHandlerTest(ILoggerProvider loggerProvider)
    {
        _loggerProvider = loggerProvider;
    }

    [Fact]
    public void ImplementationType()
    {
        var fixture = new AppFixture(_loggerProvider);
        var bus = fixture.Services.GetRequiredService<IServiceBus>();
        bus.Should().BeOfType<RebusServiceBus>();
    }

    [Fact]
    public async Task PublishReceive()
    {
        // Arrange
        var fixture = new AppFixture(_loggerProvider);
        var bus = fixture.Services.GetRequiredService<IServiceBus>();
        var messageHandler = fixture.Services.GetRequiredService<TestMessageHandler>();
        var message = new TestMessage { Message = "Hello World" };

        // Act
        await bus.Send(message);

        // Assert
        await AssertHelper.AssertRetry(() =>
        {
            Assert.Single(messageHandler.HandledMessages);
            Assert.Equal("Hello World", messageHandler.HandledMessages.First().Message);
        });
    }
}