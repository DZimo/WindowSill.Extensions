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
    }

    public enum PomodoroType
    {
        Short,
        Long,
    }
}
