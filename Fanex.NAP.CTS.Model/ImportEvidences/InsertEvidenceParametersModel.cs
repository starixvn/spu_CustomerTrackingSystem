using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fanex.NAP.CTS.Model.ImportEvidences
{
    public class InsertEvidenceParametersModel
    {
        public IEnumerable<ResultCheckAccountEvidenceModel> CheckAccountEvidenceList { get; set; }

        public IEnumerable<AddAccountEvidenceModel> AccountEvidences { get; set; }

        public string FileName { get; set; }

        public int UserId { get; set; }

        public bool IsOverride { get; set; }
    }
}
