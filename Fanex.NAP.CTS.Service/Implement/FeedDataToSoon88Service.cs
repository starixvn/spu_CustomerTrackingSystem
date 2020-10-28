namespace Fanex.NAP.CTS.Service.Implement
{
    using Fanex.Data.Repository;
    using Fanex.NAP.CTS.Criteria.Account;
    using Fanex.NAP.CTS.Model.Association;
    using Fanex.NAP.CTS.Model.SyncCustomerToMainDb;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Newtonsoft.Json;
    using System.Security.Cryptography;

    public class FeedDataToSoon88Service
    {
        readonly IDynamicRepository repository;

        public FeedDataToSoon88Service()
        {
            repository = new DynamicRepository();
        }

        public void FeedDataToSoon88(IEnumerable<CustomerClassForMainDb> custClasses)
        {
            var updateList = custClasses.Where(c => c.CustomerClass != -1);
            var deleteList = custClasses.Where(c => c.CustomerClass == -1);

            var data = GetAssociationWithCategory(updateList);
            
            
        }

        private IEnumerable<AssociationWithCategory> GetAssociationWithCategory(IEnumerable<CustomerClassForMainDb> custClasses)
        {
            var criteria = new GetAssociationWithCategoryCriteria()
            {
                CustIds = string.Join(",", custClasses.Select(c => c.CustID))
            };

            var result = repository.Fetch<AssociationWithCategory>(criteria);

            var dataGroupByAccount = result.GroupBy(a => a.RegisterName);
            foreach (var groupAcc in dataGroupByAccount)
            {
                var a = new
                {
                    action = "block",
                    reason = groupAcc.FirstOrDefault().CategoryName,
                    account = groupAcc.Key,
                    association = new
                    {
                        soon88 = StatisticAccountByCategoryInS88(groupAcc.Where(c => c.IsSoon88)),
                        others = StatisticAccountByCategoryNotInS88(groupAcc.Where(c => !c.IsSoon88)),
                    }
                };
            }
            
            var minifiedData = JsonConvert.SerializeObject(a);

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
            // Step 1, calculate MD5 hash from input
            MD5 md5 = MD5.Create();
            byte[] inputBytes = Encoding.ASCII.GetBytes(input);
            byte[] hashBytes = md5.ComputeHash(inputBytes);

            // Step 2, convert byte array to hex string
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < hashBytes.Length; i++)
            {
                sb.Append(hashBytes[i].ToString("X2"));
            }
            return sb.ToString();
        }

    }
}
