using WindowSill.PomodoroTimer.Models;

namespace WindowSill.PomodoroTimer.Services
{
    public interface ITimeHandlerService
    {
        public event EventHandler<TimeManager?> TimerFinished;

        public event EventHandler<TimeManager?>? TimerReduced;

        public void StartTimer(TimeManager timeManager, PomodoroType type);

        public void ResetTimer(TimeManager timeManager, PomodoroType type);

        public int GetMinutes(TimeManager timeManager);

        public int GetSeconds(TimeManager timeManager);
    }
}
