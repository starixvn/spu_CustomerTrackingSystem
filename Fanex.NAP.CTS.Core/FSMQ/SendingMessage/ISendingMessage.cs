namespace Fanex.NAP.CTS.Core.FSMQ
{
    public interface ISendingMessage
    {
        SendingType Type { get; set; }

        bool Send(object message);
    }
}