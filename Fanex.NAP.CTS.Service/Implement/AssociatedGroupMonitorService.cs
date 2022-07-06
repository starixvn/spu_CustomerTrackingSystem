namespace Fanex.NAP.CTS.Service.Implement
{
    using Fanex.Data.Repository;
    using Fanex.Logging;
    using Fanex.Logging.Extensions;
    using Fanex.Logging.Extensions.Database;
    using Fanex.NAP.CTS.Core;
    using Fanex.NAP.CTS.Criteria.AssociatedGroupMonitor;
    using Fanex.NAP.CTS.Model.AssociatedGroupMonitor;
    using Fanex.NAP.CTS.Service.Interface;
    using Fanex.NAP.CTS.ViewModel.AssociatedGroupMonitor;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class AssociatedGroupMonitorService : IAssociatedGroupMonitorService
    {
        private readonly IDynamicRepository repository;

        public AssociatedGroupMonitorService()
        {
            repository = new DynamicRepository();
        }

        public IEnumerable<AssociatedGroupMonitorModel> GetAssociatedGroupMonitor()
        {
            var criteria = new GetAssociatedGroupMonitorCriteria();

            return repository.Fetch<AssociatedGroupMonitorModel>(criteria);
        }

        public DBResult AddAssociatedAccount(AddAssociatedAccountViewModel model)
        {
            var criteria = new AddAssociatedAccountCriteria
            {
            };

            try
            {
                repository.Execute(criteria);

                return new DBResult { ErrorCode = 0 };
            }
            catch (Exception ex)
            {
                var logMessage = new LogMessage(ex.Message)
                 .AddSection("Data Info", DatabaseInfoGetters.GetDbExceptionInfo(ex))
                 .AddSection("Exception Info", CommonInfoGetters.GetExceptionInfo(ex)).ToString();

                LogService.Error($"Add associated account Exception { logMessage }", ex);

                return new DBResult { ErrorCode = -1, ErrorMsg = ex.Message };
            }
        }

        public DBResult AddAssociatedGroupMonitor(AddAssociatedGroupViewModel model)
        {
            var criteria = new AddAssociatedGroupMonitorCriteria
            {
            };            

            try
            {
                repository.Execute(criteria);

                return new DBResult { ErrorCode = 0 };
            }
            catch (Exception ex)
            {
                var logMessage = new LogMessage(ex.Message)
                 .AddSection("Data Info", DatabaseInfoGetters.GetDbExceptionInfo(ex))
                 .AddSection("Exception Info", CommonInfoGetters.GetExceptionInfo(ex)).ToString();

                LogService.Error($"Add associated group monitor Exception { logMessage }", ex);

                return new DBResult { ErrorCode = -1, ErrorMsg = ex.Message };
            }
        }      

        public DBResult RemoveAssociatedGroupMonitor(RemoveAssociatedGroupViewModel model)
        {
            var criteria = new RemoveAssociatedGroupMonitorCriteria
            {
            };

            try
            {
                repository.Execute(criteria);

                return new DBResult { ErrorCode = 0 };
            }
            catch (Exception ex)
            {
                var logMessage = new LogMessage(ex.Message)
                 .AddSection("Data Info", DatabaseInfoGetters.GetDbExceptionInfo(ex))
                 .AddSection("Exception Info", CommonInfoGetters.GetExceptionInfo(ex)).ToString();

                LogService.Error($"Remove associated group monitor Exception { logMessage }", ex);

                return new DBResult { ErrorCode = -1, ErrorMsg = ex.Message };
            }
        }
    }
}
