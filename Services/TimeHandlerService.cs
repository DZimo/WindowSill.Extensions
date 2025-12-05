using System.ComponentModel.Composition;
using System.Timers;
using Windows.UI;
using WindowSill.API;
using WindowSill.PomodoroTimer.Models;
using WindowSill.PomodoroTimer.UI;
using Timer = System.Timers.Timer;

namespace WindowSill.PomodoroTimer.Services
{
    [Export(typeof(ITimeHandlerService))]
    public class TimeHandlerService : ITimeHandlerService, IDisposable
    {
        public event EventHandler<TimeManager?>? TimerFinished;
        public event EventHandler<TimeManager?>? TimerReduced;

        public Timer _timerReducer = new Timer(TimeSpan.FromSeconds(1));
        private bool _isBreakTime = false;

        [ImportingConstructor]
        public TimeHandlerService()
        {
            
        }

        public void StartTimer(TimeManager timeManager, PomodoroType type)
        {
            if (timeManager.MainTimer is not null)
            {
                StartTimers(timeManager);
                return;
            }

            timeManager.MainTimer = new Timer(TimeSpan.FromMinutes(GetTimeFromBreak(_isBreakTime, type)).TotalMilliseconds);
            StartTimers(timeManager);
            timeManager.MainTimer.Elapsed += OnTimerFinished;
            _timerReducer.Elapsed += OnTimerReduced;
        }

        public void ChangeTime(TimeManager timeManager, PomodoroType type)
        {
            if (timeManager.MainTimer is null)
                return;

            _isBreakTime = !_isBreakTime;

            ThreadHelper.RunOnUIThreadAsync(() =>
            {
                if (PomodoroTimerVm.Instance is not null)
                    PomodoroTimerVm.Instance.PomodoroColor = new SolidColorBrush(GetColorFrombreak(_isBreakTime));
            });

            timeManager.MainTimer.Stop();
            _timerReducer.Stop();
            timeManager.MainTimer.Interval = TimeSpan.FromMinutes(GetTimeFromType(type)).TotalMilliseconds;

            StartTimer(timeManager, type);
        }
        private void OnTimerReduced(object? sender, ElapsedEventArgs e)
        {
            TimerReduced?.Invoke(this, null);
        }

        private void OnTimerFinished(object? sender, ElapsedEventArgs e)
        {
            TimerFinished?.Invoke(this, null);
        }

        public void ResetTimer(TimeManager timeManager, PomodoroType type)
        {
            timeManager.MainTimer?.Stop();
        }

        public int GetMinutes(TimeManager? timeManager)
        {
            if (timeManager is null)
                return 0;

            return (timeManager.Seconds / 60);
        }

        public int GetSeconds(TimeManager? timeManager)
        {
            if (timeManager is null)
                return 0;

            return (timeManager.Seconds % 60);
        }

        public int GetTimeFromType(PomodoroType type)
        {
            switch (type) 
            {
                case PomodoroType.Short:
                    return 1;
                case PomodoroType.Long:
                    return 50;
                default:
                    return 25;
            }
        }

        public int GetTimeFromBreak(bool isBreak, PomodoroType type)
        {
            return isBreak ? 2 : GetTimeFromType(type);
        }

        public Color GetColorFrombreak(bool isBreak)
        {
            return isBreak ? Colors.DarkOliveGreen : Colors.IndianRed;
        }

        private void StartTimers(TimeManager timeManager)
        {
            if (timeManager.MainTimer is null)
                return;

            timeManager.MainTimer.Start();
            _timerReducer.Start();
        }

        public void Dispose()
        {
            TimerFinished -= null;
        }
    }
}
