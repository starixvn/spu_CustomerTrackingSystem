namespace Fanex.NAP.CTS.Library.FSMQ
{
    public class UpdatingCase : SendingMessage
    {
        public UpdatingCase()
        {
            Type = SendingType.Updating;
        }

        public UpdatingCase(IMessageQueueSender messageQueue)
            : base(messageQueue)
        {
            Type = SendingType.Updating;
        }
    }
}