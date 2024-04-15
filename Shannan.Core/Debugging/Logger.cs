using System;
using System.Collections.Generic;

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

        public void ReportException(Exception e)
        {
        }

        public void ReportException(Exception e, Dictionary<string, string> tags)
        {
        }
    }
}
