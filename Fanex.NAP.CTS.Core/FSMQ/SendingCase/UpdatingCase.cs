namespace Fanex.NAP.CTS.Core.FSMQ
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