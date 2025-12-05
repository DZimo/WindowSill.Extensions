using System.ComponentModel.Composition;
using System.Timers;
using WindowSill.PomodoroTimer.Models;
using Timer = System.Timers.Timer;

namespace WindowSill.PomodoroTimer.Services
{
    [Export(typeof(ITimeHandlerService))]
    public class TimeHandlerService : ITimeHandlerService, IDisposable
    {
        public event EventHandler<TimeManager?>? TimerFinished;
        public event EventHandler<TimeManager?>? TimerReduced;

        public Timer _timerReducer = new Timer(TimeSpan.FromSeconds(1));

        [ImportingConstructor]
        public TimeHandlerService()
        {
            
        }

        public void StartTimer(TimeManager timeManager, PomodoroType type)
        {
            timeManager.MainTimer = new Timer(TimeSpan.FromMinutes(GetTimeFromType(type)).TotalMilliseconds);
            timeManager.MainTimer.Start();
            timeManager.MainTimer.Elapsed += OnTimerFinished;

            _timerReducer.Start();
            _timerReducer.Elapsed += OnTimerReduced;
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
                    return 25;
                case PomodoroType.Long:
                    return 50;
                default:
                    return 25;
            }
        }

        public void Dispose()
        {
            TimerFinished -= null;
        }
    }
}
