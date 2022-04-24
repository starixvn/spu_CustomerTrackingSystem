namespace Fanex.NAP.CTS.Service
{
    using Fanex.Data;
    using Fanex.Data.Repository;
    using Fanex.NAP.CTS.Core;
    using Fanex.NAP.CTS.Criteria.Api;
    using Fanex.NAP.CTS.Model.DTO;
    using Fanex.NAP.CTS.Model.ImportEvidences;
    using Fanex.NAP.CTS.ViewModel.Evidence;
    using System.Collections.Generic;
    using System.Linq;

    public class EvidenceService : IEvidenceService
    {
        IDynamicRepository _repository;

        public EvidenceService()
        {
            _repository = new DynamicRepository();
        }
        #region General Information
        public DBResult PlaceEvidenceByAccount(GeneralInformationPlaceEvidenceByAccount evidenceByAccount)
        {
            return new DBResult
            {
                ErrorCode = 0,
                ErrorMsg = string.Empty
            };
        }

        public InformationEvidenceByAccount GetInformationEvidenceByAccount(InformationEvidenceByAccountFilter filter)
        {
            var evidence = new InformationEvidenceByAccount();
            var evidencesHistory = new List<InformationEvidenceByAccountHistory>();
            evidencesHistory.Add(new InformationEvidenceByAccountHistory
            {
                CreatedDate = "9/10/2019 9:55:30 PM",
                Action = "Added",
                Creator = "sputest02",
                EvidenceTypeCode = "1.1",
                EvidenceTypeName = "Credit Card Fraud",
                Comment = "this is fraud"
            });

            evidencesHistory.Add(new InformationEvidenceByAccountHistory
            {
                CreatedDate = "9/10/2019 9:55:30 PM",
                Action = "Updated",
                Creator = "sputest02",
                EvidenceTypeCode = "1.1",
                EvidenceTypeName = "Credit Card Fraud",
                Comment = "this is fraud 2"
            });

            evidence.AccountId = filter.AccountId;
            evidence.UserName = "fpsa_pgmthai";
            evidence.EvidenceTypeCode = "4.4";
            evidence.EvidenceTypeName = "Account Take-Over/Hijacking";
            evidence.CreatedBy = "sputest02 on 9/10/2019 9:55:30 PM";
            evidence.Comment = "test";
            evidence.History = evidencesHistory;

            return evidence;
        }

        public DBResult UpdateEvidenceByAccountOfComment(InformationEvidenceByAccountUpdateOfComment evidenceOfComment)
        {
            return new DBResult
            {
                ErrorCode = 0,
                ErrorMsg = string.Empty
            };
        }

        public DBResult RetractEvidenceByAccount(InformationEvidenceByAccountRetractEvidence evidenceRetract)
        {
            return new DBResult
            {
                ErrorCode = 0,
                ErrorMsg = string.Empty
            };
        }

        #endregion

        #region Retracted And Exception Evidence

        public AddEvidenceResultModel AddCustomersEvidences(PlaceMultiCustomerEvidence customerEvidenceList, int userId)
        {
            var result = new List<CustEvidenceModel>();

            var criteria = new MultiCustomerEvidencesInsertCriteria
            {
                ip_CustIds = customerEvidenceList.CustIdList,
                ip_EvidenceCodes = customerEvidenceList.EvidenceCodeList,
                ip_CreatedBy = userId
            };

            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                result = db.Query<CustEvidenceModel>(criteria).ToList();
            }

            return new AddEvidenceResultModel { ErrorMessage = criteria.ErrorMessage, Eivdences = result };
        }

        #endregion

        #region Evidence Management
        public IEnumerable<Evidence> GetAllEvidences()
        {
            var evidences = new List<Evidence>();

            evidences.Add(new Evidence { EvidenceGroupID = 1, EvidenceGroupName = "Financial", EvidenceTypeID = 1, EvidenceTypeCode = "1.1", EvidenceTypeName = "Credit Card Fraud", Description = "Description Credit Card Fraud" });
            evidences.Add(new Evidence { EvidenceGroupID = 1, EvidenceGroupName = "Financial", EvidenceTypeID = 2, EvidenceTypeCode = "1.2", EvidenceTypeName = "ACH/Debit Fraud", Description = "Description ACH/Debit Fraud" });
            evidences.Add(new Evidence { EvidenceGroupID = 1, EvidenceGroupName = "Financial", EvidenceTypeID = 3, EvidenceTypeCode = "1.3", EvidenceTypeName = "Friendly Chargeback", Description = "Description Friendly Chargeback" });
            evidences.Add(new Evidence { EvidenceGroupID = 2, EvidenceGroupName = "Cheating", EvidenceTypeID = 4, EvidenceTypeCode = "2.1", EvidenceTypeName = "Collusion", Description = "Description Collusion" });
            evidences.Add(new Evidence { EvidenceGroupID = 2, EvidenceGroupName = "Cheating", EvidenceTypeID = 5, EvidenceTypeCode = "2.2", EvidenceTypeName = "Chip Dumping", Description = "Description Chip Dumping" });
            evidences.Add(new Evidence { EvidenceGroupID = 3, EvidenceGroupName = "Misconduct", EvidenceTypeID = 6, EvidenceTypeCode = "3.1", EvidenceTypeName = "Chat Abuse", Description = "Description Chat Abuse" });

            return evidences;
        }

        public IEnumerable<Evidence> GetEvidencesByAccount(int accountId)
        {
            return new List<Evidence>();
        }
        #endregion
    }
}