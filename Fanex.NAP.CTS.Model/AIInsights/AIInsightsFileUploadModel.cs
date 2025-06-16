namespace Fanex.NAP.CTS.Model.AIInsights
{
    public class AIInsightsFileUploadModel
    {
        public string TempFolder => "~/TempFiles";

        public int MaxFileSizeBytes => 5 * 1024 * 1024; // 5 MB

        public string[] AllowedExtensions => new[] { ".xls" };
    }
}
