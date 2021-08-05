namespace Fanex.NAP.CTS.Service
{
    using Fanex.NAP.CTS.Model.LogInputData;
    using static Fanex.NAP.CTS.Resource.Consts.Consts;

    public interface ILogInputDataService
    {
        void LogInputData(LogInputViewModel content, FunctionId funcId);
    }
}
