namespace Fanex.NAP.CTS.Model.AIInsights
{
    public class AIInsightsFileUploadModel
    {
        public string TempFolder => "~/TempFiles";

        public int MaxFileSizeBytes => 20 * 1024 * 1024; // 20 MB

        public string[] AllowedExtensions => new[] { ".xls" };
    }
}
