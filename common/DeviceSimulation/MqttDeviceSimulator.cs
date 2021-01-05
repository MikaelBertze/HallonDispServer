using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using HallonDispDtos;
using MQTTnet;
using MQTTnet.Extensions.ManagedClient;
using MqttUtils;
using MqttUtils.Messages;
using Newtonsoft.Json;

namespace DeviceSimulation
{
    public abstract class MqttDeviceSimulator<T>
    {
        protected readonly IManagedMqttClient _client;
        protected readonly string _topic;
        private CancellationTokenSource _tokenSource = new CancellationTokenSource();

        protected MqttDeviceSimulator(IManagedMqttClient client, string topic)
        {
            _client = client;
            _topic = topic;
        }

        protected abstract T CreateMessage(out int timeUntilNextMessageMs);
        
        public CancellationTokenSource Run()
        {
            // Define the cancellation token.
            var source = new CancellationTokenSource();
            var token = source.Token;
            Task.Run(async () =>
            {
                while (!token.IsCancellationRequested)
                {
                    var message = CreateMessage(out var t);
                    await Task.Delay(TimeSpan.FromMilliseconds(t));
                    await _client.PublishAsync(new MqttApplicationMessageBuilder()
                        .WithTopic(_topic)
                        .WithPayload(
                            Encoding.UTF8.GetBytes(
                                JsonConvert.SerializeObject(message, new PowerMessageConverter())))
                        .Build());
                }
                await _client.StopAsync();
            }, token);
            return source;
        }
    }
    
    
}