using Newtonsoft.Json;

namespace Fanex.NAP.CTS.Model.MatchMonitor
{
    public class MatchGroupModel
    {
        public int GroupID { get; set; }

        public int GroupName { get; set; }

        public int ScoreDiff { get; set; }

        public string Key {

            get => JsonConvert.SerializeObject(new MatchGroupKeyModel  
            {
                GroupID =  GroupID, 
                ScoreDiff = ScoreDiff,
                GroupName = GroupName
            });
        }
    }
}
