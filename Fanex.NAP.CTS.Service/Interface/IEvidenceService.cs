namespace Fanex.NAP.CTS.Service
{
    using System.Collections.Generic;
    using Fanex.NAP.CTS.Core;
    using Fanex.NAP.CTS.Model.DTO;
    using Fanex.NAP.CTS.Model.ImportEvidences;
    using Fanex.NAP.CTS.ViewModel.Account.Evidence;
    using Fanex.NAP.CTS.ViewModel.Account.Evidence.Exception;
    using Fanex.NAP.CTS.ViewModel.Evidence;    

    public interface IEvidenceService
    { 
        IEnumerable<Evidence> GetAllEvidences();

        IEnumerable<Evidence> GetEvidencesByAccount(int accountId);

        DBResult PlaceEvidenceByAccount(GeneralInformationPlaceEvidenceByAccount evidenceByAccount);

        InformationEvidenceByAccount GetInformationEvidenceByAccount(InformationEvidenceByAccountFilter filter);

        DBResult UpdateEvidenceByAccountOfComment(InformationEvidenceByAccountUpdateOfComment evidenceOfComment);

        DBResult RetractEvidenceByAccount(InformationEvidenceByAccountRetractEvidence evidenceRetract);
        
        void AddCustomersEvidences(CustomerEvidenceList customerEvidenceList);
    }
}