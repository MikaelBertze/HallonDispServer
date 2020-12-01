using System;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Client.Options;
using MQTTnet.Extensions.ManagedClient;

namespace MqttUtils
{
    public interface IMqttSubscriber {
        Task Init();
        IObservable<(string topic, string message)> WhenMessageRecieved {get;}
    }
    public class MqttSubscriber : IMqttSubscriber
    {
        IManagedMqttClient _mqttClient;
        Subject<(string topic, string message)> _whenMessageRecieved = new Subject<(string topic, string message)>();

        
        public MqttSubscriber()
        {
            // Create a new MQTT client.
            
        }

        public async Task Init()
        {
            // Setup and start a managed MQTT client.
            var options = new ManagedMqttClientOptionsBuilder()
                .WithAutoReconnectDelay(TimeSpan.FromSeconds(5))
                .WithClientOptions(new MqttClientOptionsBuilder()
                    .WithClientId("Client1")
                    .WithTcpServer("mew.bertze.se")
                    .WithCredentials("hallondisp", "disphallon")
                    .Build())
                .Build();

            _mqttClient = new MqttFactory().CreateManagedMqttClient();
            await _mqttClient.SubscribeAsync(new MqttTopicFilterBuilder().WithTopic("#") .Build());
            _mqttClient.UseConnectedHandler(e => { Console.WriteLine("connected"); });
            _mqttClient.UseApplicationMessageReceivedHandler(e => 
            { 
                _whenMessageRecieved.OnNext((e.ApplicationMessage.Topic, Encoding.UTF8.GetString(e.ApplicationMessage.Payload)));
            });
            
            await _mqttClient.StartAsync(options);
        }

        public IObservable<(string topic, string message)> WhenMessageRecieved => _whenMessageRecieved;
    }
}
