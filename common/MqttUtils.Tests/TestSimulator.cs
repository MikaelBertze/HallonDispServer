using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using HallonDispDtos;
using MQTTnet;
using MQTTnet.Extensions.ManagedClient;
using MqttUtils.Messages;
using Newtonsoft.Json;

namespace MqttUtils.Tests
{
    public class TestSimulator
    {
        private readonly IMqttClientFactory _clientFactory;

        public TestSimulator(IMqttClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
            
        }

        public async Task Run(string server, string user, string pass)
        {
            var client = await _clientFactory.GetConnectedMqttClient(server, user, pass, "sim1");
            var powerSimulator = new PowerSimulator(500, 5000, 30, client);
            powerSimulator.Run();
        }

        public class PowerSimulator
        {
            private readonly int _min;
            private readonly int _max;
            private readonly int _stepsPerCycle;
            private readonly IManagedMqttClient _client;
            
            public PowerSimulator(int min, int max, int stepsPerCycle, IManagedMqttClient client)
            {
                _min = min;
                _max = max;
                _stepsPerCycle = stepsPerCycle;
                _client = client;
            }

            public CancellationTokenSource Run()
            {
                // Define the cancellation token.
                var source = new CancellationTokenSource();
                var token = source.Token;
                Task.Run(async () =>
                {
                    
                    int i = 0;
                    double stepSize = ((double) _max - _min) / _stepsPerCycle;

                    while (!token.IsCancellationRequested)
                    {
                        
                        double current = _min + stepSize * (i % _stepsPerCycle);
                        await Task.Delay(TimeSpan.FromMilliseconds(current));
                        var message = new PowerMessage("power", (int) current);
                        await _client.PublishAsync(new MqttApplicationMessageBuilder()
                            .WithTopic("hallondisptest/power")
                            .WithPayload(
                                Encoding.UTF8.GetBytes(
                                    JsonConvert.SerializeObject(message, new PowerMessageConverter())))
                            .Build());
                        i++;
                    }
                    await _client.StopAsync();
                }, token);
                return source;
            }
        }
    }
}