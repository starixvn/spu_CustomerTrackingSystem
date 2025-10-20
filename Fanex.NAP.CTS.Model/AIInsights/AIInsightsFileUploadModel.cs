namespace Fanex.NAP.CTS.Model.AIInsights
{
    using Fanex.NAP.CTS.Core.Utilities;

    public class AIInsightsFileUploadModel
    {
        public string TempFolder => "~/TempFiles";

        public int MaxFileSizeBytes => ConfigSettings.AIUploadMaxFileSizeMB * 1024 * 1024;

        public int MaxFileSizeMB => ConfigSettings.AIUploadMaxFileSizeMB;

        public string AllowedExtensions => ConfigSettings.AIUploadSupportExtension;
    }
}
