namespace Fanex.NAP.CTS.Service
{
    using Fanex.Data.Repository;
    using Fanex.Logging;
    using Fanex.NAP.CTS.Core.Helpers.Api;
    using Fanex.NAP.CTS.Core.Utilities;
    using Fanex.NAP.CTS.Criteria.AssociationDetection;
    using Fanex.NAP.CTS.Model.API;
    using Fanex.NAP.CTS.Resource.Consts;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public class AntiFraudCenterService : IAntiFraudCenterService
    {
        private readonly IDynamicRepository repository;

        public AntiFraudCenterService()
        {
            repository = new DynamicRepository();
        }

        public void InsertAFCAccounts(IEnumerable<long> custIds)
        {
            try
            {
                if (custIds != null && custIds.Any())
                {
                    var afcAccountInfo = GetAccountInfo(string.Join(",", custIds));

                    if (afcAccountInfo != null && afcAccountInfo.Any())
                    {
                        var inputData = new List<InsertAccountParasModel>();

                        afcAccountInfo.ToList().ForEach(c =>
                        {
                            inputData.Add(new InsertAccountParasModel
                            {
                                AccountID = c.CustId,
                                CountryCode = c.CountryCode,
                                Phone = c.PhoneNo,
                                Email = c.Email,
                                BankAccountID = c.AccountNum,
                                BankName = c.AccountName
                            });
                        });

                        var para = new InsertAccountParasViewModel
                        {
                            Key = Consts.AFCInternalKey,
                            Data = inputData
                        };

                        var result = ApiHelper.Post<bool>(ConfigSettings.InsertAntiFraudAccounts, para, 300);

                        Task.WaitAll(result);
                    }
                }
            }
            catch (Exception ex)
            {
                LogService.Error($"{ConfigSettings.EnvironmentName} - Web - Insert Accounts to AFC DB - Exception: "
                    + ex.Message
                    + ex.StackTrace + "\n"
                    + ex.InnerException.Message + "\n"
                    + $"DataInput: {JsonConvert.SerializeObject(custIds) + "\n"}"
                    + ex.InnerException.Source
                    + ex.InnerException.StackTrace);
            }
        }

        public void DeleteAFCAccounts(IEnumerable<long> custIds)
        {
            try
            {
                if (custIds != null && custIds.Any())
                {
                    var b2cCustIds = GetB2CCustIds(string.Join(",", custIds));

                    if (b2cCustIds != null && b2cCustIds.Any())
                    {
                        var inputData = new List<DeleteAccountParasModel>();

                        b2cCustIds.ToList().ForEach(c =>
                        {
                            inputData.Add(new DeleteAccountParasModel
                            {
                                AccountID = c.B2CCustId
                            });
                        });

                        var para = new DeleteAccountParasViewModel
                        {
                            Key = Consts.AFCInternalKey,
                            Data = inputData
                        };

                        var result = ApiHelper.Post<bool>(ConfigSettings.DeleteAntiFraudAccounts, para, 300);

                        Task.WaitAll(result);
                    }
                }
            }
            catch (Exception ex)
            {
                LogService.Error($"{ConfigSettings.EnvironmentName} - Web - Insert Accounts to AFC DB - Exception: "
                    + ex.Message
                    + ex.StackTrace + "\n"
                    + ex.InnerException.Message + "\n"
                    + $"DataInput: {JsonConvert.SerializeObject(custIds) + "\n"}"
                    + ex.InnerException.Source
                    + ex.InnerException.StackTrace);
            }
        }

        private IEnumerable<GetAFCAccountModel> GetAccountInfo(string custIds)
        {
            if (!string.IsNullOrEmpty(custIds))
            {
                var filterB2CLicensee = FilterCustIdB2CLicensee(custIds);

                if(filterB2CLicensee != null && filterB2CLicensee.Any())
                {
                    var b2cCustIds = GetB2CCustIds(string.Join(",", filterB2CLicensee));

                    if (b2cCustIds != null && b2cCustIds.Any())
                    {
                        var b2cCustJson = JsonConvert.SerializeObject(b2cCustIds.Select(c => c.B2CCustId));

                        return GetAFCAccountInfo(b2cCustJson) ?? new List<GetAFCAccountModel>();
                    }
                }
            }

            return new List<GetAFCAccountModel>();
        }

        #region DataAccess Layer

        public IEnumerable<GetAFCAccountModel> GetAFCAccountInfo(string custIdJson)
        {
            var criteria = new GetAFCAccountInfoCriteria
            {
                B2CCustIdJson = custIdJson
            };

            return repository.Fetch<GetAFCAccountModel>(criteria);
        }

        public IEnumerable<GetB2CModel> GetB2CCustIds(string custIds)
        {
            var criteria = new GetB2CCustIdsCriteria
            {
                CustIds = custIds
            };

            return repository.Fetch<GetB2CModel>(criteria);
        }

        public IEnumerable<long> FilterCustIdB2CLicensee(string custIds)
        {
            var criteria = new FilterB2CLicenseeGetCriteria
            {
                CustIDs = custIds
            };

            return repository.Fetch<long>(criteria);
        }

        #endregion DataAccess Layer
    }
}