namespace Fanex.NAP.CTS.Service
{
    using Fanex.Data.Repository;
    using Fanex.Logging;
    using Fanex.NAP.CTS.Core.Helpers.Api;
    using Fanex.NAP.CTS.Core.Utilities;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using static Fanex.NAP.CTS.Resource.Consts.Consts;

    public class AntiFraudCenterService : IAntiFraudCenterService
    {
        private readonly IDynamicRepository repository;

        public AntiFraudCenterService()
        {
            repository = new DynamicRepository();
        }

        public async Task InsertAFCAccounts(IEnumerable<long> custIds, FromFlow fromFlow, bool isSyncAFC)
        {
            try
            { 
                if (custIds != null && custIds.Any() && isSyncAFC)
                {
                    await CallAPI(custIds, fromFlow, ConfigSettings.InsertAFCAccountsURL);
                }
            }
            catch (Exception ex)
            {
                LogService.Error($"{ConfigSettings.EnvironmentName} - AntiFraudCener - {fromFlow} - Insert Accounts - Exception: "
                    + ex.Message
                    + ex.StackTrace + "\n"
                    + ex?.InnerException?.Message + "\n"
                    + $"DataInput: {JsonConvert.SerializeObject(custIds) + "\n"}"
                    + ex?.InnerException?.Source
                    + ex?.InnerException?.StackTrace);
            }
        }

        public async Task DeleteAFCAccounts(IEnumerable<long> custIds, FromFlow fromFlow)
        {
            try
            {
                if (custIds != null && custIds.Any() && ConfigSettings.IsSyncAFCData)
                {
                    await CallAPI(custIds, fromFlow, ConfigSettings.DeleteAFCAccountsURL);
                }
            }
            catch (Exception ex)
            {
                LogService.Error($"{ConfigSettings.EnvironmentName} - AntiFraudCener - {fromFlow} - Delete Accounts to AFC DB - Exception: "
                + ex.Message
                + ex.StackTrace + "\n"
                + ex?.InnerException.Message + "\n"
                + $"DataInput: {JsonConvert.SerializeObject(custIds) + "\n"}"
                + ex?.InnerException.Source
                + ex?.InnerException.StackTrace);
            }
        }

        private async Task<bool> CallAPI(IEnumerable<long> custIds, FromFlow fromFlow, string url)
        {
            if (custIds != null && custIds.Any())
            {
                var para = new
                {
                    CustIds = custIds,
                    FromFlow = fromFlow
                };

                return await ApiHelper.Post<bool>(url, para, 300);
            }

            return true;
        }

    }
}