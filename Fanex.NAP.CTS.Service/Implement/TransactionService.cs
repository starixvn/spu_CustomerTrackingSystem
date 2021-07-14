namespace Fanex.NAP.CTS.Service
{
    using System.Collections.Generic;
    using Fanex.NAP.CTS.Model.Transaction;
    using Fanex.NAP.CTS.Core.BaseClasses;
    using Fanex.NAP.CTS.Model.IP;
    using System.Net;
    using System;
    using System.Numerics;
    using Fanex.NAP.CTS.Criteria.Account.TransactionHistory;
    using Fanex.NAP.CTS.Model.LatestTransaction;
    using Fanex.NAP.CTS.Criteria.LatestTransaction;
    using Fanex.Data;

    public class TransactionService : ITransactionService
    {      
        private IpModel Ip2Long(string ipAddress)
        {
            var ipAd = IPAddress.Parse(ipAddress);

            BigInteger ipnum = 0;

            byte[] addrBytes = ipAd.GetAddressBytes();

            if (BitConverter.IsLittleEndian)
            {
                var byteList = new List<byte>(addrBytes);
                byteList.Reverse();
                addrBytes = byteList.ToArray();
            }

            if (addrBytes.Length > 8)
            {
                //IPv6
                ipnum = BitConverter.ToUInt64(addrBytes, 8);
                ipnum <<= 64;
                ipnum += BitConverter.ToUInt64(addrBytes, 0);

                return new IpModel
                {
                    IpNumber = decimal.Parse(ipnum.ToString()),
                    IsIpV6 = true
                };
            }
            else
            {
                //IPv4
                ipnum = BitConverter.ToUInt32(addrBytes, 0);

                return new IpModel
                {
                    IpNumber = decimal.Parse(ipnum.ToString()),
                    IsIpV6 = false
                };
            }
        }        

        public IEnumerable<HistoryTransactionModel> GetTransactionHistory(long ctsCustId, int subscriberId, int dateRange, int pageSize, int pageNumber, out int totalItem)
        {
            var criteria = new GetTransactionHistoryCriteria
            {
                CTSCustId = ctsCustId,
                SubscriberId = subscriberId,
                DateRange = dateRange,
                Take = pageSize,
                Skip = pageNumber,
                TotalItem = 0
            };

            var result = DataService.GetData<HistoryTransactionModel>(criteria);

            totalItem = criteria.TotalItem;

            return result;
        }

        public IEnumerable<LatestTransactionModel> GetLatestTransaction(int subscriberId, int length)
        {
            var criteria = new GetLatestTransactionCriteria
            {
                SubscriberId = subscriberId,
                Length = length
            };

            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                return db.Query<LatestTransactionModel>(criteria);
            }
        }
    }
}