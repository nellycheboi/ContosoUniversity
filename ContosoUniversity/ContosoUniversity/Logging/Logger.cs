using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Web;

namespace ContosoUniversity.Logging
{
    /// <summary>
    /// The implementation uses System.Diagnostics to do the tracing. This is a built-in feature of .NET which makes it easy to generate and use tracing information. 
    /// There are many "listeners" you can use with System.Diagnostics tracing, to write logs to files, for example, or to write them to blob storage in Azure.
    /// See some of the options, https://docs.microsoft.com/azure/app-service-web/web-sites-dotnet-troubleshoot-visual-studio
    /// In a production application you might want to consider tracing packages other than System.Diagnostics, and the ILogger interface makes it relatively easy to switch to a different tracing mechanism if you decide to do that.
    /// </summary>
    public class Logger : ILogger
    {
        public void Information(string message)
        {
            Trace.TraceInformation(message);
        }

        public void Information(string fmt, params object[] vars)
        {
            Trace.TraceInformation(fmt, vars);
        }

        public void Information(Exception exception, string fmt, params object[] vars)
        {
            Trace.TraceInformation(FormatExceptionMessage(exception, fmt, vars));
        }

        public void Warning(string message)
        {
            Trace.TraceWarning(message);
        }

        public void Warning(string fmt, params object[] vars)
        {
            Trace.TraceWarning(fmt, vars);
        }

        public void Warning(Exception exception, string fmt, params object[] vars)
        {
            Trace.TraceWarning(FormatExceptionMessage(exception, fmt, vars));
        }

        public void Error(string message)
        {
            Trace.TraceError(message);
        }

        public void Error(string fmt, params object[] vars)
        {
            Trace.TraceError(fmt, vars);
        }

        public void Error(Exception exception, string fmt, params object[] vars)
        {
            Trace.TraceError(FormatExceptionMessage(exception, fmt, vars));
        }

        public void TraceApi(string componentName, string method, TimeSpan timespan)
        {
            TraceApi(componentName, method, timespan, "");
        }

        public void TraceApi(string componentName, string method, TimeSpan timespan, string fmt, params object[] vars)
        {
            TraceApi(componentName, method, timespan, string.Format(fmt, vars));
        }
        public void TraceApi(string componentName, string method, TimeSpan timespan, string properties)
        {
            string message = String.Concat("Component:", componentName, ";Method:", method, ";Timespan:", timespan.ToString(), ";Properties:", properties);
            Trace.TraceInformation(message);
        }

        private static string FormatExceptionMessage(Exception exception, string fmt, object[] vars)
        {
            // Simple exception formatting: for a more comprehensive version see 
            // http://code.msdn.microsoft.com/windowsazure/Fix-It-app-for-Building-cdd80df4
            var sb = new StringBuilder();
            sb.Append(string.Format(fmt, vars));
            sb.Append(" Exception: ");
            sb.Append(exception.ToString());
            return sb.ToString();
        }
    }
}