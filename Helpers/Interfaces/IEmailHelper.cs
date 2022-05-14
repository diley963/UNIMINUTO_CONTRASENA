using MailKit;
using MailKit.Search;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Helpers.Interfaces
{
    public interface IEmailHelper
    {
        /// <summary>
        /// To Send Email
        /// </summary>
        /// <param name="senderName"></param>
        /// <param name="senderAddress"></param>
        /// <param name="recipients"></param>
        /// <param name="subject"></param>
        /// <param name="body"></param>
        /// <param name="attachments"></param>
        void SendEmail(string senderName, string senderAddress, string recipients, string subject, string body, List<string> attachments);

        /// <summary>
        /// To List the Emails
        /// </summary>
        /// <param name="searchQuery"></param>
        /// <param name="searchFolder"></param>
        /// <returns></returns>
        IList<UniqueId> ListEmails(SearchQuery searchQuery, string searchFolder);

        /// <summary>
        /// To Download one Attached
        /// </summary>
        /// <param name="emailUid"></param>
        /// <param name="localPath"></param>
        /// <returns></returns>
        string DownloadFirstAttachment(UniqueId emailUid, string localPath);

        /// <summary>
        /// To marker the message to readed
        /// </summary>
        /// <param name="emailId"></param>
        /// <param name="folderName"></param>
        void MarkMessageAsRead(UniqueId emailId, string folderName);
    }
}
