using UnityEngine;

namespace Odyssey.MomentumCommon
{
    /// <summary>
    /// Provides formatted time adjusted by an offset (for example a time zone)
    /// </summary>
    public static class TimeUtil
    {
        //
        //
        // Privates
        private static System.DateTimeOffset startDateTimeOffset = System.DateTime.Now;
        private static float startTime = Time.time;

        //
        //
        // Getters
        public static System.DateTimeOffset CurrentTime
        {
            get => startDateTimeOffset.AddSeconds((double)(Time.time - startTime));
        }

        public static string CurrentTimeString
        {
            get => CurrentTime.ToString("HH:mm:ss");
        }

        public static string TimeToNextHourString
        {
            get => "00:" + (60 - CurrentTime.Minute) + ":" + (60 - CurrentTime.Second);
        }

        //
        //
        // EventTime methods

        /// <summary>
        /// Sets current time from  astring (meant for mqtt)
        /// </summary>
        /// <param name="time"> Time </param>
        public static void SetTimeFromString(string time)
        {
            startDateTimeOffset = System.DateTimeOffset.Parse(time);
            startTime = Time.time;
        }
    }
}
