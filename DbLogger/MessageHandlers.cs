using System;
using System.Reactive.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using HallonDisp;
using MqttUtils;


namespace DbLogger
{
    public class PowerMessageHandler
    {
        private string _topic;
        public PowerMessageHandler(string topic, IMqttSubscriber mqttSubscriber) {
            _topic = topic;
            mqttSubscriber.WhenMessageRecieved.Where(x => x.topic == topic).Subscribe(m => HandleMessage(m.message));
        }
        public void HandleMessage(string message)
        {
            Console.WriteLine(message);
            var powerMessage = JsonSerializer.Deserialize<PowerMessage>(message);
            Console.WriteLine(powerMessage.PowerTickPeriod);

        }
    }
    
    public class WaterMessageHandler
    {
        private string _topic;
        public WaterMessageHandler(string topic, IMqttSubscriber mqttSubscriber) {
            _topic = topic;
            mqttSubscriber.WhenMessageRecieved.Where(x => x.topic == topic).Subscribe(m => HandleMessage(m.message));
        }
        public void HandleMessage(string message)
        {
            Console.WriteLine(message);
            var waterMessage = JsonSerializer.Deserialize<WaterMessage>(message);
            Console.WriteLine(waterMessage.Angle);

        }
    }
}