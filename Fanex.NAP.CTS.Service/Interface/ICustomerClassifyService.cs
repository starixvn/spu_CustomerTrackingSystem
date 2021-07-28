namespace Fanex.NAP.CTS.Service
{
    using Fanex.NAP.CTS.Core;
    using Fanex.NAP.CTS.Model.CustomerClassify;
    using Fanex.NAP.CTS.Model.RescanCustClassificationModel;
    using System.Collections.Generic;
    using static Fanex.NAP.CTS.Resource.Consts.Consts;

    public interface ICustomerClassifyService
    {
        void RescanCustomerClassify(RescanCustClassificationModel para);

        IEnumerable<LatestClassificationModel> GetLatestClassification(string custIds);

        void UpdateAppliedCustomerClassification(string jsonData);
    }
}
