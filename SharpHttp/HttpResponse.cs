using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace SharpHttp
{
    public class HttpResponse
    {
        public HttpResponse(HttpResponseMessage resp)
        {
            _resp = resp;
        }

        private HttpResponseMessage _resp;

        public HttpStatusCode StatusCode
        {
            get
            {
                return _resp.StatusCode;
            }
        }

        public string ResponseRawString
        {
            get
            {
                var task = _resp.Content.ReadAsStringAsync();
                task.Wait();
                return task.Result;
            }
        }

        public string Charset
        {
            get
            {
                var charset = ContentHeaders.ContentType.CharSet;
                var bytes = ResponseByteArray;
                var utf8String = Encoding.UTF8.GetString(bytes);
                if (string.IsNullOrEmpty(charset))
                {
                    var regex = new Regex("<meta ?[^<]*charset=\"?[a-zA-Z0-9\\-]+\"?[^<]*/?>");
                    var match = regex.Match(utf8String);
                    if (match.Success)
                    {
                        var arr = match.Value.Split(new[] { ' ', ';', '=', '"' }, StringSplitOptions.RemoveEmptyEntries);
                        var idx = arr.ToList().IndexOf("charset") + 1;
                        if (arr.Count() > idx)
                        {
                            charset = arr[idx];
                        }
                    }
                }
                if (string.IsNullOrEmpty(charset))
                {
                    charset = "utf-8";
                }
                return charset;
            }
        }

        private string _responseString;
        public string ResponseString
        {
            get
            {
                if (string.IsNullOrEmpty(_responseString))
                {
                    var charset = ContentHeaders.ContentType.CharSet;
                    var bytes = ResponseByteArray;
                    var utf8String = Encoding.UTF8.GetString(bytes);

                    var encoding = Encoding.GetEncoding(Charset);
                    if (encoding != null)
                    {
                        _responseString = encoding.GetString(bytes);
                    }
                    else
                    {
                        _responseString = Encoding.UTF8.GetString(bytes);
                    }
                }
                return _responseString;
            }
        }

        private byte[] _responseByteArray;
        public byte[] ResponseByteArray
        {
            get
            {
                if (_responseByteArray == null)
                {
                    var task = _resp.Content.ReadAsByteArrayAsync();
                    task.Wait();
                    _responseByteArray = task.Result;
                }
                return _responseByteArray;
            }
        }

        private Stream _responseStream;
        public Stream ResponseStream
        {
            get
            {
                if (_responseStream == null)
                {
                    var task = _resp.Content.ReadAsStreamAsync();
                    task.Wait();
                    _responseStream = task.Result;
                }
                return _responseStream;
            }
        }
        
        public HttpContentHeaders ContentHeaders
        {
            get
            {
                return _resp.Content.Headers;
            }
        }

        public HttpResponseHeaders ResponseHeaders
        {
            get
            {
                return _resp.Headers;
            }
        }
    }
}
