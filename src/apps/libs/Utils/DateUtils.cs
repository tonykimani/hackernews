namespace libs.Utils
{
    public static class DateUtils
    {
        /// <summary>
        /// Returns midnight of d
        /// </summary>
        /// <param name="d"></param>
        /// <returns></returns>
        public static DateTimeOffset Midnight(this  DateTimeOffset d)
        {
            var tomorrow = d.AddDays(1).ToString("dd-MMM-yyyy");
            var tomorrowDate = DateTimeOffset.ParseExact($"{tomorrow} 00:00", "dd-MMM-yyyy HH:mm", null);
            return tomorrowDate.AddMinutes(-1);
        }
    }
}
