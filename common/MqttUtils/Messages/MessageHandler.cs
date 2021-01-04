using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Newtonsoft.Json;

namespace MqttUtils.Messages
{
    public interface IMessageHandler<T>
    {
        IObservable<T> WhenMessageReceived { get; }
    }
    public class MessageHandler<T> : IMessageHandler<T>
    {
        private readonly JsonConverter[] _converters = {new PowerMessageConverter(), new WaterMessageConverter()};
        private readonly Subject<T> _whenMessageReceived = new Subject<T>();
        public MessageHandler(string topic, IMqttSubscriber mqttSubscriber)
        {
            mqttSubscriber.RegisterTopic(topic);
            mqttSubscriber.WhenMessageReceived.Where(x => x.topic == topic).Subscribe(m => _HandleMessage(m.message));
        }
        
        public IObservable<T> WhenMessageReceived => _whenMessageReceived;

        private void _HandleMessage(string message)
        {
            var typedMessage = JsonConvert.DeserializeObject<T>(message, _converters);
            if (typedMessage != null)
                _whenMessageReceived.OnNext(typedMessage);
        }
    }
}