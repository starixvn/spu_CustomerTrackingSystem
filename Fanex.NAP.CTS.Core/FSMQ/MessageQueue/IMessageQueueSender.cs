namespace Fanex.NAP.CTS.Core.FSMQ
{
    public interface IMessageQueueSender
    {
        void Clear();

        void Send(object task);

        void Send(object task, string label);
    }
}