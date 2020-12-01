using System;
using System.Threading.Tasks;
using MqttUtils;

namespace DbLogger
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var subscriber = new MqttSubscriber();
            subscriber.WhenMessageRecieved.Subscribe(x => {
                //Console.WriteLine($"{x.topic,-30}-> {x.message}");
                Console.WriteLine($"{x.topic,-30}");
            });
            
            var powerMessageHandler = new PowerMessageHandler("/hallondisp/power", subscriber);
            var waterMessageHandler = new WaterMessageHandler("/hallondisp/water", subscriber);
            await subscriber.Init();
            Console.ReadKey();
        }
    }
}
