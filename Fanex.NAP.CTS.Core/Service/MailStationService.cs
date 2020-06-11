namespace Fanex.NAP.CTS.Core
{
    using Fanex.MailDelivery.Client;
    using Fanex.MailDelivery.Core.Models;
    using System.Collections.Generic;

    public class MailStationService : IMailStationService
    {
        private Mail _mail;
        private int _mailingListId;
        private List<MailAttachment> _attachmentList;

        public MailStationService(int mailingListId)
        {
            _mailingListId = mailingListId;
            _attachmentList = new List<MailAttachment>();
        }

        public IMailStationService GenerateAttachment(string fileName, byte[] fileContent)
        {
            _attachmentList.Add(new MailAttachment(fileName, fileContent, FileExtension.MS_EXCEL));

            return this;
        }

        public IMailStationService GenerateMail(string subject, string title, string body)
        {
            _mail = new Mail
            {
                Subject = subject,
                Title = title,
                Body = body,
                MailingListId = _mailingListId,
                Attachments = _attachmentList
            };

            return this;
        }

        public void SendMail()
        {
            MailClientHandler.Start();

            MailClientHandler.SendFunctionMail(_mail);

            MailClientHandler.Stop();
        }
    }
}
