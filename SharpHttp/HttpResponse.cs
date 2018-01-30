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

namespace SharpHttpClient
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

        public string ResponseString
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
                        if (arr.Count() > idx) {
                            charset = arr[idx];
                        }
                    }
                }
                if (string.IsNullOrEmpty(charset))
                {
                    charset = "utf-8";
                }

                if (charset.ToLower().Contains("utf-8"))
                {
                    return utf8String;
                }
                else
                {
                    var encoding = Encoding.GetEncoding(charset);
                    if (encoding != null)
                    {
                        return encoding.GetString(bytes);
                    }
                    return Encoding.UTF8.GetString(bytes);
                }
            }
        }

        public byte[] ResponseByteArray
        {
            get
            {
                var task = _resp.Content.ReadAsByteArrayAsync();
                task.Wait();
                return task.Result;
            }
        }

        public Stream ResponseStream
        {
            get
            {
                var task = _resp.Content.ReadAsStreamAsync();
                task.Wait();
                return task.Result;
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
