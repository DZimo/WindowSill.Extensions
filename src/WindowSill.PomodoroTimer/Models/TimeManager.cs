using CommunityToolkit.Mvvm.ComponentModel;
using Timer = System.Timers.Timer;

namespace WindowSill.PomodoroTimer.Models
{
    public partial class TimeManager : ObservableObject
    {
        public Timer? MainTimer { get; set; }

        [ObservableProperty]
        private int minutes;

        [ObservableProperty]
        private int seconds;

        [ObservableProperty]
        private bool isBreakTime = false;
    }

    public enum PomodoroType
    {
        Short,
        Long,
    }
    public enum TimeDisplayMode
    {
        TimeLeft,
        TimeSpent
    }
}
