namespace Fanex.NAP.CTS.Model.CustomerClassify
{
    public class MultipleProblemAccountsModel
    {
        public long CustID { get; set; }

        public int ReasonID { get; set; }

        //Default value = 0, except Check/Get RobotUser
        public int RobotCounter { get; set; }
    }
}