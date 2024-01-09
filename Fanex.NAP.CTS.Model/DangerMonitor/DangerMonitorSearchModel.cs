namespace Fanex.NAP.CTS.Model.DangerMonitor
{
    using Newtonsoft.Json;
    using System;
    using static Fanex.NAP.CTS.Core.Extentions.Extentions;

    public class DangerMonitorSearchModel
    {
        public DangerMonitorSearchModel() { }

        public DangerMonitorSearchModel(DangerMonitorSearchModel model)
        {
            FromDate = model.FromDate;
            ToDate = model.ToDate;
            DangerList = model.DangerList;
            Sports = model.Sports;
            Event = model.Event;
            Market = model.Market;
            MatchSection = model.MatchSection;
            BetTypes = model.BetTypes;
        }

        public DateTime? FromDate { get; set; }

        public DateTime? ToDate { get; set; }

        public string DangerList { get; set; }

        public string Sports { get; set; }

        public string Event { get; set; } /* League / Match */

        public string Market { get; set; }

        public string MatchSection { get; set; }

        public string BetTypes { get; set; }

        public int LastID { get; set; }

        public string LastMatchSection { get; set; }

        public DangerMonitorSearchModel Clone() => new DangerMonitorSearchModel(this);

        public string BuildCacheKey(string nameOfFuntion, string region, string filterkey, string keyName)
        {
            return $"{nameOfFuntion ?? string.Empty}_{region ?? string.Empty}_{filterkey ?? string.Empty}_{keyName ?? string.Empty}";
        }

        public string BuildMD5FilterKey()
        {
            var model = Clone();
            model.Event = string.Empty;

            var modelConvertToJsonString = JsonConvert.SerializeObject(model);
            return CreateMD5Hash(modelConvertToJsonString);
        }
    }
}