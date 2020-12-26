using System;
using System.Collections.Generic;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;
using MQTTnet;
using MQTTnet.Extensions.ManagedClient;

namespace MqttUtils
{
    public interface IMqttSubscriber {
        IObservable<(string topic, string message)> WhenMessageReceived {get;}
        Task RegisterTopic(string topic);
    }
    public class MqttSubscriber : IMqttSubscriber
    {
        private readonly IManagedMqttClient _mqttClient;
        private readonly List<string> _subscribedTopics = new List<string>();
        private readonly Subject<(string topic, string message)> _whenMessageReceived = new Subject<(string topic, string message)>();
        
        public MqttSubscriber(IManagedMqttClient mqttClient)
        {
            _mqttClient = mqttClient;
            if (!mqttClient.IsConnected)
                throw new ArgumentException("The provided mqtt client should be connected");
            
            mqttClient.UseApplicationMessageReceivedHandler(e => 
            {
                _whenMessageReceived.OnNext((e.ApplicationMessage.Topic, Encoding.UTF8.GetString(e.ApplicationMessage.Payload)));
            });
        }
        
        public IObservable<(string topic, string message)> WhenMessageReceived => _whenMessageReceived;
        
        public async Task RegisterTopic(string topic)
        {
            if (!_mqttClient.IsConnected)
            {
                throw new ArgumentException("MQTT client must be connected before subscribing to topics");
            }

            if (!_subscribedTopics.Contains(topic))
            {
                _subscribedTopics.Add(topic);
                await _mqttClient.SubscribeAsync(new MqttTopicFilterBuilder().WithTopic(topic).Build());
            }
        }
    }
}
