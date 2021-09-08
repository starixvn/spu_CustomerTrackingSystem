namespace Fanex.NAP.CTS.Model.Account
{
    public class CustomerRobotUserStatusModel
    {
        public long? CustId { get; set; }

        public int? RobotType { get; set; }

        public bool IsRobotUser
        {
            get
            {
                return (CustId != null && RobotType != null);
            }
        }
    }
}
