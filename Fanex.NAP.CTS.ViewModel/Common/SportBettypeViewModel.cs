namespace Fanex.NAP.CTS.ViewModel.Common
{
    public class SportBettypeViewModel
    {
        public int SportGroup { get; set; }

        public string SportName { get; set; }

        public string SportNameDisplay { get; set; }

        public int SporttypeId { get; set; }

        public int? LeagueGroupID { get; set; }

        public int BettypeId { get; set; }

        //[JsonProperty("BettypeId")]
        //public int Typeid { get; set; }

        //[JsonProperty("BettypeName")]
        //public string Typenamee { get; set; }
    }
}