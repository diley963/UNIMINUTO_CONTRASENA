using Helpers.Interfaces;
using MailKit;
using MailKit.Net.Imap;
using MailKit.Net.Smtp;
using MailKit.Search;
using MailKit.Security;
using MimeKit;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Helpers.Implementations
{
    public enum MailServiceType
    {
        Imap,
        Smtp
    }
    public class EmailHelper : IEmailHelper, IDisposable
    {
        #region PROPERTIES
        private IMailService MailClient;
        private MailServiceType MailServiceType;
        #endregion

        #region CONSTRUCTORS
        public EmailHelper(MailServiceType mailServiceType, string server, int port, string user, string password)
        {
            this.MailServiceType = mailServiceType;

            switch (mailServiceType)
            {
                case MailServiceType.Smtp:
                    this.MailClient = new SmtpClient();
                    this.MailClient.ServerCertificateValidationCallback = (s, c, h, e) => true;
                    this.MailClient.Connect(server, port, SecureSocketOptions.Auto);

                  /*  try
                    {
                        this.MailClient.Authenticate(user, password);
                    }
                    catch (Exception)
                    {

                    }*/
                    break;
                case MailServiceType.Imap:
                    this.MailClient = new ImapClient();
                    this.MailClient.Connect(server, port, SecureSocketOptions.Auto);

                    try
                    {
                        this.MailClient.Authenticate(user, password);
                    }
                    catch (Exception)
                    {

                    }
                    break;
            }
        }
        #endregion

        #region METHODS

        /// <summary>
        /// To send Email
        /// </summary>
        /// <param name="senderName"></param>
        /// <param name="senderAddress"></param>
        /// <param name="recipients"></param>
        /// <param name="subject"></param>
        /// <param name="body"></param>
        /// <param name="attachments"></param>
        public void SendEmail(string senderName, string senderAddress, string recipients, string subject, string body, List<string> attachments)
        {
            var smtpClient = (SmtpClient)this.MailClient;
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(senderName, senderAddress));
            message.To.AddRange(recipients.Split(',', ';').Select(x => new MailboxAddress(x.Trim())).ToList());
            message.Subject = subject;

            var builder = new BodyBuilder();
            builder.HtmlBody = body;
            //attachments.ForEach(x => builder.Attachments.Add(x));
            message.Body = builder.ToMessageBody();

            smtpClient.Send(message);
        }

        /// <summary>
        /// To list the Emails
        /// </summary>
        /// <param name="searchQuery"></param>
        /// <param name="searchFolder"></param>
        /// <returns></returns>
        public IList<UniqueId> ListEmails(SearchQuery searchQuery, string searchFolder)
        {
            var imapClient = (ImapClient)this.MailClient;
            var folder = imapClient.GetFolder(searchFolder);
            folder.Open(FolderAccess.ReadOnly);
            return folder.Search(searchQuery);
        }

        /// <summary>
        /// To Download one Attached
        /// </summary>
        /// <param name="emailUid"></param>
        /// <param name="localPath"></param>
        /// <returns></returns>
        public string DownloadFirstAttachment(UniqueId emailUid, string localPath)
        {
            var imapClient = (ImapClient)this.MailClient;
            MimeMessage message = imapClient.Inbox.GetMessage(emailUid);

            if (message.Attachments.Count() > 0)
            {
                var attachment = message.Attachments.ElementAt(0);
                var fileName = attachment.ContentDisposition?.FileName ?? attachment.ContentType.Name;

                using (var fileStream = File.Create(Path.Combine(localPath, fileName)))
                {
                    if (attachment is MessagePart)
                    {
                        var rfc822 = (MessagePart)attachment;

                        rfc822.Message.WriteTo(fileStream);
                    }
                    else
                    {
                        var part = (MimePart)attachment;

                        part.Content.DecodeTo(fileStream);
                    }
                }

                return Path.Combine(localPath, fileName);
            }

            throw new ImapCommandException(ImapCommandResponse.No, "Email doesn't have attachments");
        }

        /// <summary>
        /// to Mark the Message
        /// </summary>
        /// <param name="emailId"></param>
        /// <param name="folderName"></param>
        public void MarkMessageAsRead(UniqueId emailId, string folderName)
        {
            var imapClient = (ImapClient)this.MailClient;
            var folder = imapClient.GetFolder(folderName);
            folder.Open(FolderAccess.ReadWrite);

            folder.AddFlags(emailId, MessageFlags.Seen, true);
        }
        #endregion

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    MailClient.Dispose();
                }

                disposedValue = true;
            }
        }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
        }
        #endregion
    }
}
