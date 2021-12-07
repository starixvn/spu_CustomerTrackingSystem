namespace Fanex.NAP.CTS.Model.API
{
    using System.Collections.Generic;

    public class InsertAccountParasViewModel
    {
        public string Key { get; set; }

        public IEnumerable<GetAccountViewModel> Data { get; set; }
    }
}