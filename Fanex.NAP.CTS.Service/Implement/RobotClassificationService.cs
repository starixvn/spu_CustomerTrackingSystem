namespace Fanex.NAP.CTS.Service.Implement
{
    using System.Linq;
    using Fanex.NAP.CTS.Core.Service;
    using Fanex.NAP.CTS.Criteria.Account;
    using Fanex.NAP.CTS.Model.Account;
    using Fanex.NAP.CTS.Service.Interface;
    using System.Collections.Generic;
    using static Fanex.NAP.CTS.Resource.Consts.Consts;

    public class RobotClassificationService : IRobotClassificationService
    {
        private readonly ICriteriaService repository;

        public RobotClassificationService()
        {
            repository = new CriteriaService();
        }      
        
        public CustomerRobotUserStatusModel GetStatusRobotClassification(long custID, ROLE_CUSTOMER RoleID)
        {
            var criteria = new GetStatusRobotClassificationCriteria() 
            { 
                CustID = custID,
                RoleID = RoleID
            };

            var robotStatus = repository.Fetch<CustomerRobotUserStatusModel>(criteria);
                
            return robotStatus?.FirstOrDefault() ?? new CustomerRobotUserStatusModel { CustID = null, RobotType = null };
        }

        public IEnumerable<RobotClassification> GetHistoryRobotClassification(long custID)
        {
            var criteria = new GetHistoryRobotClassificationCriteria
            {
                CustId = custID              
            };

            return repository.Fetch<RobotClassification>(criteria);
        }
    }
}
