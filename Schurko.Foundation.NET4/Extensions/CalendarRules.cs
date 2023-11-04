
using System;
using System.Globalization;

namespace Schurko.Foundation.Extensions
{
    internal struct CalendarRules
    {
        public DayOfWeek FirstDayOfFirstWeek { get; set; }

        public CalendarWeekRule WeekRule { get; set; }
    }
}
