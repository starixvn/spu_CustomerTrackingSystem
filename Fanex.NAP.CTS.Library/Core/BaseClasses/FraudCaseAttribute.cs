namespace Fanex.NAP.CTS.Library.Core.BaseClasses
{
    using Fanex.NAP.CTS.Library.Models;
    using System;

    [AttributeUsage(AttributeTargets.Class)]
    public class FraudCaseAttribute : Attribute
    {
        public FraudCaseAttribute(FraudCaseId fraudCase)
        {
            FraudCase = fraudCase;
        }

        public FraudCaseId FraudCase { get; }
    }
}