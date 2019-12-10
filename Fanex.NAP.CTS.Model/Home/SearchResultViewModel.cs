using Fanex.NAP.CTS.Model.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fanex.NAP.CTS.Model.Home
{
   public class SearchResultViewModel
    {
        public int CTSCustId { get; set; }

        public string Username { get; set; }

        public string Username2 { get; set; }

        public RoleUser RoldId { get; set; }

        public DTO.Subscriber Subscriber { get; set; }

        public DateTime LastLogin { get; set; }
    }
}
