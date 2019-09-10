using System;

namespace pmdbs
{
    class TimeConverter
    {
        /// <summary>
        /// Convert a unix timestamp to a DateTime object.
        /// </summary>
        /// <param name="unixTimeStamp">The unix time stamp to convert.</param>
        /// <returns>The DateTime represented by the timestamp.</returns>
        public static DateTime UnixTimeStampToDateTime(double unixTimeStamp)
        {
            // Unix timestamp is seconds past epoch
            DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddSeconds(unixTimeStamp).ToLocalTime();
            return dtDateTime;
        }

        /// <summary>
        /// Creates a timestamp representing the unix time.
        /// </summary>
        /// <returns>Timestamp representing the unix time</returns>
        public static string TimeStamp()
        {
            return DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString();
        }
    }
}
