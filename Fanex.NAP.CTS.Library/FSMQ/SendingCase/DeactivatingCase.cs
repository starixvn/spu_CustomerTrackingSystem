namespace Fanex.NAP.CTS.Library.FSMQ
{
    public class DeactivatingCase : SendingMessage
    {
        public DeactivatingCase()
        {
            Type = SendingType.Deactivating;
        }

        public DeactivatingCase(IMessageQueueSender messageQueue)
            : base(messageQueue)
        {
            Type = SendingType.Deactivating;
        }
    }
}