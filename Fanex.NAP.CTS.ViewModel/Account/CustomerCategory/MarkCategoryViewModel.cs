using Fanex.NAP.CTS.Model.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fanex.NAP.CTS.ViewModel.Account.CustomerCategory
{
    public class MarkCategoryViewModel
    {
        public List<SportGroup> SportGroupList { get; set; }

        public List<Model.DTO.CustomerCategory> CustomerCategoryList { get; set; }

        public string CategoryName { get; set; }

        public int CategoryId { get; set; }

        public string SportGroupIdList { get; set; }

        public long CTSCustId { get; set; }

        public int SubscriberId { get; set; }

        public string Comment { get; set; }
    }
}
