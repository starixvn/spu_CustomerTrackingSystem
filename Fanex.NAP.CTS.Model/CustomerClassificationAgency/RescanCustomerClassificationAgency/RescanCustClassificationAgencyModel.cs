namespace Fanex.NAP.CTS.Model.CustomerClassificationAgency.RescanCustomerClassificationAgency
{
    using System.Collections.Generic;
    using static Fanex.NAP.CTS.Resource.Consts.Consts;

    public class RescanCustClassificationAgencyModel
    {
        public RescanCustClassificationAgencyModel()
        {
            IsUpdateDangerToMainDB = false;
            IsToQueue = false;
        }

        public IEnumerable<long> CustIds { get; set; }

        public bool IsUpdateDangerToMainDB { get; set; }


        public bool IsToQueue { get; set; }

        public FunctionId FunctionId { get; set; }

        public string ServiceName { get; set; }
    }
}
