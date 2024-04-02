using Fanex.NAP.CTS.Core.Extentions;
using static Fanex.NAP.CTS.Resource.Consts.Consts;

namespace Fanex.NAP.CTS.ViewModel.SpecialCustomerClass
{
    public class CustomerClassViewModel
    {
        public long CTSCustID { get; set; }

        public int CustID { get; set; }

        public string Username { get; set; }

        public string SubscriberName { get; set; }

        public int SportID { get; set; }

        public string SportType
        {
            get
            {
                return SportID == 0 ?
                        "General" :
                        ((CustomerCategorySportType)SportID).GetStringSlow() ?? string.Empty;
            }
        }

        public int CustomerClass { get; set; }

        public string Comment { get; set; }

        public string CreatedBy { get; set; }

        public string CreatedDateString { get; set; }
    }
}
