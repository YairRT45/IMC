using CommunityToolkit.Mvvm.Messaging.Messages;

namespace IMCApp.Utilidades
{
    public class PacienteMensajeria : ValueChangedMessage<PacienteMensaje>
    {
        public PacienteMensajeria(PacienteMensaje value) : base(value)
        {
            
        }
    }
}
