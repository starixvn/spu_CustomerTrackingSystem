namespace Fanex.NAP.CTS.Model.API
{
    using System.Collections.Generic;

    public class DeleteAccountParasViewModel
    {
        public string Key { get; set; }

        public IEnumerable<GetB2CustIDModel> Data { get; set; }
    }
}