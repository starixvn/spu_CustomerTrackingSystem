namespace Fanex.NAP.CTS.Model.CustomerClassification
{
    using System.Collections.Generic;

    public class RescanResultModel
    {
        public RescanResultModel()
        {
            CustIds = new List<long>();
            IsSuccess = true;
        }

        public List<long> CustIds { get; set; }

        public bool IsSuccess { get; set; }
    }
}