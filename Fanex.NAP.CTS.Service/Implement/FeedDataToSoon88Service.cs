namespace Fanex.NAP.CTS.Service.Implement
{
    using Fanex.Data.Repository;
    using Fanex.Logging;
    using Fanex.NAP.CTS.Core.Helpers.Api;
    using Fanex.NAP.CTS.Criteria.Account;
    using Fanex.NAP.CTS.Model.Association;
    using Fanex.NAP.CTS.Model.SyncCustomerToMainDb;
    using Fanex.NAP.CTS.Resource.Consts;
    using Fanex.NAP.CTS.ViewModel.FeedDataToSoon88;
    using Newtonsoft.Json;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Linq;
    using System.Security.Cryptography;
    using System.Text;
    using System.Threading.Tasks;

    public class FeedDataToSoon88Service
    {
        readonly IDynamicRepository repository;

        public FeedDataToSoon88Service()
        {
            repository = new DynamicRepository();
        }

        public void FeedDataToSoon88(IEnumerable<CustomerClassForMainDb> custClasses)
        {
            //var url = "https://player.xoon88.com/fps/updateFPSstatus";
            var url = ConfigurationManager.AppSettings["Soon88FeedDataAPI"];
            var updateList = custClasses.Where(c => c.CustomerClass != -1);
            var deleteList = custClasses.Where(c => c.CustomerClass == -1);

            if (updateList.Any())
            {
                UpdateDataToSoon88(updateList, url);
            }

            if (deleteList.Any())
            {
                DeleteDataInSoon88(deleteList, url);
            }
        }

        private void UpdateDataToSoon88(IEnumerable<CustomerClassForMainDb> updateList, string url)
        {
            var result = GetAssociationWithCategory(updateList, false);

            var dataGroupByAccount = result.GroupBy(a => a.RegisterName);
            foreach (var groupAcc in dataGroupByAccount)
            {
                var soon88 = StatisticAccountByCategoryInS88(groupAcc.Where(c => c.IsSoon88.HasValue && c.IsSoon88.Value));
                var others = StatisticAccountByCategoryNotInS88(groupAcc.Where(c => c.IsSoon88.HasValue && !c.IsSoon88.Value));

                var tempData = new PreparedUpdateDataToSoon88ViewModel()
                {
                    reason = groupAcc.FirstOrDefault().CategoryName,
                    account = groupAcc.Key,
                    association = new
                    {
                        soon88,
                        others
                    }
                };

                var finalData = new UpdateDataToSoon88ViewModel()
                {
                    reason = groupAcc.FirstOrDefault().CategoryName,
                    account = groupAcc.Key,
                    association = tempData.association,
                    signature = CreateMD5Hash(JsonConvert.SerializeObject(tempData) + Consts.SALT)
                };

                var returnedData = ApiHelper.PostWithProxy<S88Result>(url, finalData);
                Task.WhenAll(returnedData);
                if (returnedData.Result.result == "success")
                {
                    LogService.Info($"FeedDataToSoon88Helper - UpdateDataToSoon88 success");
                }
                else
                {
                    LogService.Info($"FeedDataToSoon88Helper - DeleteDataInSoon88 failed");
                }
            }
        }

        private void DeleteDataInSoon88(IEnumerable<CustomerClassForMainDb> deleteList, string url)
        {
            var result = GetAssociationWithCategory(deleteList, true);

            foreach (var item in result)
            {
                var tempData = new PreparedDeleteDataToSoon88ViewModel()
                {
                    reason = item.CategoryName,
                    account = item.RegisterName
                };

                var finalData = new DeleteDataToSoon88ViewModel()
                {
                    reason = item.CategoryName,
                    account = item.RegisterName,
                    signature = CreateMD5Hash(JsonConvert.SerializeObject(tempData) + Consts.SALT)
                };

                var returnedData = ApiHelper.PostWithProxy<S88Result>(url, finalData);
                Task.WhenAll(returnedData);
                if (returnedData.Result.result == "success")
                {
                    LogService.Info($"FeedDataToSoon88Helper - UpdateDataToSoon88 success");
                }
                else
                {
                    LogService.Info($"FeedDataToSoon88Helper - DeleteDataInSoon88 failed");
                }
            }
        }

        private IEnumerable<AssociationWithCategory> GetAssociationWithCategory(IEnumerable<CustomerClassForMainDb> custClasses, bool isDelete)
        {
            var criteria = new GetAssociationWithCategoryCriteria()
            {
                CustIds = string.Join(",", custClasses.Select(c => c.CustID)),
                IsDelete = isDelete
            };

            var result = repository.Fetch<AssociationWithCategory>(criteria);
            return result;
        }

        private Dictionary<string, int> StatisticAccountByCategoryNotInS88(IEnumerable<AssociationWithCategory> assList)
        {
            var statResult = new Dictionary<string, int>();
            foreach (var item in assList.GroupBy(a => a.AssCategoryName))
            {
                statResult.Add(item.Key, item.Count());
            }

            return statResult;
        }

        private Dictionary<string, IEnumerable<string>> StatisticAccountByCategoryInS88(IEnumerable<AssociationWithCategory> assList)
        {
            var statResult = new Dictionary<string, IEnumerable<string>>();
            foreach (var item in assList.GroupBy(a => a.AssCategoryName))
            {
                statResult.Add(item.Key, item.Select(b => b.AssRegisterName));
            }

            return statResult;
        }

        private string CreateMD5Hash(string input)
        {
            // Use input string to calculate MD5 hash
            using (MD5 md5 = MD5.Create())
            {
                byte[] inputBytes = Encoding.ASCII.GetBytes(input);
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                // Convert the byte array to hexadecimal string
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    sb.Append(hashBytes[i].ToString("x2"));
                }

                return sb.ToString();
            }
        }

    }
}
