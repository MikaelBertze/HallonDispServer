using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MQTTnet;
using MQTTnet.Client.Options;
using MQTTnet.Extensions.ManagedClient;

namespace MqttUtils
{
    public interface IMqttClientFactory
    {
        Task<IManagedMqttClient> GetConnectedMqttClient(string server, string user, string passwd, string clientId = null);
    }

    public class MqttClientFactory: IMqttClientFactory
    {
        private AutoResetEvent _whenConnected = new AutoResetEvent(false);
        private Random _random = new Random();

        public async Task<IManagedMqttClient> GetConnectedMqttClient(string server, string user, string passwd, string clientId = null)
        {
            if (clientId == null)
            {
                clientId = _GenerateId();
            }
            var options = new ManagedMqttClientOptionsBuilder()
                .WithAutoReconnectDelay(TimeSpan.FromSeconds(5))
                .WithClientOptions(new MqttClientOptionsBuilder()
                    .WithClientId(clientId)
                    .WithTcpServer(server)
                    .WithCredentials(user, passwd)
                    .Build())
                .Build();
            var mqttClient = new MqttFactory().CreateManagedMqttClient();
            mqttClient.UseConnectedHandler(e => { _whenConnected.Set(); });
            await mqttClient.StartAsync(options);

            if (!_whenConnected.WaitOne(TimeSpan.FromSeconds(10)))
            {
                throw new TimeoutException("Mqtt client did not connect within expected time frame.");
            }
            return mqttClient;
        }

        private string _GenerateId()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, 4)
                .Select(s => s[_random.Next(s.Length)]).ToArray());
        }
    }
}