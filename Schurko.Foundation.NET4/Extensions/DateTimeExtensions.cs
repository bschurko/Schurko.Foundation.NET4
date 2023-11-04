using System;
using System.Globalization;


#nullable enable
namespace Schurko.Foundation.Extensions
{
    public static class DateTimeExtensions
    {
        public static WeekNum WeekOfYear(this DateTime date) => CalculateWeekOfYear(date, null);

        public static WeekNum WeekOfYear(this DateTime date, CultureInfo culture) => CalculateWeekOfYear(date, culture);

        public static DayOfWeek FirstWeekOfYearStartsWith(this DateTime date, CultureInfo culture) => GetLocalisedCalendarRules(culture).FirstDayOfFirstWeek;

        public static bool Between(this DateTime date, DateTime startDate, DateTime endDate) => date >= startDate && date <= endDate;

        public static int WeeksInYear(this DateTime date) => CalculateWeekOfYear(date, null).Week;

        public static int WeeksInYear(this DateTime date, CultureInfo culture) => CalculateWeekOfYear(date, culture).Week;

        public static bool IsLeapYear(this DateTime date) => GetStandardCalendar().IsLeapYear(date.Year);

        private static WeekNum CalculateWeekOfYear(DateTime date, CultureInfo culture)
        {
            Calendar standardCalendar = GetStandardCalendar();
            CalendarRules localisedCalendarRules = GetLocalisedCalendarRules(culture);
            WeekNum weekOfYear = new WeekNum(0, date.Year)
            {
                Week = standardCalendar.GetWeekOfYear(date, localisedCalendarRules.WeekRule, localisedCalendarRules.FirstDayOfFirstWeek)
            };
            if (weekOfYear.Week >= 52 && date.DayOfYear < 7)
                --weekOfYear.Year;
            return weekOfYear;
        }

        private static Calendar GetStandardCalendar() => new GregorianCalendar()
        {
            CalendarType = GregorianCalendarTypes.USEnglish
        };

        private static CalendarRules GetLocalisedCalendarRules(CultureInfo culture)
        {
            CalendarRules localisedCalendarRules = new CalendarRules();
            culture = culture ?? CultureInfo.CurrentCulture;
            switch (culture.LCID)
            {
                case 1033:
                    localisedCalendarRules.WeekRule = CalendarWeekRule.FirstFullWeek;
                    localisedCalendarRules.FirstDayOfFirstWeek = DayOfWeek.Saturday;
                    break;
                case 2057:
                    localisedCalendarRules.WeekRule = CalendarWeekRule.FirstDay;
                    localisedCalendarRules.FirstDayOfFirstWeek = DayOfWeek.Sunday;
                    break;
                default:
                    localisedCalendarRules.WeekRule = CalendarWeekRule.FirstFullWeek;
                    localisedCalendarRules.FirstDayOfFirstWeek = DayOfWeek.Saturday;
                    break;
            }
            return localisedCalendarRules;
        }
    }
}
