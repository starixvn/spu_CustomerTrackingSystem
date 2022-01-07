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
    using System.Globalization;
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

        public void InsertAFCAccounts(IEnumerable<long> custIds, bool isSyncAFCData)
        {
            try
            {
                if (custIds != null && custIds.Any() && isSyncAFCData)
                {
                    var accountInfo = GetAccountForInsert(string.Join(",", custIds));

                    if (accountInfo != null && accountInfo.Any())
                    {
                        var notExistedB2CustIDs = CheckAccountExist(accountInfo);

                        SyncAccountToAFCDBAsync(accountInfo, notExistedB2CustIDs);
                    }
                }
            }
            catch (Exception ex)
            {
                LogService.Error($"{ConfigSettings.EnvironmentName} - Web - Insert Accounts to AFC DB - Exception: "
                    + ex.Message
                    + ex.StackTrace + "\n"
                    + ex?.InnerException?.Message + "\n"
                    + $"DataInput: {JsonConvert.SerializeObject(custIds) + "\n"}"
                    + ex?.InnerException?.Source
                    + ex?.InnerException?.StackTrace);
            }
        }

        public void DeleteAFCAccounts(IEnumerable<long> custIds, bool isSyncAFCData)
        {
            try
            {
                if (custIds != null && custIds.Any() && isSyncAFCData)
                {
                    var b2cCustIds = GetAccountForDelete(string.Join(",", custIds));

                    if (b2cCustIds != null && b2cCustIds.Any())
                    {
                        var para = new DeleteAccountParasViewModel
                        {
                            Key = GetKey(),
                            Data = b2cCustIds
                        };

                        var result = ApiHelper.Post<AccountDBResult>(ConfigSettings.DeleteAntiFraudAccounts, para, 300);

                        Task.WaitAll(result);

                        if (result.Result.StatusCode != (int)HttpStatus.OK)
                        {
                            LogService.Error($"{ConfigSettings.EnvironmentName} - Web - Delete Accounts to AFC DB - Exception: " + result.Result.ErrorMessage + $"\nDataInput: {JsonConvert.SerializeObject(custIds) + "\n"}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogService.Error($"{ConfigSettings.EnvironmentName} - Web - Delete Accounts to AFC DB - Exception: "
                    + ex.Message
                    + ex.StackTrace + "\n"
                    + ex?.InnerException.Message + "\n"
                    + $"DataInput: {JsonConvert.SerializeObject(custIds) + "\n"}"
                    + ex?.InnerException.Source
                    + ex?.InnerException.StackTrace);
            }
        }

        #region Private methods

        private IEnumerable<GetB2CustIDModel> GetAccountForDelete(string custIds)
        {
            var filterB2CLicensee = FilterLicenseeCustDAL(custIds);

            if (filterB2CLicensee != null && filterB2CLicensee.Any())
            {
                var custIdsToDelete = filterB2CLicensee.Where(c => c.FraudIDList == null || !c.FraudIDList.Any())?.Select(c => c.CustID)?.Distinct();

                if (custIdsToDelete != null && custIdsToDelete.Any())
                {
                    return GetB2CCustIdsDAL(string.Join(",", custIdsToDelete));
                }
            }

            return Enumerable.Empty<GetB2CustIDModel>();
        }

        private IEnumerable<GetAccountViewModel> GetAccountForInsert(string custIds)
        {
            if (!string.IsNullOrEmpty(custIds))
            {
                var filterB2CLicensee = FilterLicenseeCustDAL(custIds);

                if (filterB2CLicensee != null && filterB2CLicensee.Any())
                {
                    var probAccouts = filterB2CLicensee.Where(c => c.FraudIDList != null && c.FraudIDList.Any()).Select(c => c.CustID).Distinct();

                    var b2cCustIds = GetB2CCustIdsDAL(string.Join(",", probAccouts));

                    if (b2cCustIds != null && b2cCustIds.Any())
                    {
                        var b2cCustJson = JsonConvert.SerializeObject(b2cCustIds.Select(c => c.B2CCustId));

                        var accountInfo = GetB2CInfoDAL(b2cCustJson);

                        return MappingFrauIDs(accountInfo, filterB2CLicensee, b2cCustIds);
                    }
                }
            }

            return Enumerable.Empty<GetAccountViewModel>();
        }

        private IEnumerable<GetAccountViewModel> MappingFrauIDs(IEnumerable<GetAFCAccountModel> accountInfo, IEnumerable<FilterCustToSyncAFCKModel> fraudIDs, IEnumerable<GetB2CustIDModel> b2cCustIDs)
        {
            if (IsValid(accountInfo, fraudIDs, b2cCustIDs))
            {
                var accInfo = accountInfo.GroupJoin(b2cCustIDs,
                                   a => a.CustId,
                                   b => b.B2CCustId,
                                   (a, b) => new { a, b })
                               .SelectMany(
                                    group => group.b.DefaultIfEmpty(),
                                    (group, temp) => new GetAccountViewModel
                                    {
                                        CustId = temp?.CustId,
                                        B2CustId = group.a.CustId,
                                        CountryCode = group.a.CountryCode,
                                        PhoneNo = group.a.PhoneNo,
                                        AccountName = group.a.AccountName,
                                        AccountNum = group.a.AccountNum,
                                        Email = group.a.Email,
                                        FullName = group.a.FullName
                                    })
                                .GroupJoin(fraudIDs,
                                    a => a.CustId,
                                    f => f.CustID,
                                    (a, f) => new { a, f })
                                .SelectMany(
                                   group => group.f.DefaultIfEmpty(),
                                   (group, temp) => new GetAccountViewModel
                                   {
                                       CustId = group.a.CustId,
                                       B2CustId = group.a.B2CustId,
                                       CountryCode = new RegionInfo(group.a.CountryCode)?.ThreeLetterISORegionName ?? string.Empty,
                                       PhoneNo = group.a.PhoneNo,
                                       AccountName = group.a.AccountName,
                                       AccountNum = group.a.AccountNum,
                                       Email = group.a.Email,
                                       FullName = group.a.FullName,
                                       FraudID = temp?.FraudIDList
                                   });

                return accInfo ?? Enumerable.Empty<GetAccountViewModel>();
            }

            return Enumerable.Empty<GetAccountViewModel>();
        }

        private IEnumerable<string> CheckAccountExist(IEnumerable<GetAccountViewModel> accInfo)
        {
            if (accInfo != null && accInfo.Any())
            {
                var participants = GetParticipant();

                if (participants != null && participants.Any())
                {
                    var internalInfo = participants.SingleOrDefault(c => string.Equals(c.SecretKey, GetKey()));

                    if (internalInfo == null)
                    {
                        throw new Exception("AFC - Internal Key is Invalid");
                    }

                    var result = CheckAccountValidationDAL(internalInfo.ParticipantID, accInfo.Select(c => new CheckAccountExistModel { AccountID = c.B2CustId }));

                    if (result != null && result.Any())
                    {
                        var notExistedAcc = result.Where(c => string.Equals(c.Status, "Not Exist")).Select(c => c.AccountID);

                        return notExistedAcc ?? Enumerable.Empty<string>();
                    }
                }
            }

            return Enumerable.Empty<string>();
        }

        private void SyncAccountToAFCDBAsync(IEnumerable<GetAccountViewModel> accountInfo, IEnumerable<string> notExistedAccount)
        {
            if (accountInfo != null && accountInfo.Any())
            {
                var accToInsert = notExistedAccount.Any() ? accountInfo.Where(c => notExistedAccount.Contains(c.B2CustId)) : null;
                var accToUpdate = notExistedAccount.Any() ? accountInfo.Where(c => !notExistedAccount.Contains(c.B2CustId)) : accountInfo;

                var tasks = new List<Task>
                {
                    Task.Run(async () =>
                    {
                        try
                        {
                           var result = await SyncAFCData(accToInsert, ConfigSettings.InsertAntiFraudAccounts);

                            if (result.StatusCode != (int)HttpStatus.OK)
                            {
                                 LogService.Error($"{ConfigSettings.EnvironmentName} - Web - Insert Accounts to AFC DB - Exception: "+ result.ErrorMessage + $"\nDataInput: {JsonConvert.SerializeObject(accToInsert) + "\n"}");
                            }
                        }
                        catch (Exception ex)
                        {
                            LogService.Error($"{ConfigSettings.EnvironmentName} - Web - Insert Accounts to AFC DB - Exception: "
                                + ex.Message
                                + ex.StackTrace + "\n"
                                + $"DataInput: {JsonConvert.SerializeObject(accToInsert) + "\n"}");
                        }
                    }),

                    Task.Run(async () =>
                    {
                        try
                        {
                            var result = await SyncAFCData(accToUpdate, ConfigSettings.UpdateAntiFraudAccounts);

                            if (result.StatusCode != (int)HttpStatus.OK)
                            {
                                 LogService.Error($"{ConfigSettings.EnvironmentName} - Web - Update Accounts to AFC DB - Exception: "+ result.ErrorMessage + $"\nDataInput: {JsonConvert.SerializeObject(accToUpdate) + "\n"}");
                            }
                        }
                        catch (Exception ex)
                        {
                            LogService.Error($"{ConfigSettings.EnvironmentName} - Web - Update Accounts to AFC DB - Exception: "
                                + ex.Message
                                + ex.StackTrace + "\n"
                                + $"DataInput: {JsonConvert.SerializeObject(accToUpdate) + "\n"}");
                        }
                    })
                };

                Task.WhenAll(tasks.ToArray());
            }
        }

        /// <summary>
        /// Insert or Update AFC account
        /// </summary>
        /// <param name="accInfo"></param>
        /// <param name="isInsertAction"></param>
        private async Task<AccountDBResult> SyncAFCData(IEnumerable<GetAccountViewModel> accInfo, string url)
        {
            if (accInfo != null && accInfo.Any())
            {
                var para = new InsertAccountParasViewModel
                {
                    Key = GetKey(),
                    Data = accInfo
                };

                return await ApiHelper.Post<AccountDBResult>(url, para, 300);
            }

            return new AccountDBResult { StatusCode = (int)HttpStatus.OK, ErrorMessage = string.Empty };
        }

        private string GetKey()
        {
            return ConfigSettings.EnvironmentName == "PRO" ? Consts.AFCInternal_ProKey : Consts.AFCInternal_LocalKey;
        }

        private static bool IsValid(IEnumerable<GetAFCAccountModel> accountInfo, IEnumerable<FilterCustToSyncAFCKModel> fraudIDs, IEnumerable<GetB2CustIDModel> b2cCustIDs)
        {
            if (accountInfo != null && fraudIDs != null && b2cCustIDs != null)
            {
                return accountInfo.Any() && fraudIDs.Any() && b2cCustIDs.Any();
            }

            return false;
        }

        #endregion Private methods

        #region DataAccess Layer

        public IEnumerable<GetAFCAccountModel> GetB2CInfoDAL(string custIdJson)
        {
            var criteria = new GetAFCAccountInfoCriteria
            {
                B2CCustIdJson = custIdJson
            };

            return repository.Fetch<GetAFCAccountModel>(criteria);
        }

        public IEnumerable<GetB2CustIDModel> GetB2CCustIdsDAL(string custIds)
        {
            var criteria = new GetB2CCustIdsCriteria
            {
                CustIds = custIds
            };

            return repository.Fetch<GetB2CustIDModel>(criteria);
        }

        public IEnumerable<FilterCustToSyncAFCKModel> FilterLicenseeCustDAL(string custIds)
        {
            var criteria = new LicenseeCustFilterCriteria
            {
                CustIDs = custIds
            };

            return repository.Fetch<FilterCustToSyncAFCKModel>(criteria);
        }

        public IEnumerable<CheckAccountExistModel> CheckAccountValidationDAL(int? participantID, IEnumerable<CheckAccountExistModel> accountIDs)
        {
            var criteria = new AccountCheckExistCriteria
            {
                JsonAccountInfo = JsonConvert.SerializeObject(accountIDs),
                ParticipantID = participantID
            };

            return repository.Fetch<CheckAccountExistModel>(criteria);
        }

        public IEnumerable<ParticipantModel> GetParticipant()
        {
            var criteria = new ParticipantGetCriteria();

            return repository.Fetch<ParticipantModel>(criteria);
        }

        #endregion DataAccess Layer
    }
}