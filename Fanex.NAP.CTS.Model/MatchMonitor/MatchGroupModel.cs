using Newtonsoft.Json;

namespace Fanex.NAP.CTS.Model.MatchMonitor
{
    public class MatchGroupModel
    {
        public int GroupID { get; set; }

        public bool IsHedging { get; set; }

        public string ListCustID { get; set; }

        public string ReasonGroupName { get; set; }

        public int TotalScore { get; set; }

        public string Key {

            get => JsonConvert.SerializeObject(new MatchGroupKeyModel  
            {
                GroupID =  GroupID, 
                TotalScore = TotalScore
            });
        }
    }
}
