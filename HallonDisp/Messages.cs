using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace HallonDisp
{

    public class DoorMessage
    {
        [JsonPropertyName("id")]        
        public string Id { get; set; }
        public bool Door { get; set; }
    }

    public class PowerMessage
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }
        [JsonPropertyName("power_tick_period")]        
        public int PowerTickPeriod { get; set; }
    }

    public class WaterMessage
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }
        
        [JsonPropertyName("angle")]
        public string Angle { get; set; }
        [JsonPropertyName("diff")]
        public string Diff { get; set; }
        [JsonPropertyName("t_diff")]
        public string T_Diff { get; set; }
        [JsonPropertyName("consumption")]
        public string Consumption { get; set; }
    }
}
