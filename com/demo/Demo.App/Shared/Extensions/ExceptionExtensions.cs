using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo.App.Shared.Extensions
{
    public static class ExceptionExtensions
    {
        public static string ToMessage(this Exception exception)
        {
            if (exception == null)
                return string.Empty;

            var builder = new StringBuilder();
            var current = exception;

            while (current != null)
            {
                builder.AppendLine(current.Message);
                current = current.InnerException;
            }

            return builder.ToString();
        }
    }
}
