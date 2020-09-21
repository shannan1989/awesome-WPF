using Newtonsoft.Json.Linq;
using System;
using System.Collections.Specialized;

namespace Shannan.Core.Framework
{
    public enum DataRequestMethod
    {
        GET,
        POST
    }

    public class RequestCompletedEventArgs : EventArgs
    {
        public RequestCompletedEventArgs(DataRequest request, JObject value = null, Exception e = null)
        {
            Request = request;
            Value = value;
            Exception = e;
        }

        public DataRequest Request
        {
            get;
            private set;
        }

        public JObject Value
        {
            get;
            private set;
        }

        public Exception Exception
        {
            get;
            private set;
        }
    }

    public class DataRequest : IDisposable
    {
        public delegate void RequestCompletedEventHandler(object sender, RequestCompletedEventArgs e);

        public event RequestCompletedEventHandler RequestCompleted;

        public DataRequestMethod Method
        {
            get;
            set;
        }

        private DataRequest(string uri)
        {
        }

        public DataRequest(string uri, NameValueCollection parameters) : this(uri)
        {
        }

        public void Start()
        {
        }

        public void Dispose()
        {
        }
    }
}
