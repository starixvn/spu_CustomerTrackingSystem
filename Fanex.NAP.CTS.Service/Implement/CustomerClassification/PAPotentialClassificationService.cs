namespace Fanex.NAP.CTS.Service.Implement.CustomerClassification
{
    using Fanex.Data.Repository;
    using Fanex.NAP.CTS.Criteria.CustomerClassification;
    using Fanex.NAP.CTS.Model;
    using Fanex.NAP.CTS.Model.CustomerClassification;
    using Fanex.NAP.CTS.Model.CustomerClassification.RescanCustomerClassification;
    using Fanex.NAP.CTS.Service.Interface.CustomerClassification;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class PAPotentialClassificationService : BaseCustomerClassificationService, IPAPotentialClassificationService
    {
        private readonly IDynamicRepository repository;

        public PAPotentialClassificationService()
        {
            repository = new DynamicRepository();
        }        

        public ResultInsertCustomerClassificationModel InsertDangerousScorePA(string custIDs)
        {
            var rescanDangerous = this.GetDangerousScorePA(custIDs);

            if (rescanDangerous != null && rescanDangerous.Any())
            {
                var paDangerousScores = rescanDangerous.Select(x => new InsertProblemAccountModel(x));

                var modelInsert = new InsertCustomerClassificationModel()
                {
                    CustClassificationJson = JsonConvert.SerializeObject(paDangerousScores)
                };

                return base.InsertCustomerClassification(CCInsertInputFlow.Potential, modelInsert);
            }

            return new ResultInsertCustomerClassificationModel();
        }

        public ResultInsertCustomerClassificationModel InsertInitialSmartPA(string custIDs)
        {
            var rescanInitialSmart = this.GetInitialSmartPA(custIDs);

            if (rescanInitialSmart != null && rescanInitialSmart.Any())
            {
                var pasInitialSmart = rescanInitialSmart.Select(x => new InsertProblemAccountModel(x));

                var modelInsert = new InsertCustomerClassificationModel()
                {
                    CustClassificationJson = JsonConvert.SerializeObject(pasInitialSmart)
                };

                return base.InsertCustomerClassification(CCInsertInputFlow.Potential, modelInsert);
            }

            return new ResultInsertCustomerClassificationModel();
        }

        public ResultInsertCustomerClassificationModel InsertInitialTWGroupBettingPA(string custIDs)
        {
            var rescanInitialTWGB = this.GetInitialTWGroupBetting(custIDs);

            if (rescanInitialTWGB != null && rescanInitialTWGB.Any())
            {
                var custInfos = this.GetCustInfos(custIDs);

                var paTWGBs = rescanInitialTWGB.Join(custInfos, twGBCust => twGBCust.CustID, ctsCust => ctsCust.CustID,
                               (twGBCust, ctsCust) => new InsertProblemAccountModel(twGBCust, ctsCust));

                var modelInsert = new InsertCustomerClassificationModel()
                {
                    CustClassificationJson = JsonConvert.SerializeObject(paTWGBs)
                };

                return base.InsertCustomerClassification(CCInsertInputFlow.Potential, modelInsert);
            }

            return new ResultInsertCustomerClassificationModel();
        }

        public IEnumerable<RescanPADangerousModel> GetDangerousScorePA(string custIDs)
        {
            var criteria = new RescanDangerousScorePACriteria
            {
                CustIDs = custIDs
            };

            return repository.Fetch<RescanPADangerousModel>(criteria);
        }

        public IEnumerable<TWInitialSmartPAModel> GetInitialSmartPA(string custIDs)
        {
            var criteria = new CheckExistTWInitialSmartPACriteria
            {
                CustIDs = custIDs
            };

            return repository.Fetch<TWInitialSmartPAModel>(criteria);
        }

        public IEnumerable<GetInitialTWGroupBettingModel> GetInitialTWGroupBetting(string custIDs)
        {
            var criteria = new RescanInitialTWGroupBettingCriteria() { CustIDs = custIDs };

            return repository.Fetch<GetInitialTWGroupBettingModel>(criteria);
        }

        public IEnumerable<PotentialPACustInfoModel> GetCustInfos(string custIDs)
        {
            var criteria = new GetCustInfoByCustIDCriteria
            {
                CustIDs = custIDs
            };

            return repository.Fetch<PotentialPACustInfoModel>(criteria);
        }
    }
}
