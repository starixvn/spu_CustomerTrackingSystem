namespace Fanex.NAP.CTS.ViewModel.Account
{
    public class RobotClassificationViewModel
    {
        public string ScannedDate { get; set; }
        
        public string BettingStatistic { get; set; }

        public int RobotType { get; set; }

        public string RobotTypeName
        {
            get
            {
                var result = string.Empty;

                switch (this.RobotType)
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
    }
}
