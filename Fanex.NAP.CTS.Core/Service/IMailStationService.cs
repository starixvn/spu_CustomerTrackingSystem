namespace Fanex.NAP.CTS.Core
{
    public interface IMailStationService
    {
        IMailStationService GenerateAttachment(string fileName, byte[] fileContent);

        IMailStationService GenerateMail(string subject, string title, string body);

        void SendMail();
    }
}