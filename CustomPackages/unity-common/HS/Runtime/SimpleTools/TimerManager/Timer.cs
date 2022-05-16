using System.Collections;
using System.Runtime.CompilerServices;
/// <summary>
/// Timer system made by Geoffrey Hendrikx.
/// </summary>
namespace UnityEngine.Timers
{
    /// <summary>
    /// This class should only be created by the class TimerManager.
    /// Created by Geoffrey Hendrikx
    /// </summary>
    public class Timer
    {
        public delegate void ExecuteAfterTimer();
        public event ExecuteAfterTimer executeAfterTimerRunsOut;

        public readonly string methodeInfo;

        private float settedTime;
        public float SettedTime
        {
            get
            {
                return settedTime;
            }
            set
            {
                settedTime = value;
            }
        }
        private bool toggle;

        private bool? toggleTimer;
        private bool stopTimer;
        public bool PauseTimer
        {
            get
            {
                return stopTimer;
            }
            set
            {
                stopTimer = value;
            }
        }

        /// <summary>
        /// Should only be made by the TimerManager
        /// </summary>
        /// <param name="execute"></param>
        /// <param name="settedTime"></param>
        public Timer(ExecuteAfterTimer execute, float settedTime)
        {
            this.executeAfterTimerRunsOut = execute;
            this.settedTime = settedTime;
            methodeInfo = execute.Method.ToString();
        }

        /// <summary>
        /// Update the timer.
        /// </summary>
        public void UpdateTimer()
        {
            if (!stopTimer)
                settedTime -= Time.deltaTime;

            if (settedTime <= 0)
                EndTimer();
        }

        /// <summary>
        /// Executing the delegate.
        /// </summary>
        public void EndTimer()
        {
            executeAfterTimerRunsOut?.Invoke();
            if (toggleTimer != null)
                TimerToggle();
            RemoveTimer();
        }

        public void RemoveTimer()
        {
            TimerManager.Instance.RemoveTimer(this);
        }


        private void TimerToggle()
        {
            toggleTimer = !toggleTimer;
        }

    }
}