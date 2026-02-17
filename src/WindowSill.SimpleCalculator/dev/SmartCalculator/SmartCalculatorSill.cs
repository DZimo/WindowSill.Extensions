using Microsoft.UI.Xaml.Media.Imaging;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using WindowSill.API;

namespace WindowSill.SimpleCalculator.SmartCalculator;

[Export(typeof(ISill))]
[Name("WindowSill.SmartCalculator")]
[Priority(Priority.Lowest)]
public sealed class SmmartCalculatorSill : ISillActivatedByTextSelection, ISillListView
{
    [Import]
    private IPluginInfo _pluginInfo = null!;

    //private readonly SmartCalculatorVm _viewModel = new();
    public string DisplayName => "/WindowSill.SimpleCalculator/Misc/DisplayName".GetLocalizedString();

    public IconElement CreateIcon()
        => new ImageIcon
        {
            Source = new SvgImageSource(new Uri(System.IO.Path.Combine(_pluginInfo.GetPluginContentDirectory(), "Assets", "calculator_logo.svg")))
        };

    public SillSettingsView[]? SettingsViews => null;

    public ObservableCollection<SillListViewItem> ViewList { get; } = new();

    public SillView? PlaceholderView => null;

    public string[] TextSelectionActivatorTypeNames { get; } = [SmartCalculatorActivator.ActivatorName];

    public async ValueTask OnActivatedAsync(string textSelectionActivatorTypeName, WindowTextSelection currentSelection)
    {
        await ThreadHelper.RunOnUIThreadAsync(() =>
        {
            ViewList.Clear();

            if (textSelectionActivatorTypeName == SmartCalculatorActivator.ActivatorName)
            {
                var view = SmartCalculatorActivator.SmartCalculatorVm.CreateView();
                var viewitem = new SillListViewButtonItem(view, null, DoNothing);
                ViewList.Add(viewitem);
            }
        });
    }

    private async Task DoNothing()
    {
        throw new NotImplementedException();
    }
    public async ValueTask OnDeactivatedAsync()
    {
        await ThreadHelper.RunOnUIThreadAsync(ViewList.Clear);
    }
}