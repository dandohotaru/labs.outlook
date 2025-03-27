using System;
using System.Collections.Generic;

namespace Demo.App.Agents
{
    public class EmailModel
    {
        public string Subject { get; set; }

        public string Sender { get; set; }

        public IEnumerable<string> Recipients { get; set; }

        public DateTime? Sent { get; set; }

        public string Body { get; set; }
    }
}
