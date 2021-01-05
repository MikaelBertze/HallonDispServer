using System;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;
using DeviceSimulation;
using HallonDispDtos;
using MqttUtils.Messages;
using NSubstitute;
using Xunit;

namespace MqttUtils.Tests
{
    public class MessageHandlerTests
    {
        [Fact]
        public async Task IntegrationTest()
        {
            // Note: Require mosquitto on localhost port 1883. No user/password
            // To run with docker:
            // > docker run -d -p 1883:1883 -p 9001:9001 eclipse-mosquitto

            // Arrange
            var factory = new MqttClientFactory();

            var simulator = new PowerSimulator(100, 200, 10,
                await factory.GetConnectedMqttClient("localhost", null, null, "sim"), "hallondisptest/power");
            var tokenSource = simulator.Run();
            var client = await factory.GetConnectedMqttClient("localhost", null, null, "mht");
            var subscriber = new MqttSubscriber(client);

            var sut = new MessageHandler<PowerMessage>("hallondisptest/power", subscriber);

            PowerMessage powerMessage = null;

            var waitHanle = new AutoResetEvent(false);
            sut.WhenMessageReceived.Subscribe(x =>
            {
                powerMessage = x;
                waitHanle.Set();
            });
            
            waitHanle.WaitOne(TimeSpan.FromSeconds(5));

            // Assert
            Assert.NotNull(powerMessage);
            Assert.InRange(powerMessage.PowerTickPeriodMs, 100, 200);

            tokenSource.Cancel();
        }

        [Fact]
        public void Construction_ObservableIsSubscribed()
        {
            // Arrange
            var mqttSubscriber = Substitute.For<IMqttSubscriber>();
            var whenMessageReceivedObservable =
                Substitute.For<IObservable<(string, string)>>(); //  new Subject<(string topic, string message)>();
            mqttSubscriber.WhenMessageReceived.Returns(whenMessageReceivedObservable);

            // Act
            var sut = new MessageHandler<PowerMessage>("power", mqttSubscriber);

            // Assert
            whenMessageReceivedObservable.Received().Subscribe(Arg.Any<IObserver<(string, string)>>());
        }

        [Fact]
        public void PowerMessage_Received()
        {
            // Arrange
            var mqttSubscriber = Substitute.For<IMqttSubscriber>();
            var whenMessageReceivedObservable = new Subject<(string topic, string message)>();
            mqttSubscriber.WhenMessageReceived.Returns(whenMessageReceivedObservable);
            var sut = new MessageHandler<PowerMessage>("power", mqttSubscriber);
            PowerMessage powerMessage = null;
            sut.WhenMessageReceived.Subscribe(x => { powerMessage = x; });
            string iotMessage = "{ \"id\" : \"garage\", \"power_tick_period\" : 1778 }";

            // Act
            whenMessageReceivedObservable.OnNext(("power", iotMessage));

            // Assert
            Assert.NotNull(powerMessage);
            Assert.Equal("garage", powerMessage.Id);
            Assert.Equal(1778, powerMessage.PowerTickPeriodMs);
        }
        
        [Fact]
        public void TempMessage_Received()
        {
            // Arrange
            var mqttSubscriber = Substitute.For<IMqttSubscriber>();
            var whenMessageReceivedObservable = new Subject<(string topic, string message)>();
            mqttSubscriber.WhenMessageReceived.Returns(whenMessageReceivedObservable);
            var sut = new MessageHandler<TempMessage>("temperature", mqttSubscriber);
            TempMessage tempMessage = null;
            sut.WhenMessageReceived.Subscribe(x => { tempMessage = x; });
            string iotMessage = "{ \"id\" : \"test\", \"temp\" : 13.9 }";

            // Act
            whenMessageReceivedObservable.OnNext(("temperature", iotMessage));

            // Assert
            Assert.NotNull(tempMessage);
            Assert.Equal("test", tempMessage.Id);
            Assert.InRange(tempMessage.Temp, 13.89, 13.91);
        }

        [Fact]
        public void WaterMessage()
        {
            // Arrange
            var mqttSubscriber = Substitute.For<IMqttSubscriber>();
            var whenMessageReceivedObservable = new Subject<(string topic, string message)>();
            mqttSubscriber.WhenMessageReceived.Returns(whenMessageReceivedObservable);
            var sut = new MessageHandler<WaterMessage>("water", mqttSubscriber);
            WaterMessage waterMessage = null;
            sut.WhenMessageReceived.Subscribe(x => { waterMessage = x; });
            string iotMessage =
                "{ \"id\" : \"water_thingy\", \"angle\" : 10, \"diff\" : 1, \"t_diff\" : 559, \"consumption\" : 1.2, \"acc_consumption\" : 228.1960825920 , \"times\" : [507, 559] }";

            // Act
            whenMessageReceivedObservable.OnNext(("water", iotMessage));

            // Assert
            var e = .001;
            Assert.NotNull(waterMessage);
            Assert.Equal("water_thingy", waterMessage.Id);
            Assert.Equal(10, waterMessage.Angle);
            Assert.InRange(waterMessage.Consumption, 1.2 - e, 1.2 + e);
            Assert.Equal(1, waterMessage.Diff);
            Assert.Equal(559, waterMessage.TimeDiffMs);
        }

        [Fact]
        public void PowerMessage()
        {
            // Arrange
            var mqttSubscriber = Substitute.For<IMqttSubscriber>();
            var whenMessageReceivedObservable = new Subject<(string topic, string message)>();
            mqttSubscriber.WhenMessageReceived.Returns(whenMessageReceivedObservable);
            var sut = new MessageHandler<PowerMessage>("power", mqttSubscriber);
            PowerMessage powerMessage = null;
            sut.WhenMessageReceived.Subscribe(x => { powerMessage = x; });
            string iotMessage = "{ \"id\" : \"power_thingy\", \"power_tick_period\" : 1068 }";

            // Act
            whenMessageReceivedObservable.OnNext(("power", iotMessage));

            // Assert
            Assert.NotNull(powerMessage);
            Assert.Equal("power_thingy", powerMessage.Id);
            Assert.Equal(1068, powerMessage.PowerTickPeriodMs);
        }
        
        [Fact]
        public void SwitchMessage()
        {
            // Arrange
            var mqttSubscriber = Substitute.For<IMqttSubscriber>();
            var whenMessageReceivedObservable = new Subject<(string topic, string message)>();
            mqttSubscriber.WhenMessageReceived.Returns(whenMessageReceivedObservable);
            var sut = new MessageHandler<SwitchMessage>("switch", mqttSubscriber);
            SwitchMessage switchMessage = null;
            sut.WhenMessageReceived.Subscribe(x => { switchMessage = x; });
            string iotMessage = "{ \"id\" : \"switch_thingy\", \"state\" : true }";

            // Act
            whenMessageReceivedObservable.OnNext(("switch", iotMessage));

            // Assert
            Assert.NotNull(switchMessage);
            Assert.Equal("switch_thingy", switchMessage.Id);
            Assert.True(switchMessage.State);
        }
    }
}