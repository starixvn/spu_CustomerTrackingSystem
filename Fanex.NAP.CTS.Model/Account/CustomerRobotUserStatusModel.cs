namespace Fanex.NAP.CTS.Model.Account
{
    public class CustomerRobotUserStatusModel
    {
        public CustomerRobotUserStatusModel() 
        {
            RobotType = -1;
        }

        public int CustId { get; set; }

        public bool IsRobotUser { get; set; }

        public int RobotType { get; set; }
    }
}
