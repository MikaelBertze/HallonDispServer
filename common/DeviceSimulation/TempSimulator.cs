using HallonDispDtos;
using MQTTnet.Extensions.ManagedClient;

namespace DeviceSimulation
{
    public class TempSimulator : MqttDeviceSimulator<TempMessage>
    {
        public TempSimulator(IManagedMqttClient client, string topic) : base(client, topic)
        {
        }

        protected override TempMessage CreateMessage(out int timeUntilNextMessageMs)
        {
            throw new System.NotImplementedException();
        }
    }
}