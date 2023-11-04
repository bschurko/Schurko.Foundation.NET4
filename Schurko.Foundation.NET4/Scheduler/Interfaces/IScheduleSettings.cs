using System;

namespace Schurko.Foundation.Scheduler.Interfaces
{
    public interface IScheduleSettings
    {
         TimeSpan MaxDifference { get; }
         TimeSpan MaxTimeout { get; }
         bool HideExceptions { get; }
    }

    public class ScheduleSettings : IScheduleSettings
    {
        public TimeSpan MaxDifference => TimeSpan.FromMilliseconds(500);
        public TimeSpan MaxTimeout => TimeSpan.FromMilliseconds(0);
        public bool HideExceptions => false;

        public ScheduleSettings()
        {
        }
    }
}
