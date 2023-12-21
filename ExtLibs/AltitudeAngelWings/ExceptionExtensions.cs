using System;
using System.Text;
using Flurl.Http;

namespace AltitudeAngelWings
{
    public static class ExceptionExtensions
    {
        public static string ToDisplayedException(this Exception exception)
        {
            var builder = new StringBuilder();
            if (exception is AggregateException aggregate)
            {
                foreach (var inner in aggregate.InnerExceptions)
                {
                    FormatException(builder, inner);
                }
            }
            else
            {
                FormatException(builder, exception);
            }
            return builder.ToString();
        }

        private static void FormatException(StringBuilder builder, Exception ex)
        {
            var message = $"{ex.GetType().Name}: {ex.Message}";
            switch (ex)
            {
                case FlurlHttpException exception:
                    var response = exception.GetResponseStringAsync().GetAwaiter().GetResult();
                    builder.AppendLine($"{message}: {response}");
                    break;

                default:
                    builder.AppendLine(message);
                    break;
            }

            if (ex.InnerException != null)
            {
                builder.AppendLine();
                FormatException(builder, ex.InnerException);
            }
        }
    }
}