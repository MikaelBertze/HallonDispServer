
namespace HallonDispDtos
{
    public class SwitchMessage
    {
        public SwitchMessage(string id, bool state)
        {
            Id = id;
            State = state;
        }
        public string Id { get; }
        public bool State { get; }
    }

    public class PowerMessage
    {
        public PowerMessage(string id, int powerTickPeriodMs)
        {
            Id = id;
            PowerTickPeriodMs = powerTickPeriodMs;
        }
        public string Id { get; }
        public int PowerTickPeriodMs { get; }
    }

    public class WaterMessage
    {
        public WaterMessage(string id, int angle, int diff, int timeDiffMs, double consumption)
        {
            Id = id;
            Angle = angle;
            Diff = diff;
            TimeDiffMs = timeDiffMs;
            Consumption = consumption;
        }

        public string Id { get; }
        public int Angle { get; }
        public int Diff { get;  }
        public int TimeDiffMs { get; }
        public double Consumption { get; }
    }
}
