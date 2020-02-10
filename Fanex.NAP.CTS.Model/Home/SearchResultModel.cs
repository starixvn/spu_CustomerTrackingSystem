using Fanex.NAP.CTS.Model.DTO;
using System;

namespace Fanex.NAP.CTS.Model.Home
{
    public class SearchResultModel
    {
        public Int64 CTSCustId { get; set; }

        public int CustId { get; set; }

        public string Username { get; set; }

        public string Username2 { get; set; }

        public Role Role { get; set; }

        public Currency Currency { get; set; }

        public DTO.Subscriber Subscriber { get; set; }

        public DateTime LastLogin { get; set; }
    }
}
