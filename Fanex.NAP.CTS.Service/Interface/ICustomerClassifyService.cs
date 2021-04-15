namespace Fanex.NAP.CTS.Service
{
    using Fanex.NAP.CTS.Model.CustomerClassify;
    using System.Collections.Generic;

    public interface ICustomerClassifyService
    {
        void RescanCustomerClassify(List<int> custIds, bool isCheckPAFromNAP, bool isPushCCToSoon88);

        IEnumerable<LatestClassificationModel> GetLatestClassification(string custIds);

        void UpdateAppliedCustomerClassification(string jsonData);
    }
}
