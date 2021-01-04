using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MqttUtils.Messages
{
    public class PowerMessageConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var message = (PowerMessage) value;
            var jObj = new JObject {{"id", message.Id}, {"power_tick_period", message.PowerTickPeriodMs}};
            jObj.WriteTo(writer);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            // Load the JSON for the Result into a JObject
            JObject jo = JObject.Load(reader);
            if (!jo.TryGetValue("id", out var id) || !jo.TryGetValue("power_tick_period", out var tickPeriod))
                return null;
            return new PowerMessage(id.Value<string>(), tickPeriod.Value<int>());
        }

        public override bool CanConvert(Type objectType)
        {
            return (objectType == typeof(PowerMessage));
        }
    }
    
    public class WaterMessageConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            // Load the JSON for the Result into a JObject
            JObject jo = JObject.Load(reader);
            if (!jo.TryGetValue("id", out var id) 
                || !jo.TryGetValue("angle", out var angle)
                || !jo.TryGetValue("diff", out var diff)
                || !jo.TryGetValue("t_diff", out var timeDiff)
                || !jo.TryGetValue("consumption", out var consumption))
                return null;
            return new WaterMessage(id.Value<string>(), angle.Value<int>(), diff.Value<int>(), timeDiff.Value<int>(), consumption.Value<double>());
        }

        public override bool CanConvert(Type objectType)
        {
            return (objectType == typeof(WaterMessage));
        }
    }
    
}