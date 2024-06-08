
using CommunityToolkit.Mvvm.Messaging.Messages;

namespace AgroGestor360.App.Tools.Messages;

public class CancelDialogForPgSalesRequestMessage : ValueChangedMessage<bool>
{
    public CancelDialogForPgSalesRequestMessage(bool cancel) : base(cancel)
    {
    }
}
