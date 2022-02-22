namespace Fanex.NAP.CTS.Model.AntiFraudCenter
{
    using Fanex.NAP.CTS.Model.API;
    using System.Collections.Generic;
    using static Fanex.NAP.CTS.Resource.Consts.Consts;

    public class B2CAccountResultModel
    {
        public IEnumerable<B2CAccountModel> B2CAccounts { get; set; }

        public HttpStatus StatusCode { get; set; }

        public string ErrorMessage { get; set; }
    }
}