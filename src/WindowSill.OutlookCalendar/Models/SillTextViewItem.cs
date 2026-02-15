using System.Runtime.CompilerServices;
using WindowSill.API;
using WinRT;

namespace WindowSill.OutlookCalendar.Models
{
    [Bindable]
    [CreateNewOnMetadataUpdate]
    [WinRTRuntimeClassName("Microsoft.UI.Xaml.IUIElementOverrides")]
    public class SillTextViewItem : FrameworkElement
    {
        public SillTextViewItem()
        {

        }

        public static explicit operator SillTextViewItem(string text)
        {
            return new SillTextViewItem();
        }

        public static explicit operator SillTextViewItem(SillListViewButtonItem viewButton)
        {
            return new SillTextViewItem();
        }

    }
}
