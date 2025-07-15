namespace Fanex.NAP.CTS.Model.AIInsights
{
    public class AIInsightsFileUploadModel
    {
        public string TempFolder => "~/TempFiles";

        public int MaxFileSizeBytes => 20 * 1024 * 1024; // 100 MB

        public string[] AllowedExtensions => new[] { ".xls", ".xlsx" };
    }
}
