namespace Fanex.NAP.CTS.ViewModel.API.TWSpecialLicSub
{
    using System;

    public class ForceTWLicSubCCViewModel
    {
        public DateTime responseDateTime { get; set; }

        public int httpStatusCode { get; set; } = (int)System.Net.HttpStatusCode.OK;
    }
}
