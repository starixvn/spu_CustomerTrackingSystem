namespace Fanex.NAP.CTS.Library.FSMQ
{
    public class RemovingCase : SendingMessage
    {
        public RemovingCase()
        {
            Type = SendingType.Removing;
        }

        public RemovingCase(IMessageQueueSender messageQueue)
            : base(messageQueue)
        {
            Type = SendingType.Removing;
        }
    }
}