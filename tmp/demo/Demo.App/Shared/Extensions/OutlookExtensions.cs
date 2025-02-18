using Demo.App.Agents;
using Microsoft.Office.Interop.Outlook;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Demo.App.Shared.Extensions;

public static class OutlookExtensions
{
    public static IEnumerable<MailItem> Conversation(this MailItem mailItem)
    {
        var emails = new List<MailItem>();

        var conversation = mailItem.GetConversation();
        if (conversation == null)
        {
            return emails;
        }

        var entries = conversation.GetTable().AsEnumerable();

        foreach (var entry in entries)
        {
            if (entry["EntryID"] is string id)
            {
                var item = mailItem.Application.Session.GetItemFromID(id);
                if (item is MailItem email)
                {
                    emails.Add(email);
                }
            }
        }

        return emails;
    }

    public static ThreadModel Map(this IEnumerable<MailItem> emails)
    {
        var results = new List<EmailModel>();
        foreach (var email in emails)
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

        return new ThreadModel
        {
            Emails = results
        };
    }

    public static IEnumerable<EmailModel> Map1(this IEnumerable<MailItem> emails)
    {
        foreach (var email in emails)
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
            yield return result;
        }
    }

    public static string Draft(this MailItem email)
    {
        if (email == null || string.IsNullOrWhiteSpace(email.Body))
            return string.Empty;

        string[] separators =
        {
            "From:",
            "Sent:",
            "-----Original Message-----",
            "On "
        };

        var body = email.Body;

        foreach (var separator in separators)
        {
            int index = body.IndexOf(separator, StringComparison.OrdinalIgnoreCase);
            if (index > 0)
            {
                return body.Substring(0, index).Trim();
            }
        }

        return body.Trim();
    }

    private static IEnumerable<Row> AsEnumerable(this Table table)
    {
        for (Row row = table.GetNextRow(); row != null; row = table.GetNextRow())
        {
            yield return row;
        }
    }
}
