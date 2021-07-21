namespace Fanex.NAP.CTS.Service
{
    using Fanex.NAP.CTS.Model.CustomerClassify;
    using System.Collections.Generic;
    using static Fanex.NAP.CTS.Resource.Consts.Consts;

    public interface ICustomerClassifyService
    {
        void RescanCustomerClassify(List<int> custIds, bool isCheckPAFromNAP, bool isPushCCToSoon88, FunctionId funcId);

        IEnumerable<LatestClassificationModel> GetLatestClassification(string custIds);

        void UpdateAppliedCustomerClassification(string jsonData);
    }
}
