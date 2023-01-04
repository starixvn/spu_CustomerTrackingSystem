namespace Fanex.NAP.CTS.ViewModel.Account.CustomerCategoryBySport
{
    using Fanex.NAP.CTS.Model.Account.CustomerCategoryBySport;

    public class GetCustomerCategoryBySportViewModel
    {
        public GetCustomerCategoryBySportViewModel(GetCustomerCategoryBySportModel data)
        {
            this.SportID = data.SportID;
            this.CategoryName = data.CategoryName;
            this.CustomerClass = (data.CustomerClass == -1 || data.CustomerClass == 0) ? "-" : data.CustomerClass.ToString();

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
                case 229:
                    this.SportType = "Non-Sport"; break;
                default: break;
            }
        }

        public int SportID { get; set; }

        public string SportType { get; set; }

        public string CategoryName { get; set; }

        public string CustomerClass { get; set; }
    }
}
