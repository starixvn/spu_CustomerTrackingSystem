namespace Fanex.NAP.CTS.Library.Services
{
    using System.Collections.Generic;
    using Fanex.NAP.CTS.Library.Models;
    using Fanex.NAP.CTS.Library.ViewModels.Account.Evidence;
    using Fanex.NAP.CTS.Library.ViewModels.Account.Evidence.Exception;
    using Fanex.NAP.CTS.Library.ViewModels.Evidence;    

    public interface IEvidenceService
    {
        ChainsToEvidence GetChainsToEvidence(ChainsToEvidenceFilter filter);

        DBResult AddExceptionEvidence(ExceptionEvidenceAdd exceptionEvidenceAdd);

        RetractedAndExceptionEvidence GetRetractedAndExceptionEvidence(RetractedAndExceptionEvidenceFilter filter);

        IEnumerable<Evidence> GetAllEvidences();

        IEnumerable<Evidence> GetEvidencesByAccount(int accountId);

        DBResult PlaceEvidenceByAccount(GeneralInformationPlaceEvidenceByAccount evidenceByAccount);

        InformationEvidenceByAccount GetInformationEvidenceByAccount(InformationEvidenceByAccountFilter filter);

        DBResult UpdateEvidenceByAccountOfComment(InformationEvidenceByAccountUpdateOfComment evidenceOfComment);

        DBResult RetractEvidenceByAccount(InformationEvidenceByAccountRetractEvidence evidenceRetract);
    }
}