namespace Fanex.NAP.CTS.Model.AIInsights
{
    public class AIInsightsFileUploadModel
    {
        public string TempFolder => "~/TempFiles";

        public int MaxFileSizeBytes => 200 * 1024 * 1024; // 50MB

        public string[] AllowedExtensions => new[] { ".xls", ".xlsx" };
    }
}
