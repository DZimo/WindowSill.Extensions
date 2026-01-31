using CommunityToolkit.Mvvm.Messaging.Messages;
using WindowSill.SimpleCalculator.Enums;

namespace WindowSill.SimpleCalculator.Models
{
    public class RequestNumberChanged : RequestMessage<InterVmMessage>
    {
        public InterVmMessage VmMessage;
        public RequestNumberChanged(InterVmMessage vmMessage)
        {
            VmMessage = vmMessage;
        }
    }
}
