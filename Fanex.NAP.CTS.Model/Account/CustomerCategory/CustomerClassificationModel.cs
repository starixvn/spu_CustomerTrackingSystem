using System;

namespace Fanex.NAP.CTS.Model.Account.CustomerCategory
{
    public class CustomerClassificationModel
    {
        public int CategoryId { get; set; }

        public string CategoryName { get; set; }

        public string SportGroupIdList { get; set; }

        public string SportGroupNameList { get; set; }

        public string Remark { get; set; }

        public string LastModifiedBy { get; set; }

        public DateTime LastModifiedDate { get; set; }

    }
}
