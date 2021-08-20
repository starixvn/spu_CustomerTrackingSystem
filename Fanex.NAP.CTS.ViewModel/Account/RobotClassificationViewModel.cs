namespace Fanex.NAP.CTS.ViewModel.Account
{
    using Fanex.NAP.CTS.Core.BaseClasses;
    using Fanex.NAP.CTS.Model.Account;
    using Fanex.NAP.CTS.Resource.Consts;

    public class RobotClassificationViewModel
    {
        readonly RobotClassification model;

        public RobotClassificationViewModel(RobotClassification model)
        {
            this.model = model;
        }

        public string Sport
        {
            get { return CachedRef.GetSportTypeName(model.SportTypeID); }
        }

        public string Bettype
        {
            get { return CachedRef.GetBettypeName(model.BetTypeID); }
        }

        public string LiveIndicator
        {
            get { return model.LiveIndicator ? "Live" : "Non-Live"; }
        }

        public string RobotType
        {
            get
            {
                var result = string.Empty;
                switch (model.RobotType)
                {
                    case 0:
                        result = "Observed";
                        break;
                    case 1:
                        result = "Good";
                        break;
                    case 2:
                        result = "Bad";
                        break;
                    default:
                        break;
                }

                return result;
            }
        }

        public string ScannedDate
        {
            get { return model.ScannedDate.ToString(Consts.FormatDateToString_MMMddyyyy); }
        }
    }
}
