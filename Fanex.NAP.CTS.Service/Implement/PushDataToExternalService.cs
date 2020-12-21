namespace Fanex.NAP.CTS.Service
{
    using System;
    using System.Collections.Generic;
    using Fanex.NAP.CTS.Model.CustomerClassify;

    public class PushDataToExternalService : IPushDataToExternalService
    {
        public bool PushCustomerClassificationToMain(IEnumerable<LatestClassificationModel> customerClassifications, int retryTime)
        {
            throw new NotImplementedException();
        }

        public void PushCustomerClassificationToSoon88(IEnumerable<LatestClassificationModel> customerClassifications)
        {
            throw new NotImplementedException();
        }
    }
}
