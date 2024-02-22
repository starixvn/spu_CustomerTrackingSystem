namespace Fanex.NAP.CTS.ViewModel.Common
{
    using Newtonsoft.Json;

    public class SportBettypeViewModel
    {
        public int SportType { get; set; }

        public string SportName { get; set; }

        //[JsonProperty("BettypeId")]
        public int Typeid { get; set; }

        //[JsonProperty("BettypeName")]
        public string Typenamee { get; set; }
    }
}