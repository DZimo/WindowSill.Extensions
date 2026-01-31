using CommunityToolkit.Mvvm.ComponentModel;

namespace WindowSill.ColorPicker.Model
{
    public partial class CombinedColor : ObservableObject
    {
        [ObservableProperty]
        public double h;

        [ObservableProperty]
        public double s;

        [ObservableProperty]
        public double v;

        public double A;

        [ObservableProperty]
        public double hL;

        [ObservableProperty]
        public double sL;

        [ObservableProperty]
        public double l;

        public double AL;
    }
}
