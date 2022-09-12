

namespace SmartCityWorkService.Extensions
{
    public static class DateTimeExtension
    {

        public static string ToWeekName(this DateTime dt) 
        {
            var week = dt.DayOfWeek;
            switch ((int)week)
            {
                case 0:
                    return "星期日";
                case 1:
                    return "星期一";
                case 2:
                    return "星期二";
                case 3:
                    return "星期三";
                case 4:
                    return "星期四";
                case 5:
                    return "星期五";
                case 6:
                    return "星期六";
            }
            return string.Empty;
        }


        public static decimal ToReservationMoney(this DateTime dt) 
        {
            string week = dt.ToWeekName();
            if (week == "星期六" || week == "星期日")
            {
                if (TimeOnly.Parse(dt.ToString("HH:mm:ss")) >= TimeOnly.Parse("08:00:00")) 
                {
                    return 60;
                }
                else
                {
                    return 40;
                }
            }
            else 
            {
                if (TimeOnly.Parse(dt.ToString("HH:mm:ss")) >= TimeOnly.Parse("17:00:00"))
                {
                    return 60;
                }
                else
                {
                    return 40;
                }
            }
        
        
        }


    }
}
