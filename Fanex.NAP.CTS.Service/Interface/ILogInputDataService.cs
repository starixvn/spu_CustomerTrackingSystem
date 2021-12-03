namespace Fanex.NAP.CTS.Service
{
    using Fanex.NAP.CTS.Model.CustomerClassify;
    using Fanex.NAP.CTS.Model.LogInputData;
    using System.Collections.Generic;
    using static Fanex.NAP.CTS.Resource.Consts.Consts;

    public interface ILogInputDataService
    {
        void LogInputData(LogInputViewModel content, FunctionId funcId);

        void LogInputData(IEnumerable<LogInputDataModel> data, FunctionId funcId);
    }
}
