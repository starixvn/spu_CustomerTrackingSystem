namespace Fanex.NAP.CTS.Model.MatchMonitor
{
    using Fanex.NAP.CTS.Model.Common;

    public class MatchBetTypeSettingModel
    {
        public MatchBetTypeSettingModel() { }

        public MatchBetTypeSettingModel(BetTypeSettingModel data)
        {
            BetTypeID = data.BetTypeID;
            BetChoiceType = data.BetChoiceType;
            BetChoiceHome = data.BetChoiceHome;
            BetChoiceAway = data.BetChoiceAway;
            BetID = data.BetID;
            BetChoiceHomeFullName = data.BetChoiceHomeFullName;
            BetChoiceAwayFullName = data.BetChoiceAwayFullName;
        }

        public int SportTypeID { get; set; }

        public int BetTypeID { get; set; }

        public int BetChoiceType { get; set; }

        public string BetChoiceHome { get; set; }

        public string BetChoiceAway { get; set; }

        public string BetIDPattern { get; set; }

        public long BetID { get; set; }

        public string BetChoiceHomeFullName { get; set; }

        public string BetChoiceAwayFullName { get; set; }
    }
}
