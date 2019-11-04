namespace Fanex.NAP.CTS.Library.FSMQ
{
    public interface ISendingMessage
    {
        SendingType Type { get; set; }

        bool Send(object message);
    }
}