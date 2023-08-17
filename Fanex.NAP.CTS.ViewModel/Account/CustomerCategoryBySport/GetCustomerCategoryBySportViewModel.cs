namespace Fanex.NAP.CTS.ViewModel.Account.CustomerCategoryBySport
{
    using Fanex.NAP.CTS.Core.Extentions;
    using Fanex.NAP.CTS.Model.Account.CustomerCategoryBySport;
    using static Fanex.NAP.CTS.Resource.Consts.Consts;

    public class GetCustomerCategoryBySportViewModel
    {
        public GetCustomerCategoryBySportViewModel(GetCustomerCategoryBySportModel data)
        {
            this.SportID = data.SportID;
            this.CategoryName = data.CategoryName;
            this.CustomerClass = (data.CustomerClass == -1 || data.CustomerClass == 0) ? "-" : data.CustomerClass.ToString();
            this.SportType = ((CustomerCategorySportType)data.SportID).GetStringSlow() ?? string.Empty;
        }

        public int SportID { get; set; }

        public string SportType { get; set; }

        public string CategoryName { get; set; }

        public string CustomerClass { get; set; }
    }
}
