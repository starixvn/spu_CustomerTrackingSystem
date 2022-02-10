namespace Fanex.NAP.CTS.Service.Implement
{
    using Fanex.Data.Repository;
    using Fanex.NAP.CTS.Criteria.Account;
    using Fanex.NAP.CTS.Model.Account;
    using Fanex.NAP.CTS.Service.Interface;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class RobotClassificationService : IRobotClassificationService
    {
        private readonly IDynamicRepository repository;

        public RobotClassificationService()
        {
            repository = new DynamicRepository();
        }      
        
        public CustomerRobotUserStatusModel GetStatusRobotClassification(long custID)
        {
            var criteria = new GetStatusRobotClassificationCriteria() 
            { 
                CustId = custID 
            };

            var robotStatus = repository.Fetch<CustomerRobotUserStatusModel>(criteria);
                
            return robotStatus?.FirstOrDefault() ?? new CustomerRobotUserStatusModel { CustId= null, RobotType = null };
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
