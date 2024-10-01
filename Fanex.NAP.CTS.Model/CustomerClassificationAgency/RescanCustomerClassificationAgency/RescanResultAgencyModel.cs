namespace Fanex.NAP.CTS.Model.CustomerClassificationAgency.RescanCustomerClassificationAgency
{
    using System.Collections.Generic;

    public class RescanResultAgencyModel
    {
        public RescanResultAgencyModel()
        {
            CustIds = new List<long>();
            IsSuccess = true;
        }

        public List<long> CustIds { get; set; }

        public bool IsSuccess { get; set; }
    }
}
