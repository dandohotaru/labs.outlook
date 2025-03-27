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
                Sender = email.SenderName ?? email.Sender?.Name ?? email.SenderEmailAddress,
                Subject = email.Subject,
                Recipients = email.Recipients
                    .Cast<Recipient>()
                    .Select(p => p.Name ?? p.Address)
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
                Sender = email.SenderName ?? email.Sender?.Name ?? email.SenderEmailAddress,
                Subject = email.Subject,
                Recipients = email.Recipients
                    .Cast<Recipient>()
                    .Select(p => p.Name ?? p.Address)
                    .ToList(),
                Sent = email.SentOn,
                Body = email.Body
            };
            yield return result;
        }
    }

    public static DraftModel Draft(this MailItem email, Recipient sender)
    {
        if (email == null)
        {
            throw new ArgumentNullException(nameof(email));
        }

        string body = email.Body;

        // patterns
        string[] replyPatterns =
        {
            "From:", // Common starting point for forwarded/replied emails
            "Sent:", // Sent header in replies
            "To:", // Often found in the body of a forwarded email
            "Subject:", // Also found in forwarded/replied emails
            "----- Forwarded message -----", // Common in forwarded emails
            "On " // For replies, starting with "On [date], [sender] wrote:"
        };

        // extract
        foreach (var pattern in replyPatterns)
        {
            int index = body.IndexOf(pattern, StringComparison.OrdinalIgnoreCase);
            if (index >= 0)
            {
                // Extract the body before the quoted text
                body = body.Substring(0, index).Trim();
                break;
            }
        }

        return string.IsNullOrWhiteSpace(body)
            ? null
            : new DraftModel
            {
                Subject = email.Subject,
                Sender = sender?.Name ?? email.SenderName ?? email.Sender?.Name ?? email.SenderEmailAddress,
                Recipients = email.Recipients
                    .Cast<Recipient>()
                    .Select(p => p.Name ?? p.Address)
                    .ToArray(),
                Body = body
            };
    }

    private static IEnumerable<Row> AsEnumerable(this Table table)
    {
        for (Row row = table.GetNextRow(); row != null; row = table.GetNextRow())
        {
            yield return row;
        }
    }
}
