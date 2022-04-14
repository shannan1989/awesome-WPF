using Shannan.Core.Framework;
using SharpRaven;
using SharpRaven.Data;
using System;
using System.Collections.Generic;

namespace Shannan.Core.Debugging
{
    public class Logger
    {
        private RavenClient _ravenClient;

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

        public void InitRavenClient(string dsn)
        {
            _ravenClient = new RavenClient(dsn);
            _ravenClient.Release = Client.Instance.AppName + "@" + Client.Instance.AppVersion;
#if DEBUG
            _ravenClient.Environment = "development";
#else
            _ravenClient.Environment = "production";
#endif
        }

        public void ReportException(Exception e)
        {
            if (_ravenClient == null)
            {
                return;
            }
            try
            {
                _ravenClient.Capture(new SentryEvent(e));
            }
            catch (Exception)
            { }
        }

        public void ReportException(Exception e, Dictionary<string, string> tags)
        {
            if (_ravenClient == null)
            {
                return;
            }
            try
            {
                _ravenClient.Capture(new SentryEvent(e) { Tags = tags });
            }
            catch (Exception)
            { }
        }
    }
}
