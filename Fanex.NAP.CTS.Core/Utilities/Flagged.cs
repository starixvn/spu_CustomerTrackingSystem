using System.ComponentModel;

namespace Fanex.NAP.CTS.Core.Utilities
{
    public enum Flagged
    {
        [Description("Human")]
        Human = 1,
        [Description("Browserless - Empty Token")]
        EmptyToken,
        [Description("Browserless - Invalid Token")]
        InvalidToken,
        [Description("Browserless - Empty Analysis")]
        EmptyAnalysis,
        [Description("Browserless - Invalid Analysis")]
        InvalidAnalysis,
        [Description("Browserless - Empty Device")]
        EmptyDevice,
        [Description("Browserless - Invalid Device")]
        InvalidDevice,
        [Description("Browserless - Empty User Agent")]
        EmptyUserAgent,
        [Description("Browserless - Headless")]
        Headless,
    }

}
