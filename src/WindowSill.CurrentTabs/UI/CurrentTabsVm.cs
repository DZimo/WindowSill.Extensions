using CommunityToolkit.Diagnostics;
using CommunityToolkit.Mvvm.ComponentModel;
using WindowSill.API;

namespace WindowSill.CurrentTabs.UI;

public partial class CurrentTabsVm : ObservableObject
{
    private readonly ISettingsProvider _settingsProvider;
    private readonly IProcessInteractionService _processInteraction;

    public static CurrentTabsVm? Instance;

    public CurrentTabsVm(ISettingsProvider settingsProvider, IProcessInteractionService processInteraction)
    {
        Guard.IsNotNull(settingsProvider, nameof(settingsProvider));
        Guard.IsNotNull(processInteraction, nameof(processInteraction));

        _settingsProvider = settingsProvider;
        _processInteraction = processInteraction;
 
         Instance = this;
    }

    public SillView CreateView()
    {
        return new SillView { Content = new CurrentTabsView(this) };
    }
}