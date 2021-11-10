using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Shannan.Core.Debugging;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Text;
using System.Web;
using System.Windows;

namespace Shannan.Core.Framework
{
    public enum DataRequestMethod
    {
        GET,
        POST
    }

    public class DataRequestEventArgs : EventArgs
    {
        public DataRequestEventArgs(DataRequest request, JObject data = null, Exception e = null)
        {
            Request = request;
            Data = data;
            Exception = e;
        }

        public DataRequest Request
        {
            get;
            private set;
        }

        public JObject Data
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

    public struct FormData
    {
        public string FileName;
        public Stream Stream;
    }

    public class DataRequest : IDisposable
    {
        protected string _uri;
        protected NameValueCollection _param;
        protected Dictionary<string, FormData> _uploadFiles;
        protected Stream _postStream;

        private HttpWebRequest _request;
        private HttpWebResponse _response;
        private Stream _responseStream;
        private MemoryStream _ms;
        private byte[] _buffer;

        public delegate void DataRequestEventHandler(DataRequestEventArgs e);

        public event DataRequestEventHandler Completed;

        static DataRequest()
        {
            if (ServicePointManager.DefaultConnectionLimit < 16)
            {
                ServicePointManager.DefaultConnectionLimit = 16;
            }
            ServicePointManager.Expect100Continue = false;
            ServicePointManager.DnsRefreshTimeout = 0;
        }

        private DataRequest(string uri)
        {
            _uri = uri;
            _buffer = new byte[1024 * 64];
            _ms = new MemoryStream(1024 * 64);
            _uploadFiles = new Dictionary<string, FormData>();
            Method = DataRequestMethod.POST;
        }

        public DataRequest(string uri, NameValueCollection param) : this(uri)
        {
            _param = param == null ? new NameValueCollection() : param;
        }

        public DataRequest(string uri, string param) : this(uri)
        {
            _param = HttpUtility.ParseQueryString(param);
        }

        public DataRequest(string uri, params object[] nameValuePairs) : this(uri, (NameValueCollection)null)
        {
            if (nameValuePairs.Length / 2 * 2 != nameValuePairs.Length)
            {
                throw new ArgumentException("参数数量错误");
            }

            for (int i = 0; i < nameValuePairs.Length; i += 2)
            {
                _param.Add(nameValuePairs[i].ToString(), nameValuePairs[i + 1].ToString());
            }
        }

        public DataRequestMethod Method { get; set; }

        public string Token { get; set; } = string.Empty;

        public string CallerName { get; set; } = string.Empty;

        protected string NormalizeUri(string uri, DataRequestMethod method)
        {
            string s = uri.IndexOf("http://", StringComparison.CurrentCultureIgnoreCase) == -1 && uri.IndexOf("https://", StringComparison.CurrentCultureIgnoreCase) == -1 ? Session.Instance.BaseUri + _uri : _uri;

            if (method == DataRequestMethod.POST)
            {
                return s;
            }

            Uri u = new Uri(s);
            NameValueCollection param = new NameValueCollection();
            param.Add(_param);
            param.Add(HttpUtility.ParseQueryString(u.Query));
            param.Add(Session.Instance.Param);

            StringBuilder sb = new StringBuilder();
            foreach (string name in param.AllKeys)
            {
                if (sb.Length > 0)
                {
                    sb.Append("&");
                }
                sb.Append(name);
                sb.Append("=");
                sb.Append(param.Get(name));
            }

            UriBuilder ub = new UriBuilder(u);
            ub.Query = sb.ToString();

            return ub.Uri.ToString();
        }

        public virtual void SetPostStream(string filename)
        {
            if (Method != DataRequestMethod.POST)
            {
                throw new InvalidOperationException("请求方法为GET时无法设置POST流");
            }
            _postStream = new FileStream(filename, FileMode.Open, FileAccess.Read);
        }

        public virtual void SetPostStream(Stream postStream)
        {
            if (Method != DataRequestMethod.POST)
            {
                throw new InvalidOperationException("请求方法为GET时无法设置POST流");
            }
            _postStream = postStream;
        }

        public void AddUploadFile(string name, string filename)
        {
            if (Method != DataRequestMethod.POST)
            {
                throw new InvalidOperationException("请求方法为GET时无法上载文件");
            }

            FormData file;
            file.Stream = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            file.FileName = filename;

            _uploadFiles.Add(name, file);
        }

        public void AddUploadFile(string name, string filename, Stream stream)
        {
            if (Method != DataRequestMethod.POST)
            {
                throw new InvalidOperationException("请求方法为GET时无法上载文件");
            }

            FormData file;
            file.Stream = stream;
            file.FileName = filename;

            _uploadFiles.Add(name, file);
        }

        public virtual bool Start()
        {
            _uri = NormalizeUri(_uri, Method);
            _request = CreateRequest();
            if (_request == null)
            {
                return false;
            }

            if (Method == DataRequestMethod.POST)
            {
                _request.BeginGetRequestStream(RequestCallback, this);
            }
            else
            {
                _request.BeginGetResponse(ResponseCallback, this);
            }
            ConsoleManager.Log("{0}: {1} Request Start", CallerName, Token);
            return true;
        }

        public bool Abort()
        {
            Clean();
            ConsoleManager.Log("{0}: {1} Request Abort", CallerName, Token);
            return true;
        }

        private string Boundary
        {
            get
            {
                return "-----------" + "AaB03x";
            }
        }

        private HttpWebRequest CreateRequest()
        {
            GC.Collect();
            HttpWebRequest request = WebRequest.Create(_uri) as HttpWebRequest;
            if (request == null)
            {
                return null;
            }

            request.Method = Method == DataRequestMethod.POST ? "POST" : "GET";
            request.Timeout = 30 * 1000;
            request.Headers.Add(HttpRequestHeader.AcceptEncoding, "gzip,deflate");
            request.Headers.Add("Pragma", "no-cache");
            request.Headers.Add("Cache-Control", "no-cache");

            if (Method == DataRequestMethod.POST)
            {
                if (_uploadFiles.Count == 0)
                {
                    request.ContentType = "application/x-www-form-urlencoded";
                }
                else
                {
                    request.ContentType = "multipart/form-data; boundary=" + Boundary;
                }
            }
            request.Accept = "text/html, */*";
            request.CookieContainer = Session.Instance.Cookies;

            request.UserAgent = Session.Instance.UserAgent;

            request.AllowAutoRedirect = true;
            request.KeepAlive = false;

            return request;
        }

        protected void Clean()
        {
            if (_postStream != null)
            {
                try
                {
                    _postStream.Dispose();
                }
                catch (Exception ex)
                {
                    ex.Data.Add("_postStream", "");
                    Logger.Instance.ReportException(ex);
                }
                _postStream = null;
            }
            if (_ms != null)
            {
                try
                {
                    _ms.Dispose();
                }
                catch (Exception ex)
                {
                    ex.Data.Add("_ms", "");
                    Logger.Instance.ReportException(ex);
                }
                _ms = null;
            }
            if (_responseStream != null)
            {
                try
                {
                    _responseStream.Dispose();
                }
                catch (Exception ex)
                {
                    ex.Data.Add("_responseStream", "");
                    Logger.Instance.ReportException(ex);
                }
                _responseStream = null;
            }
            if (_response != null)
            {
                try
                {
                    _response.Dispose();
                }
                catch (Exception ex)
                {
                    ex.Data.Add("_response", "");
                    Logger.Instance.ReportException(ex);
                }
                _response = null;
            }
            _request = null;
        }

        protected virtual void RaiseCompletedEvent(JObject data, Exception e)
        {
            ConsoleManager.Log("{0}: {1} Request Completed", CallerName, Token);
            Application.Current.Dispatcher.BeginInvoke(new Action(delegate
            {
                Completed?.Invoke(new DataRequestEventArgs(this, data, e));
            }));
        }

        private void RequestCallback(IAsyncResult asyncResult)
        {
            if (_request == null)
            {
                return;
            }
            try
            {
                if (Method == DataRequestMethod.POST)
                {
                    Stream ps = _request.EndGetRequestStream(asyncResult);

                    if (_postStream != null)
                    {
                        _postStream.CopyTo(ps);
                        _postStream.Dispose();
                        _postStream = null;
                    }
                    else if (_uploadFiles.Count > 0)
                    {
                        NameValueCollection param = new NameValueCollection();
                        param.Add(_param);
                        param.Add(Session.Instance.Param);

                        foreach (string key in param.AllKeys)
                        {
                            string value = string.Format("--{0}\r\nContent-Disposition: form-data; name=\"{1}\"\r\n\r\n{2}\r\n", Boundary, key, param.Get(key));
                            byte[] buffer = Encoding.UTF8.GetBytes(value);
                            ps.Write(buffer, 0, buffer.Length);
                        }

                        foreach (string name in _uploadFiles.Keys)
                        {
                            string value = string.Format("--{0}\r\nContent-Disposition: form-data; name=\"{1}\"; filename=\"{2}\"\r\nContent-Type: application/octet-stream\r\n\r\n", Boundary, name, _uploadFiles[name].FileName);
                            byte[] header = Encoding.UTF8.GetBytes(value);
                            ps.Write(header, 0, header.Length);

                            _uploadFiles[name].Stream.CopyTo(ps);
                            _uploadFiles[name].Stream.Dispose();

                            byte[] buffer = Encoding.UTF8.GetBytes("\r\n");
                            ps.Write(buffer, 0, buffer.Length);
                        }

                        byte[] trailer = Encoding.ASCII.GetBytes("--" + Boundary + "--\r\n");
                        ps.Write(trailer, 0, trailer.Length);
                    }
                    else
                    {
                        NameValueCollection param = new NameValueCollection();
                        param.Add(_param);
                        param.Add(Session.Instance.Param);

                        if (param.Count > 0)
                        {
                            StringBuilder sb = new StringBuilder();
                            foreach (string key in param.AllKeys)
                            {
                                if (sb.Length > 0)
                                    sb.Append('&');
                                sb.Append(Uri.EscapeDataString(key).Replace("%20", "+"));
                                sb.Append('=');
                                sb.Append(Uri.EscapeDataString(param.Get(key)).Replace("%20", "+"));
                            }
                            byte[] buffer = Encoding.UTF8.GetBytes(sb.ToString());

                            ps.Write(buffer, 0, buffer.Length);
                        }
                    }
                    ps.Dispose();
                }

                _request.BeginGetResponse(ResponseCallback, this);
            }
            catch (Exception ex)
            {
                ex.Data.Add("CallerName", CallerName);
                Logger.Instance.ReportException(ex);
                RaiseCompletedEvent(null, new Exception("网络请求失败，请检查您的网络设置"));
                Clean();
            }
        }

        private void ResponseCallback(IAsyncResult asyncResult)
        {
            if (_request == null)
            {
                return;
            }
            try
            {
                _response = _request.EndGetResponse(asyncResult) as HttpWebResponse;

                string contentEncoding = _response.Headers["Content-Encoding"] == null ? string.Empty : _response.Headers["Content-Encoding"].ToLower();
                if (contentEncoding.Contains("gzip"))
                {
                    _responseStream = new GZipStream(_response.GetResponseStream(), CompressionMode.Decompress);
                }
                else if (contentEncoding.Contains("deflate"))
                {
                    _responseStream = new DeflateStream(_response.GetResponseStream(), CompressionMode.Decompress);
                }
                else
                {
                    _responseStream = _response.GetResponseStream();
                }

                _responseStream.BeginRead(_buffer, 0, _buffer.Length, ReadCallback, this);
            }
            catch (Exception)
            {
                RaiseCompletedEvent(null, new Exception("网络响应出错，请检查您的网络设置"));
                Clean();
            }
        }

        private void ReadCallback(IAsyncResult asyncResult)
        {
            if (_ms == null)
            {
                return;
            }
            try
            {
                int read = _responseStream.EndRead(asyncResult);
                if (read == 0)
                {
                    JObject value = null;
                    _ms.Seek(0, SeekOrigin.Begin);
                    using (StreamReader reader = new StreamReader(_ms, Encoding.UTF8))
                    {
                        value = (JObject)JToken.ReadFrom(new JsonTextReader(reader));
                    }

                    JObject data = null;
                    Exception e = null;
                    int errcode = int.Parse(value["errcode"].ToString());
                    if (errcode != 0)
                    {
                        e = new Exception(value["msg"].ToString());
                        e.Data["errcode"] = errcode;
                    }
                    else
                    {
                        data = (JObject)value["data"];
                    }
                    RaiseCompletedEvent(data, e);

                    Clean();
                }
                else if (read > 0)
                {
                    _ms.Write(_buffer, 0, read);
                    _responseStream.BeginRead(_buffer, 0, _buffer.Length, ReadCallback, this);
                }
                else
                {
                    RaiseCompletedEvent(null, new Exception("未知的网络错误"));
                    Clean();
                }
            }
            catch (Exception)
            {
                RaiseCompletedEvent(null, new Exception("网络连接不畅，请检查您的网络设置"));
                Clean();
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                Clean();
            }
        }
    }
}
