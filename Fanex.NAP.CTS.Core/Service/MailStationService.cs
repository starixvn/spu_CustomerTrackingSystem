namespace Fanex.NAP.CTS.Core
{
    using Fanex.MailDelivery.Client;
    using Fanex.MailDelivery.Core.Models;
    using OfficeOpenXml;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

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
        
        public bool SendMail()
        {
            MailClientHandler.Start();

            MailClientHandler.SendFunctionMail(_mail);

            MailClientHandler.Stop();

            return true;
        }

        public bool SaveToExcelFile(string savePath, string password)
        {
            if (_attachmentList.Any())
            {
                var fileBytes = _attachmentList[0].FileContent;

                try
                {
                    using (var memoryStream = new MemoryStream(fileBytes))
                    {
                        var excelPackage = new ExcelPackage(memoryStream);

                        excelPackage.SaveAs(new FileInfo(savePath), password);
                    }
                }
                catch
                {
                    return false;
                }                
            }

            return true;
        }

    }
}
