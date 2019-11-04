using System;

namespace Fanex.NAP.CTS.Model
{
    public class ReportModel
    {
        public string ServiceName { get; set; }

        public bool Status { get; set; }

        public string Exception { get; set; }

        public int Timeout { get; set; }

        public bool IsActive { get; set; }

        public string Message { get; set; }
    }
}
