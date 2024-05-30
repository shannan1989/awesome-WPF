using Sentry;
using Shannan.Core.Framework;
using System;

namespace Shannan.Core.Debugging
{
    public class Logger
    {
        static Logger()
        {
            Instance = new Logger();
        }

        private Logger()
        {
        }

        public static Logger Instance
        {
            get;
            private set;
        }

        public void Init(string dsn)
        {
            SentrySdk.Init(o =>
            {
                o.Dsn = dsn;
                o.SendDefaultPii = true;
                o.AutoSessionTracking = true;
                o.IsGlobalModeEnabled = true; // Enabling this option is recommended for client applications only. It ensures all threads use the same global scope.
                o.Release = Client.Instance.AppName + "@" + Client.Instance.AppVersion;
                o.Environment = "release";
                o.TracesSampleRate = 0.2;
#if DEBUG
                o.Environment = "debug";
                o.TracesSampleRate = 1.0;
#endif
            });
        }

        public void ReportException(Exception e)
        {
            SentrySdk.CaptureException(e);
        }

        public void ReportMessage(string message)
        {
            SentrySdk.CaptureMessage(message);
        }
    }
}
