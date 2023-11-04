
using System;

namespace Schurko.Foundation.Extensions
{
    public struct WeekNum : IComparable<WeekNum>
    {
        public int Week { get; set; }

        public int Year { get; set; }

        public WeekNum(int week, int year)
          : this()
        {
            Week = week;
            Year = year;
        }

        public int CompareTo(WeekNum other)
        {
            if (other.Week == Week && other.Year == Year)
                return 0;
            return other.Year == Year ? other.Week >= Week ? 1 : -1 : other.Year >= Year ? 1 : -1;
        }
    }
}
