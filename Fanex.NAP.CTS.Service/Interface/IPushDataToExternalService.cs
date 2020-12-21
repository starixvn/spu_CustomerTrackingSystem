namespace Fanex.NAP.CTS.Service
{
    using Fanex.NAP.CTS.Model.CustomerClassify;
    using System.Collections.Generic;

    public interface IPushDataToExternalService
    {
        bool PushCustomerClassificationToMain(IEnumerable<LatestClassificationModel> customerClassifications, int retryTime);

        void PushCustomerClassificationToSoon88(IEnumerable<LatestClassificationModel> customerClassifications);
    }
}
