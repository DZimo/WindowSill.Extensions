using Microsoft.Windows.AppNotifications;
using Microsoft.Windows.AppNotifications.Builder;
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

        private int _shortBreakTime = 5;
        private int _longBreakTime = 5;
        private int _shortPomoTime = 25;
        private int _LongPomoTime = 50;

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

            timeManager.MainTimer = new Timer(TimeSpan.FromMinutes(GetTimeFromBreak(timeManager.IsBreakTime, type)).TotalMilliseconds);
            StartTimers(timeManager);
            timeManager.MainTimer.Elapsed += OnTimerFinished;
            _timerReducer.Elapsed += OnTimerReduced;
        }

        public void ChangeTime(TimeManager timeManager, PomodoroType type)
        {
            ShowNotification(timeManager.IsBreakTime ? "/WindowSill.PomodoroTimer/Misc/BreakEnd".GetLocalizedString() : "/WindowSill.PomodoroTimer/Misc/BreakTime".GetLocalizedString(),
                timeManager.IsBreakTime ? "/WindowSill.PomodoroTimer/Misc/BreakEndDesc".GetLocalizedString() : "/WindowSill.PomodoroTimer/Misc/BreakTimeDesc".GetLocalizedString());

            if (timeManager.MainTimer is null)
                return;

            timeManager.IsBreakTime = !timeManager.IsBreakTime;

            ThreadHelper.RunOnUIThreadAsync(() =>
            {
                if (PomodoroTimerVm.Instance is not null)
                    PomodoroTimerVm.Instance.PomodoroColor = new SolidColorBrush(GetColorFrombreak(timeManager.IsBreakTime));
            });

            timeManager.MainTimer.Stop();
            _timerReducer.Stop();
            timeManager.MainTimer.Interval = TimeSpan.FromMinutes(GetTimeFromBreak(timeManager.IsBreakTime, type)).TotalMilliseconds;

            StartTimer(timeManager, type);
        }

        public void ResetTimer(TimeManager timeManager, PomodoroType type)
        {
            timeManager.MainTimer?.Stop();
            _timerReducer.Stop();

            if (timeManager.MainTimer is not null)
                timeManager.MainTimer.Elapsed -= OnTimerFinished;
            _timerReducer.Elapsed -= OnTimerReduced;

            timeManager.MainTimer = null;
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
                    return _shortPomoTime;
                case PomodoroType.Long:
                    return _LongPomoTime;
                default:
                    return _shortPomoTime;
            }
        }

        public int GetTimeFromBreak(bool isBreak, PomodoroType type)
        {
            return isBreak ? type is PomodoroType.Short ? _shortBreakTime : _longBreakTime : GetTimeFromType(type);
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

        private void OnTimerReduced(object? sender, ElapsedEventArgs e)
        {
            TimerReduced?.Invoke(this, null);
        }

        private void OnTimerFinished(object? sender, ElapsedEventArgs e)
        {
            TimerFinished?.Invoke(this, null);
        }

        public void Dispose()
        {
            _timerReducer.Dispose();
            TimerFinished -= null;
        }

        public void ShowNotification(string title, string message)
        {
            var notification = new AppNotificationBuilder()
                .AddText(title)
                .AddText(message);

            AppNotificationManager.Default.Show(notification.BuildNotification());
        }
    }
}
