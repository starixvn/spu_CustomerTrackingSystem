namespace Fanex.NAP.CTS.Model.AIInsights
{
    public class AIInsightsFileUploadModel
    {
        public string TempFolder => "~/TempFiles";

        public int MaxFileSizeBytes => 500 * 1024 * 1024;

        public string[] AllowedExtensions => new[] { ".xls", ".xlsx" };
    }
}
