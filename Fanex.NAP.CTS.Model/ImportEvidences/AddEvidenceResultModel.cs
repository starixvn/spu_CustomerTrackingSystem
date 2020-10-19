namespace Fanex.NAP.CTS.Model.ImportEvidences
{
    using System.Collections.Generic;

    public class AddEvidenceResultModel
    {
        public string ErrorMessage { get; set; }

        public IEnumerable<AddAccountEvidenceModel> Eivdences { get; set; }
    }
}
