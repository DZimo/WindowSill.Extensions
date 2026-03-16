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

        public override int GetHashCode()
        {
            return H.GetHashCode() * Random.Shared.Next() + S.GetHashCode() + V.GetHashCode();
        }

        public override bool Equals(object? obj)
        {
            if (obj is not CombinedColor other)
                return false;

            return this.H == other.H && this.S == other.S && this.V == other.V;
        }
    }
}
