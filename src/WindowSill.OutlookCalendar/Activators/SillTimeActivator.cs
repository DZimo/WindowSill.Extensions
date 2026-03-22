using System.ComponentModel.Composition;
using WindowSill.API;
using WindowSill.OutlookCalendar.Services;

namespace WindowSill.OutlookCalendar.Activators
{
    [Export(typeof(ISillTimeActivator))]
    [ActivationType(ActivatorName, baseName: null)]
    internal class SillTimeActivator : ISillTimeActivator
    {
        internal const string ActivatorName = "OutlookCalendar";

        ValueTask<bool> ISillTimeActivator.GetShouldBeActivatedAsync(IOutlookService outlookService, CancellationToken cancellationToken)
        {
            return new ValueTask<bool>(Task.FromResult(outlookService.FirstAppointment()?.Start - DateTime.Now > TimeSpan.FromMinutes(30)));
        }
    }
}
