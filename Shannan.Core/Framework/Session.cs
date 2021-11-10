using System.Collections.Specialized;
using System.Net;

namespace Shannan.Core.Framework
{
    public class Session
    {
        private NameValueCollection _param;

        static Session()
        {
            Instance = new Session();
        }

        public static Session Instance
        {
            get;
            private set;
        }

        private Session()
        {
            Init();
        }

        public void Init()
        {
            Cookies = new CookieContainer();
            BaseUri = string.Empty;
            _param = new NameValueCollection();
        }

        public string BaseUri
        {
            get;
            set;
        }

        public string UserAgent { get; set; } = string.Empty;

        public CookieContainer Cookies
        {
            get;
            private set;
        }

        public NameValueCollection Param
        {
            get
            {
                NameValueCollection param = new NameValueCollection();
                param.Add(_param);

                return param;
            }
            private set
            {
                _param = value;
            }
        }

        public void AddPersistedParam(string name, string value)
        {
            _param.Add(name, value);
        }

        public void RemovePersistedParam(string name)
        {
            _param.Remove(name);
        }

        public void ClearPersistedParam()
        {
            _param.Clear();
        }
    }
}
