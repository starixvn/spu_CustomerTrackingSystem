namespace Fanex.NAP.CTS.Model.SpecialCustomerClass
{
    public class AddGenreralUserSpecialCCModel
    {
        public long CTSCustID { get; set; }

        public long CustID { get; set; }
        
        public int SubscriberID { get; set; }

        public bool IsLicenseeVIP { get; set; }
       
        public int CustomerClass { get; set; }

        public int CreatedBy { get; set; }

        public string Remark { get; set; }
               
    }

    public class AddSportUserSpecialCCModel : AddGenreralUserSpecialCCModel
    {
        public int SportID { get; set; }
    }
}
