namespace Fanex.NAP.CTS.Model.Common
{
    public class BetTypeSettingModel
    {
        public int? LeagueGroupID { get; set; }

        public int SportTypeID { get; set; }

        public string SportName { get; set; }

        public int BetTypeID { get; set; }

        public string BetTypeNameDisplay { get; set; }

        public int BetChoiceType { get; set; }

        public string BetChoiceHome { get; set; }

        public string BetChoiceHomeDisplay { get; set; }

        public string BetChoiceHomeFullName { get; set; }

        public string BetChoiceAway { get; set; }

        public string BetChoiceAwayDisplay { get; set; }

        public string BetChoiceAwayFullName { get; set; }

        public string BetIDPattern { get; set; }

        public long BetID { get; set; }

        public bool IsMMDetection { get; set; }

        public bool IsDefaultSelected { get; set; }

        public int PriorityOrder { get; set; }

        public string NameDetailsX { get; set; }

    }
}
