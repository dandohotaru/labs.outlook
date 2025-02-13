using System.Collections.Generic;

namespace Demo.App.Agents
{
    public class DraftModel
    {
        public string Sender { get; set; }

        public IEnumerable<string> Recipients { get; set; }

        public string Body { get; set; }
    }
}
