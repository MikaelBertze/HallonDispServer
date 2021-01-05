using HallonDispDtos;
using MQTTnet.Extensions.ManagedClient;

namespace DeviceSimulation
{
    public class PowerSimulator : MqttDeviceSimulator<PowerMessage>
    {
        private int _i;
        private readonly int _min;
        private readonly int _stepsPerCycle;
        private readonly double _stepSize;

        public PowerSimulator(int min, int max, int stepsPerCycle, IManagedMqttClient client, string topic)
            :base(client, topic)
        {
            _min = min;
            _stepsPerCycle = stepsPerCycle;
            _stepSize = ((double) max - _min) / _stepsPerCycle;
        }

        protected override PowerMessage CreateMessage(out int timeUntilNextMessageMs)
        {
            int current = CalculateNextMessageTime(_i);
            _i++;
            var message = new PowerMessage("power", current);
            timeUntilNextMessageMs = CalculateNextMessageTime(_i + 1);
            return message;
        }

        private int CalculateNextMessageTime(int i)
        {
            return (int)(_min + _stepSize * ((i +1) % _stepsPerCycle));
        }

    }
}