using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MQTTnet;
using MQTTnet.Client.Receiving;
using MQTTnet.Extensions.ManagedClient;
using NSubstitute;
using Xunit;

namespace MqttUtils.Tests
{
    public class MqttSubscriberTests
    {
        private IMqttApplicationMessageReceivedHandler _messageReceiveHandler;

        [Fact]
        public async Task IntegrationTest()
        {
            var waitHanle = new AutoResetEvent(false);
            var server = "test.mosquitto.org";
            var topic = "hallondisp/unittest";
            var factory = new MqttClientFactory();

            var client1 = await factory.GetConnectedMqttClient(server, null, null);
            var client2 = await factory.GetConnectedMqttClient(server, null, null);
            
            var sut = new MqttSubscriber(client2);
            string receivedTopic = null;
            string receivedMessage = null;
            sut.WhenMessageReceived.Subscribe(x =>
            {
                receivedMessage = x.message;
                receivedTopic = x.topic;
                waitHanle.Set();
            });

            await sut.RegisterTopic(topic);

            await client1.PublishAsync(new MqttApplicationMessageBuilder()
                .WithTopic(topic)
                .WithPayload(Encoding.UTF8.GetBytes("test message"))
                .Build());

            Assert.True(waitHanle.WaitOne(TimeSpan.FromSeconds(2)), "Timeout waiting for message to be received");
            Assert.Equal("test message", receivedMessage);
            Assert.Equal(topic, receivedTopic);
        }
        
        [Fact]
        public async Task RegisterTopic()
        {
            // Arrange
            var (client, sut) = _SetupSubscriber();
            
            // Act
            await sut.RegisterTopic("topic1");
            await sut.RegisterTopic("topic2");

            // Assert
            await client.Received(1)
                .SubscribeAsync(Arg.Is<IEnumerable<MqttTopicFilter>>(x => x.Single().Topic == "topic1"));
            await client.Received(1)
                .SubscribeAsync(Arg.Is<IEnumerable<MqttTopicFilter>>(x => x.Single().Topic == "topic2"));

        }
        
        [Fact]
        public void MessageReceived_ShallFireOnObservable()
        {
            // Arrange
            var (_, sut) = _SetupSubscriber();
            string topic = null;
            string message = null;
            sut.WhenMessageReceived.Subscribe(x =>
            {
                topic = x.topic;
                message = x.message;
            });

            _messageReceiveHandler.HandleApplicationMessageReceivedAsync(
                new MqttApplicationMessageReceivedEventArgs("test",
                    new MqttApplicationMessage() {Topic = "topic1", Payload = Encoding.UTF8.GetBytes("message1")}));
            Assert.Equal("topic1", topic);
            Assert.Equal("message1", message);
        }

        [Fact]
        public void WhenMassageReceived()
        {
            // Arrange
            var (_, sut) = _SetupSubscriber();
            sut.RegisterTopic("test");
            
            string topic = null;
            string message = null;
            sut.WhenMessageReceived.Subscribe(x =>
            {
                topic = x.topic;
                message = x.message;
            });
            byte[] payLoad = Encoding.UTF8.GetBytes("message"); 
            
            // Act
            _messageReceiveHandler.HandleApplicationMessageReceivedAsync(
                new MqttApplicationMessageReceivedEventArgs("test",
                    new MqttApplicationMessage() {Topic = "test", Payload = payLoad}));

            // Assert
            Assert.Equal("test", topic);
            Assert.Equal("message", message);
        }

        [Fact]
        public async Task RegisterTopic_NotConnected_ThrowsException()
        {
            var (client, sut) = _SetupSubscriber();
            client.IsConnected.Returns(false);

            await Assert.ThrowsAsync<ArgumentException>(async () =>
            {
                await sut.RegisterTopic("test");
            });
        } 

        private (IManagedMqttClient, MqttSubscriber) _SetupSubscriber()
        {
            var mqttClient = Substitute.For<IManagedMqttClient>();
            mqttClient.IsConnected.Returns(true);
            mqttClient.IsStarted.Returns(true);
            mqttClient.UseApplicationMessageReceivedHandler(Arg.Do<IMqttApplicationMessageReceivedHandler>(x => _messageReceiveHandler = x));    
            var mqttSubscriber = new MqttSubscriber(mqttClient);
            Assert.NotNull(_messageReceiveHandler);
            return (mqttClient, mqttSubscriber);
        }
    }
}