namespace Fanex.NAP.CTS.Model.API.FPS.SearchAccount
{    
    public class SearchAccountParamModel
    {
        public int SubscriberID { get; set; }

        public string UserName { get; set; }

        public bool IsSearchPartial { get; set; }

        public int Take { get; set; }

        public int Skip { get; set; }

    }
}
