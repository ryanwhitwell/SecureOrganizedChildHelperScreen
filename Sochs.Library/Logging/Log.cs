using System.Diagnostics;
using System.Reflection;

namespace Sochs.Library.Logging
{
	public static class Log
	{
		private const string LoggingFormat = "{0:yyyy-MM-ddTHH:mm:ss.fff} - {1} - {2}\n";
		private const string LoggingExceptionFormat = "{0:yyyy-MM-ddTHH:mm:ss.fff} - {1} - {2}\n{3}";

		private const string DebugSeverity = "DEBUG";
		private const string InfoSeverity = "INFO";
		private const string WarnSeverity = "WARN";
		private const string ErrorSeverity = "ERROR";

		public static void Debug(string message, Exception? ex = null)
		{
			if (ex == null)
			{
				Console.Write(string.Format(LoggingFormat, DateTime.Now, DebugSeverity, message));
			}
			else
			{
				Console.Write(string.Format(LoggingExceptionFormat, DateTime.Now, DebugSeverity, message, ex.GetExceptionDetails()));
			}
		}

		public static void Info(string message, Exception? ex = null)
		{
			if (ex == null)
			{
				Console.Write(string.Format(LoggingFormat, DateTime.Now, InfoSeverity, message));
			}
			else
			{
				Console.Write(string.Format(LoggingExceptionFormat, DateTime.Now, InfoSeverity, message, ex.GetExceptionDetails()));
			}
		}

		public static void Warning(string message, Exception? ex = null)
		{
			if (ex == null)
			{
				Trace.Write(string.Format(LoggingFormat, DateTime.Now, WarnSeverity, message));
			}
			else
			{
				Trace.Write(string.Format(LoggingExceptionFormat, DateTime.Now, WarnSeverity, message, ex.GetExceptionDetails()));
			}
		}

		public static void Error(string message, Exception? ex = null)
		{
			if (ex == null)
			{
				Trace.Write(string.Format(LoggingFormat, DateTime.Now, ErrorSeverity, message));
			}
			else
			{
				Trace.Write(string.Format(LoggingExceptionFormat, DateTime.Now, ErrorSeverity, message, ex.GetExceptionDetails()));
			}
		}

		private static string GetExceptionDetails(this Exception exception)
		{
			_ = exception ?? throw new ArgumentNullException(nameof(exception));

			PropertyInfo[] properties = exception.GetType().GetProperties();
			List<string> fields = new();

			foreach (PropertyInfo property in properties)
			{
				object? value = property.GetValue(exception, null);

				if (value != null)
				{
					fields.Add(string.Format("    {0} = {1}", property.Name, value.ToString()));
				}
			}

			return string.Join("\n", fields.ToArray());
		}
	}
}
