using System;
using System.Collections.Generic;
using System.Threading;

/// <summary>
/// Timer system made by Geoffrey Hendrikx.
/// </summary>
namespace UnityEngine.Timers
{
    public class TimerManager : Singleton<TimerManager>
    {
        private List<Timer> timers = new List<Timer>();
        public List<Timer> Timers
        {
            get
            {
                return timers;
            }
            set
            {
                timers = value;
            }
        }

        /// <summary>
        /// Updating the timers
        /// </summary>
        private void Update()
        {
            if (timers.Count != 0)
                for (int i = 0; i < timers.Count; i++)
                    if (timers[i] != null)
                        timers[i].UpdateTimer();

        }

        /// <summary>
        /// Adding a timer
        /// </summary>
        /// <param name="executeAfterTime">function after executing</param>
        /// <param name="time">Invoke time</param>
        public void AddTimer(Timer.ExecuteAfterTimer executeAfterTime, float time)
        {
            timers.Add(new Timer(executeAfterTime, time));
        }

        public void AddTimer(ref bool toggle, float time)
        {
            //this.toggle.
        }


        /// <summary>
        /// Remove Timer.
        /// </summary>
        public void RemoveTimer(Timer timer)
        {
            timers.Remove(timer);
        }


        /// <summary>
        /// Toggling the timer to stop or to continue.
        /// </summary>
        public void ToggleTimer(int index, bool status)
        {
            timers[index].PauseTimer = status;
        }

        /// <summary>
        /// Toggling all timers to stop or to continue.
        /// </summary>
        public void ToggleAllTimers(bool status)
        {
            for (int i = 0; i < timers.Count; i++)
                timers[i].PauseTimer = status;
        }
    }
}