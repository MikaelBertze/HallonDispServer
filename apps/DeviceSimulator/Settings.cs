namespace DeviceSimulator
{

    public class Settings
    {
        public MqttSettings Mqtt { get; set; }
        public Devices Devices { get; set; }
    }
    
    public class MqttSettings
    {
        public string Server { get; set; }
        public string User { get; set; }
        public string Pass { get; set; }
    }

    public class Devices
    {
        public PowerSettings Power { get; set; }
    }

    public abstract class DeviceSettings
    {
        public string Id { get; set; }
    }
    public class PowerSettings : DeviceSettings
    {
        public int MinTp { get; set; }
        public int MaxTp { get; set; }
        public string Topic { get; set; }
    }

    public class WaterSettings
    {
        
    }

    public class SwitchSettings
    {
        
    }

    public class TempSettings
    {
        
    }
}