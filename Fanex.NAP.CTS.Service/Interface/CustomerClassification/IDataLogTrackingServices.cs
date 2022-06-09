namespace Fanex.NAP.CTS.Service
{
    using System;

    public interface IDataLogTrackingServices
    {
        void LogErrorData(Exception exception, string serviceName);
    }
}
