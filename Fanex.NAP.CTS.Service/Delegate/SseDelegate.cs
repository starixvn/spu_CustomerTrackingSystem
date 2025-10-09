namespace Fanex.NAP.CTS.Service.Delegate
{
    using System.Threading.Tasks;

    public static class SseDelegate
    {
        public delegate Task PushEventDelegate(object data);
    }
}
