using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace bot_supreme.Class
{
    public class HTTPRequest
    {
        public CookieContainer _cookieContainer = new CookieContainer();
        public Uri referer = null;

        public string MakeRequest(string url, string method, byte[] data, WebHeaderCollection headerCollection)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.CookieContainer = _cookieContainer;
            if (headerCollection != null)
                request.Headers = headerCollection;
            request.UserAgent =
                "User-Agent: Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/58.0.3029.110 Safari/537.36";
            request.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8";
            request.ContentType = "application/x-www-form-urlencoded";
            request.KeepAlive = true;
            request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
            if (referer != null)
                request.Referer = referer.AbsoluteUri;


            if (method == "POST")
            {
                request.Method = method;
                request.ContentLength = data.Length;
                using (var stream = request.GetRequestStream())
                    stream.Write(data, 0, data.Length);
            }
            else if (method == "GET")
            {
                request.Method = method;
                request.ContentLength = 0;
            }

            var response = (HttpWebResponse)request.GetResponse();
            string str = new StreamReader(response.GetResponseStream()).ReadToEnd();
            var x = response.ResponseUri;
            _cookieContainer = request.CookieContainer;
          
            referer = response.ResponseUri;
            return str;
        }
        public string MakeRequest(ref HttpWebRequest request, string method, byte[] data)
        {
            request.CookieContainer = _cookieContainer;
            if (method == "POST")
            {
                request.Method = method;
                request.ContentLength = data.Length;
                using (var stream = request.GetRequestStream())
                    stream.Write(data, 0, data.Length);
            }
            else if (method == "GET")
            {
                request.Method = method;
                //  request.ContentLength = 0;
            }
            //Encoding.GetEncoding(response.CharacterSet)
            var response = (HttpWebResponse)request.GetResponse();
            string str = new StreamReader(response.GetResponseStream()).ReadToEnd();
            var x = response.ResponseUri;
            _cookieContainer = request.CookieContainer;
            var p = _cookieContainer.GetCookies(new Uri("https://www.supremenewyork.com"));
            referer = response.ResponseUri;
            return str;
        }
        public string UrlEncode(string value)
        {
            return WebUtility.UrlEncode(value);
        }
    }
}
