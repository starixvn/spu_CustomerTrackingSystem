using System;

namespace Fanex.NAP.CTS.ViewModel.Account.CustomerCategory
{
    public class CustomerCategoryViewModel
    {
        public int CategoryId { get; set; }

        public string CategoryName { get; set; }

        public int SportGroupId { get; set; }

        public string SportGroupName { get; set; }

        public string Remark { get; set; }

        public string LastModifiedBy { get; set; }

        public DateTime LastModifiedDate { get; set; }
    }
}
