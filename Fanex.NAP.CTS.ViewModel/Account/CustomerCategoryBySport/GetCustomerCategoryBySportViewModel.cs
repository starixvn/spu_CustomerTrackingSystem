namespace Fanex.NAP.CTS.ViewModel.Account.CustomerCategoryBySport
{
    using Fanex.NAP.CTS.Model.Account.CustomerCategoryBySport;

    public class GetCustomerCategoryBySportViewModel
    {
        public GetCustomerCategoryBySportViewModel(GetCustomerCategoryBySportModel data)
        {
            this.SportID = data.SportID;
            this.CategoryName = data.CategoryName;
            this.CustomerClass = data.CustomerClass;

            switch (data.SportID)
            {
                case 1:
                    this.SportType = "Soccer"; break;
                case 2:
                    this.SportType = "Basketball"; break;
                case 43:
                    this.SportType = "E-Sports"; break;
                case 99:
                    this.SportType = "Other Sports"; break;
                default: break;
            }
        }

        public int SportID { get; set; }

        public string SportType { get; set; }

        public string CategoryName { get; set; }

        public int CustomerClass { get; set; }
    }
}
