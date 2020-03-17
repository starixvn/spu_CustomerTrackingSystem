namespace Fanex.NAP.CTS.Service
{
    using System.Collections.Generic;
    using Fanex.NAP.CTS.Model.Transaction;
    using Fanex.NAP.CTS.Criteria.Transaction.History;
    using Fanex.NAP.CTS.Core.BaseClasses;
    using Fanex.NAP.CTS.Criteria.Transaction;
    using Fanex.NAP.CTS.ViewModel.Account.TransactionHistory;
    using System.Linq;
    using Fanex.NAP.CTS.Model.IP;
    using System.Net;
    using System;
    using System.Numerics;
    using Fanex.NAP.CTS.Criteria.Account.TransactionHistory;

    public class TransactionService : ITransactionService
    {
        //public IEnumerable<HistoryTransactionModel> GetTransactionHistory(TransactionHistoryFilter filter)
        //{
        //    var criteria = new TransactionHistoryGetByCustIdCriteria
        //    {
        //        ip_accountId = filter.AccountId,
        //        ip_historyRange = (int)filter.ReportLength
        //    };

        //    return DataService.GetData<Transaction>(criteria);
        //}

        public IPInformation GetIPInformation(string ipAddress)
        {
            var ipModel = Ip2Long(ipAddress);

            var criteria = new IPInformationGetByRangeLocationCriteria
            {
                ip_IP = ipModel.IsIpV6 ? ipModel.IpNumber : ipModel.IpNumber + 281470681743360
            };

            var data = DataService.GetData<IPInformation>(criteria);

            if (data.Any())
            {
                return data.ToList()[0];
            }

            return new IPInformation();
        }

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

        public IEnumerable<HistoryTransactionModel> GetTransactionHistory(long ctsCustId, int subscriberId, DateTime fromDate, DateTime toDate)
        {
            var criteria = new GetTransactionHistoryCriteria
            {
                CTSCustId = ctsCustId,
                SubscriberId = subscriberId,
                FromDate = fromDate,
                ToDate = toDate
            };

            return DataService.GetData<HistoryTransactionModel>(criteria);
        }

        public IEnumerable<HistoryTransactionModel> GetTransactionHistory(long ctsCustId, int subscriberId, DateTime fromDate, DateTime toDate, int pageSize, int pageNumber, out int totalItem)
        {
            var criteria = new GetTransactionHistoryCriteria
            {
                CTSCustId = ctsCustId,
                SubscriberId = subscriberId,
                FromDate = fromDate,
                ToDate = toDate,
                PageSize = pageSize,
                PageNumber = pageNumber,
                TotalItem = 0
            };

            var result = DataService.GetData<HistoryTransactionModel>(criteria);

            totalItem = criteria.TotalItem;

            return result;
        }
    }
}