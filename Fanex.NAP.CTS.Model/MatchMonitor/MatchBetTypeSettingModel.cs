namespace Fanex.NAP.CTS.Model.MatchMonitor
{
    public class MatchBetTypeSettingModel
    {
        public int SportTypeID { get; set; }

        public int BetTypeID { get; set; }

        public int BetChoiceType { get; set; }

        public string BetChoiceHome { get; set; }

        public string BetChoiceAway { get; set; }

        public string BetIDPattern { get; set; }

        public long BetID { get; set; }
    }
}
