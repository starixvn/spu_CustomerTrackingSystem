namespace Fanex.NAP.CTS.SampleData
{
    using Fanex.NAP.CTS.ViewModel.Account;    

    public static class ExpectedData
    {        
        private static AccountDetail expectedAccountDetail;        
        private static CustomerDetailsInfo expectedCustomerDetailsInfo;

        public static AccountDetail ExpectedAccountDetail
        {
            get
            {
                expectedAccountDetail = expectedAccountDetail ?? GenerateSampleAccountDetail();

                return expectedAccountDetail;
            }
        }
   
        private static AccountDetail GenerateSampleAccountDetail()
            => new AccountDetail { };
        
    }
}