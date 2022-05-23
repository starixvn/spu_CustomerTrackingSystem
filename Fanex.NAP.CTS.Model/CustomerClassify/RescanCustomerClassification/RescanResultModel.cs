namespace Fanex.NAP.CTS.Model.CustomerClassify
{
    using System;
    using System.Collections.Generic;

    public class RescanResultModel
    {
        public List<long> CustIds { get; set; }

        public bool IsSuccess { get; set; }
    }
}