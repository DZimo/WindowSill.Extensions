using WindowSill.PomodoroTimer.Models;

namespace WindowSill.PomodoroTimer.Services
{
    public interface ITimeHandlerService
    {
        public event EventHandler<TimeManager?> TimerFinished;

        public event EventHandler<TimeManager?>? TimerReduced;

        public int _shortBreakTime { get; }
        public int _longBreakTime { get; }

        public void StartTimer(TimeManager timeManager, PomodoroType type);

        public void ResetTimer(TimeManager timeManager, PomodoroType type);

        public void ChangeTime(TimeManager timeManager, PomodoroType type);

        public int GetMinutes(TimeManager timeManager);

        public int GetSeconds(TimeManager timeManager);

        public int GetTimeFromType(PomodoroType type);

        public int GetTimeFromBreak(bool isBreak, PomodoroType type);
    }
}
