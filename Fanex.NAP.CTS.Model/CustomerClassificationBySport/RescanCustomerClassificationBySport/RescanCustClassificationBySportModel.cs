namespace Fanex.NAP.CTS.Model.CustomerClassificationBySport.RescanCustomerClassificationBySport
{
    using System.Collections.Generic;
    using static Fanex.NAP.CTS.Resource.Consts.Consts;

    public class RescanCustClassificationBySportModel
    {
        public IEnumerable<RescanCustomerClassificationBySportRequestModel> CustInfos { get; set; }

        public BySport_FunctionId FunctionId { get; set; }

        public int InternalChunkSize { get; set; }

        public int PushCCChunkSize { get; set; }

        public int PushCCRetryTime { get; set; }
    }
}
