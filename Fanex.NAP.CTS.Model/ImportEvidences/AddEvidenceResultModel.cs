namespace Fanex.NAP.CTS.Model.ImportEvidences
{
    using Fanex.NAP.CTS.Model.CustomerClassification;
    using System.Collections.Generic;

    public class AddEvidenceResultModel
    {
        public string ErrorMessage { get; set; }

        public IEnumerable<CustEvidenceModel> Eivdences { get; set; }
    }
}
