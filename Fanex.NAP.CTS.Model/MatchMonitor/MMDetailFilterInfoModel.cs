namespace Fanex.NAP.CTS.Model.MatchMonitor
{
    using System;
    using System.Collections.Generic;
    public class MMDetailFilterInfoModel
    {
        public IEnumerable<MMDetailFilterInforHdpModel> HDPList { get; set; }
        public IEnumerable<MMDetailFilterInforScoreModel> ScoreList { get; set; }
        public IEnumerable<MMDetailFilterInforCurrencyModel> CurrencyList { get; set; }
    }

    public class MMdetailFilterInforTypeBaseModel
    {
        public int IsLicensee { get; set; }
        public int IsCredit { get; set; }
    }

    public class MMDetailFilterInforHdpModel : MMdetailFilterInforTypeBaseModel
    {
        public decimal Hdp { get; set; }
    }

    public class MMDetailFilterInforScoreModel : MMdetailFilterInforTypeBaseModel
    {
        public int LiveHomeScore { get; set; }
        public int LiveAwayScore { get; set; }
    }

    public class MMDetailFilterInforCurrencyModel : MMdetailFilterInforTypeBaseModel
    {
        public int CurrencyId { get; set; }
        public string CurrencyName { get; set; }
    }
}
