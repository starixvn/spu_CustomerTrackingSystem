using System;

namespace Fanex.NAP.CTS.Model.API.CustomerClassCategory
{
    public class CustomerClassCategoryByCustIDModel
    {
        public long CustID { get; set; }

        public int CategoryID { get; set; }

        public int CustomerClass { get; set; }

        public DateTime? CreatedDate { get; set; }
    }
}
