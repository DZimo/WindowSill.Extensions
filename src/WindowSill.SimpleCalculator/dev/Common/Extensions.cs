using WindowSill.API;

namespace WindowSill.SimpleCalculator.Common
{
    public static class Extensions
    {
        public static void UpdateTextBox(this TextBox o)
        {
            ThreadHelper.RunOnUIThreadAsync(() =>
            {
                o.Focus(FocusState.Programmatic);
                o.SelectionStart = o.Text?.Length ?? 0;
                o.SelectionLength = 0;
            });
        }
    }
}
