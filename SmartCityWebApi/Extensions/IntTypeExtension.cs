namespace SmartCityWebApi.Extensions
{
    public static class IntTypeExtension
    {

        public static string ToSpaceTypeName(this int spaceType) 
        {
            switch (spaceType)
            {
                case 1:
                    return "网球场";
                case 2:
                    return "篮球场";
                case 3:
                    return "羽毛球场";
                case 4:
                    return "排球场";
                case 5:
                    return "乒乓球场";
                default:
                    return string.Empty;
            }

        }

        public static string ToOrderStatusName(this int spaceType)
        {
            switch (spaceType)
            {
                case 0:
                    return "预订待支付";
                case 1:
                    return "已预订";
                case 2:
                    return "已退款";
                case 3:
                    return "退款待确认";
                case 4:
                    return "拒绝退款";
                default:
                    return string.Empty;
            }

        }
    }
}
