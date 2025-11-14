namespace Fanex.NAP.CTS.Model.CustomerClassificationBySport.RescanCustomerClassificationBySport
{
    using System.Collections.Generic;
    using System.Linq;

    public class RescanBySportResultModel
    {
        public RescanBySportResultModel()
        {
            CustInfos = Enumerable.Empty<BaseCustomerInfoBySportModel>();
            IsSuccess = true;
        }

        public IEnumerable<BaseCustomerInfoBySportModel> CustInfos { get; set; }

        public bool IsSuccess { get; set; }
    }
}
