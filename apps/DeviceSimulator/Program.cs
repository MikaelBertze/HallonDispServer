using System;
using System.Threading.Tasks;
using DeviceSimulation;
using Microsoft.Extensions.Configuration;
using MqttUtils;

namespace DeviceSimulator
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", true, true)
                .Build();

            var settings = config.Get<Settings>();
            _PrintConfigInfo(settings);
            
            Console.WriteLine("- Starting power simulator ");
            var clientFactory = new MqttClientFactory();

            var sim = new PowerSimulator(settings.Devices.Power.MinTp, settings.Devices.Power.MaxTp, 100,
                await clientFactory.GetConnectedMqttClient(settings.Mqtt.Server, settings.Mqtt.User, settings.Mqtt.Pass,
                    "powersim"), settings.Devices.Power.Topic);
            sim.Run();

            while (Console.ReadKey().Key != ConsoleKey.Q)
            {
                await Task.Delay(10);
            }
        }

        private static void _PrintConfigInfo(Settings settings)
        {
            var mqttSettings = settings.Mqtt;

            var devicesSettings = settings.Devices;
            Console.WriteLine("Device simulator");
            Console.WriteLine("================");
            Console.WriteLine("MQTT:");
            Console.WriteLine($"\tServer: { mqttSettings.Server}");
            Console.WriteLine($"\tUser: { mqttSettings.User}");
            Console.WriteLine($"\tPass: { mqttSettings.Pass}");

            Console.WriteLine();
            Console.WriteLine("Devices");
            Console.WriteLine("================");
            _PrintPowerDeviceInfo(devicesSettings.Power);
        }
        private static void _PrintPowerDeviceInfo(PowerSettings powerSetting)
        {
            Console.WriteLine($"Power");
            Console.WriteLine($"\tID: {powerSetting.Id}");
            Console.WriteLine($"\tmin tick time [ms]: {powerSetting.MinTp}");
            Console.WriteLine($"\tmax tick time [ms]: {powerSetting.MaxTp}");
        }
    }
}