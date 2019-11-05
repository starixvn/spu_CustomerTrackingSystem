namespace Fanex.NAP.CTS.Criteria.FraudCase.DelayedGoal
{
    using Fanex.NAP.CTS.Core.BaseClasses;
    using System.Collections;

    public class DelayedGoalAccountInsertCriteria : CommandBase
    {
        public string Username { get; set; }

        public int UserId { get; set; }
        
        public int addedcustid { get; set; }

        public override Hashtable GetOutParam()
        {
            var hash = new Hashtable();

            hash.Add("addedcustid", addedcustid);

            return hash;
        }

        public override string GetSettingKey()
        {
            return "FS_DelayGoalAccount_Insert";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}