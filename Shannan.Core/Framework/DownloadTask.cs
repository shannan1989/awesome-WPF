using Shannan.Core.Debugging;
using System;
using System.Collections.Specialized;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Text;
using System.Web;
using System.Windows;

namespace Shannan.Core.Framework
{
    public enum DownloadStatus : int
    {
        Pending = 1,
        Running = 2,
        Finished = 3,
        Aborted = 4
    };

    public class DownloadTaskEventArgs : EventArgs
    {
        public DownloadTaskEventArgs(DownloadTask task, DownloadStatus status)
        {
            Task = task;
            Status = status;
        }

        public DownloadTask Task
        {
            get;
            private set;
        }

        public DownloadStatus Status
        {
            get;
            private set;
        }
    }

    public class DownloadTask : IDisposable
    {
        private const string Extension = ".download";
        protected string _uri;
        protected string _filename;
        protected NameValueCollection _param;

        private FileStream _writer;
        private HttpWebRequest _request;
        private HttpWebResponse _response;
        private Stream _responseStream;
        private byte[] _buffer;
        private bool _acceptRanges;
        private long _chunkSize;

        public delegate void DownloadTaskEventHandler(DownloadTaskEventArgs e);

        public event DownloadTaskEventHandler DownloadUpdated;

        static DownloadTask()
        {
            if (ServicePointManager.DefaultConnectionLimit < 16)
            {
                ServicePointManager.DefaultConnectionLimit = 16;
            }
            ServicePointManager.Expect100Continue = false;
        }

        private DownloadTask(string filename, string uri)
        {
            _filename = filename;
            _uri = uri;
            _param = new NameValueCollection();

            Status = DownloadStatus.Pending;
            NumberOfDownload = 0;
            NumberOfTotal = 0;

            _buffer = new byte[1024 * 64];
            _acceptRanges = false;
            _chunkSize = 0;
        }

        public DownloadTask(string filename, string uri, NameValueCollection param) : this(filename, uri)
        {
            if (param != null)
            {
                _param = param;
            }
        }

        public DownloadTask(string filename, string uri, params object[] nameValuePairs) : this(filename, uri)
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

        public string FileName
        { get { return _filename; } }

        public string TempFileName
        { get { return FileName + Extension; } }

        public DownloadStatus Status
        {
            get;
            private set;
        }

        public long NumberOfTotal
        {
            get;
            private set;
        }

        public long NumberOfDownload
        {
            get;
            private set;
        }

        public string CallerName { get; set; } = string.Empty;

        public string Token { get; set; } = string.Empty;

        public bool Start()
        {
            try
            {
                _writer = new FileStream(TempFileName, FileMode.Create, FileAccess.Write);
            }
            catch (Exception)
            {
                return false;
            }

            _request = CreateRequest();
            if (_request == null)
            {
                return false;
            }

            Status = DownloadStatus.Running;
            _request.BeginGetResponse(ResponseCallback, this);

            ConsoleManager.Log("{0}: {1} DownloadTask Start", CallerName, Token);
            return true;
        }

        public bool Resume()
        {
            if (Status == DownloadStatus.Finished || Status == DownloadStatus.Running)
            {
                return false;
            }

            if (_acceptRanges == false)
            {
                _writer = new FileStream(TempFileName, FileMode.Create, FileAccess.Write);
            }
            else
            {
                _writer = new FileStream(TempFileName, FileMode.Open, FileAccess.Write);
            }
            NumberOfDownload = _writer.Seek(0, SeekOrigin.End);

            _request = CreateRequest();
            if (_request == null)
            {
                Clean();
                return false;
            }

            Status = DownloadStatus.Running;
            _request.BeginGetResponse(ResponseCallback, this);
            return true;
        }

        public bool Pause()
        {
            if (Status != DownloadStatus.Running)
            {
                return false;
            }

            Status = DownloadStatus.Pending;
            Clean();

            return true;
        }

        public bool Abort()
        {
            if (Status != DownloadStatus.Running)
            {
                return false;
            }

            Status = DownloadStatus.Aborted;
            Clean();

            return true;
        }

        private HttpWebRequest CreateRequest()
        {
            string u = _uri.IndexOf("http://", StringComparison.CurrentCultureIgnoreCase) == -1 && _uri.IndexOf("https://", StringComparison.CurrentCultureIgnoreCase) == -1 ? Session.Instance.BaseUri + _uri : _uri;

            Uri uri = new Uri(u);
            NameValueCollection param = new NameValueCollection();
            param.Add(HttpUtility.ParseQueryString(uri.Query));
            param.Add(_param);
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

            HttpWebRequest request = WebRequest.Create(ub.Uri.ToString()) as HttpWebRequest;
            if (request == null)
            {
                return null;
            }

            request.Method = "GET";
            request.Timeout = 60 * 1000;
            request.Headers.Add(HttpRequestHeader.AcceptEncoding, "gzip,deflate");
            request.Headers.Add("Pragma", "no-cache");
            request.Headers.Add("Cache-Control", "no-cache");

            // 断点续传
            if (_acceptRanges == true && NumberOfDownload > 0)
            {
                request.Headers.Add("Ranges", NumberOfDownload.ToString());
            }

            request.Accept = "text/html, */*";
            request.UserAgent = Session.Instance.UserAgent;
            request.CookieContainer = Session.Instance.Cookies;
            request.AllowAutoRedirect = true;
            request.KeepAlive = false;

            return request;
        }

        private void Clean()
        {
            if (_writer != null)
            {
                _writer.Dispose();
                _writer = null;

                if (Status == DownloadStatus.Finished)
                {
                    if (File.Exists(_filename))
                    {
                        File.SetAttributes(_filename, FileAttributes.Normal);
                        File.Delete(_filename);
                    }
                    File.Move(TempFileName, _filename);
                }
                else if (Status == DownloadStatus.Aborted)
                {
                    if (File.Exists(TempFileName))
                    {
                        File.SetAttributes(TempFileName, FileAttributes.Normal);
                        File.Delete(TempFileName);
                    }
                }
            }
            if (_responseStream != null)
            {
                _responseStream.Dispose();
                _responseStream = null;
            }
            if (_response != null)
            {
                _response.Dispose();
                _response = null;
            }
            _request = null;
        }

        protected void RaiseDownloadUpdatedEvent()
        {
            ConsoleManager.Log("{0}: {1} DownloadTask " + Status, CallerName, Token);
            DownloadTaskEventArgs e = new DownloadTaskEventArgs(this, Status);
            Application.Current.Dispatcher.BeginInvoke(new Action(delegate
            {
                DownloadUpdated?.Invoke(e);
            }));
        }

        private void ResponseCallback(IAsyncResult asyncResult)
        {
            try
            {
                _response = _request.EndGetResponse(asyncResult) as HttpWebResponse;

                if (_response.Headers["Accept-Ranges"] != null && _response.Headers["Accept-Ranges"].ToLower() == "bytes")
                {
                    _acceptRanges = true;
                }

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
                    NumberOfTotal = _response.ContentLength;
                }

                if (_response.Headers["Accept-Length"] != null)
                {
                    NumberOfTotal = int.Parse(_response.Headers["Accept-Length"]);
                }

                _responseStream.BeginRead(_buffer, 0, _buffer.Length, ReadCallback, this);
            }
            catch (Exception)
            {
                Status = DownloadStatus.Aborted;
                RaiseDownloadUpdatedEvent();
                Clean();
            }
        }

        private void ReadCallback(IAsyncResult asyncResult)
        {
            try
            {
                int read = _responseStream.EndRead(asyncResult);
                if (read == 0)
                {
                    if (NumberOfTotal == -1)
                    {
                        NumberOfTotal = NumberOfDownload;
                    }

                    Status = DownloadStatus.Finished;
                    Clean();
                    RaiseDownloadUpdatedEvent();
                }
                else if (read > 0)
                {
                    if (read + NumberOfDownload > NumberOfTotal && NumberOfTotal != -1)
                    {
                        read = (int)(NumberOfTotal - NumberOfDownload);
                    }

                    _writer.Seek(NumberOfDownload, SeekOrigin.Begin);
                    _writer.Write(_buffer, 0, read);

                    NumberOfDownload += read;
                    _chunkSize += read;
                    if ((NumberOfTotal > 0 && _chunkSize > 0.01 * NumberOfTotal) || _chunkSize > 1024 * 512)
                    {
                        _chunkSize = 0;
                        RaiseDownloadUpdatedEvent();
                    }

                    _responseStream.BeginRead(_buffer, 0, _buffer.Length, ReadCallback, this);
                }
                else
                {
                    Status = DownloadStatus.Aborted;
                    RaiseDownloadUpdatedEvent();
                    Clean();
                }
            }
            catch (Exception)
            {
                Status = DownloadStatus.Aborted;
                RaiseDownloadUpdatedEvent();
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
