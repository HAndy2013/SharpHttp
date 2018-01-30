using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace SharpHttp
{
    public class SharpHttpClient
    {

        #region - PRIVATE -
        private HttpClient _httpClient;
        private HttpClientHandler _clientHandler;
        private WebProxy _proxy;
        #endregion

        #region - PUBLIC PROPS -

        public bool UserProxy
        {
            get
            {
                if (_clientHandler != null)
                {
                    return _clientHandler.UseProxy;
                }
                return false;
            }
            set
            {
                if (_clientHandler != null)
                    _clientHandler.UseProxy = value;
            }
        }

        public bool AcceptCookie
        {
            get
            {
                if (_clientHandler != null)
                {
                    return _clientHandler.UseCookies;
                }
                return false;
            }
            set
            {
                if (_clientHandler != null)
                    _clientHandler.UseCookies = value;
            }
        }

        public bool AutoRedirect
        {
            get
            {
                if (_clientHandler != null)
                    return _clientHandler.AllowAutoRedirect;
                return false;
            }
            set
            {
                if (_clientHandler != null)
                    _clientHandler.AllowAutoRedirect = value;
            }
        }
        #endregion

        #region - CONSTRUCTOR -
        public SharpHttpClient()
        {
            _clientHandler = new HttpClientHandler();
            _httpClient = new HttpClient(_clientHandler);
            SetUserAgent(UserAgent.DEFAULT);
        }
        #endregion

        #region - PUBLIC METHODS -
        public void SetProxy(string host, int port)
        {
            _proxy = new WebProxy(host, port);
            _clientHandler.Proxy = _proxy;
        }

        public void SetCookie(string cookieStr, string url)
        {
            var cc = GetCookies(cookieStr, url);
            if (_clientHandler.CookieContainer == null)
            {
                _clientHandler.CookieContainer = new CookieContainer();
            }
            _clientHandler.CookieContainer.Add(cc);
        }

        public void AddHeaders(Dictionary<string, string> headers)
        {
            foreach (var header in headers)
            {
                if (_httpClient.DefaultRequestHeaders.Contains(header.Key))
                {
                    _httpClient.DefaultRequestHeaders.Remove(header.Key);
                }
                _httpClient.DefaultRequestHeaders.Add(header.Key, header.Value);
            }
        }

        public void SetUserAgent(string ua)
        {
            AddHeaders(new Dictionary<string, string> { { "User-Agent", ua } });
        }

        #region - GET -
        public async Task<HttpResponse> GetAsync(string url)
        {
            var resp = await Get(url); 
            return new HttpResponse(resp);
        }
        #endregion

        #region - POST -
        public async Task<HttpResponse> PostAsync(string url, List<KeyValuePair<string, string>> formParams)
        {
            var content = new FormUrlEncodedContent(formParams);
            var resp = await Post(url, content);
            return new HttpResponse(resp);
        }

        public async Task<HttpResponse> PostAsync(string url, byte[] byteArray)
        {
            var content = new ByteArrayContent(byteArray);
            var resp = await Post(url, content);
            return new HttpResponse(resp);
        }

        public async Task<HttpResponse> PostAsync(string url, string stringContent)
        {
            var content = new StringContent(stringContent);
            var resp = await Post(url, content);
            return new HttpResponse(resp);
        }

        public async Task<HttpResponse> PostAsync(string url, Stream streamContent)
        {
            var content = new StreamContent(streamContent);
            var resp = await Post(url, content);
            return new HttpResponse(resp);
        } 
        #endregion

        #endregion

        #region - PRIVATE METHODS -
        private async Task<HttpResponseMessage> Post(string url, HttpContent httpContent)
        {
            var resp = await _httpClient.PostAsync(url, httpContent);
            return resp;
        }

        private async Task<HttpResponseMessage> Get(string url)
        {
            var resp = await _httpClient.GetAsync(url);
            return resp;
        }

        private CookieCollection GetCookies(string cookieString, string url)
        {
            var pass = new[] { "expires", "path" };
            var cc = new CookieCollection();
            if (!string.IsNullOrEmpty(cookieString))
            {
                var domain = new Uri(url).Host;
                var cookieArr = cookieString.Split(';');
                foreach (var cookieKv in cookieArr)
                {
                    var idx = cookieKv.IndexOf('=');
                    if (idx > 0)
                    {
                        var name = cookieKv.Substring(0, idx).Trim();
                        var val = cookieKv.Substring(idx + 1).Trim();
                        if (!pass.Contains(name))
                            cc.Add(new Cookie(name, val, "/", domain));
                    }
                }
                return cc;
            }
            return null;
        }
        #endregion

    }
}
