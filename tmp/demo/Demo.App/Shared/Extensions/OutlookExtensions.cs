using Demo.App.Agents;
using Microsoft.Office.Interop.Outlook;
using System.Collections.Generic;
using System.Linq;

namespace Demo.App.Shared.Extensions
{
    public static class OutlookExtensions
    {
        public static ThreadModel ToThread(this MailItem mailItem)
        {
            List<EmailModel> results = [];

            var conversation = mailItem.GetConversation();
            if (conversation == null)
            {
                return new ThreadModel
                {
                    Emails = results
                };
            }

            var conversationTable = conversation.GetTable();

            foreach (var row in conversationTable.AsEnumerable())
            {
                if (row["EntryID"] is string id)
                {
                    var item = mailItem.Application.Session.GetItemFromID(id);
                    if (item is MailItem email)
                    {
                        var result = new EmailModel
                        {
                            Sender = email.SenderName,
                            Subject = email.Subject,
                            Recipients = email.Recipients
                                .Cast<Recipient>()
                                .Select(p => p.Address)
                                .ToList(),
                            Sent = email.SentOn,
                            Body = email.Body
                        };
                        results.Add(result);
                    }
                }
            }

            return new ThreadModel
            {
                Emails = results
            };
        }

        public static IEnumerable<Row> AsEnumerable(this Table table)
        {
            for (Row row = table.GetNextRow(); row != null; row = table.GetNextRow())
            {
                yield return row;
            }
        }
    }
}
