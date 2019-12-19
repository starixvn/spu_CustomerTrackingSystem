using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fanex.NAP.CTS.Model.Account
{
   public class CustomerStaticStatusModel
    {
        public int ListId { get; set; }

        public int ItemId { get; set; }

        public string ListName { get; set; }

        public string ListNameDisplay { get; set; }

        public string ItemName { get; set; }

        public string ItemNameDisplay { get; set; }

        public int PriorityOrder { get; set; }

        public bool Status { get; set; }

        public string Description { get; set; }

        public DateTime CreatedDate { get; set; }
    }
}
