using WindowSill.API;
using WindowSill.OutlookCalendar.Services;

namespace WindowSill.OutlookCalendar.Activators
{
    internal interface ISillTimeActivator : ISillActivator
    {

        ValueTask<bool> GetShouldBeActivatedAsync(IOutlookService outlookService, CancellationToken cancellationToken);
    }
}
