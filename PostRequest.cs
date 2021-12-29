using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ParserBaucenter
{
    class PostRequest
    {
        public string _address;
        public HttpWebRequest _request;

        public Dictionary<string, string> Headers;

        public string Response { get; set; }
        public string Accept { get; set; }
        public string Host { get; set; }
        public string Data { get; set; }
        public string ContentType { get; set; }
        public WebProxy Proxy { get; set; }
        public string Referer { get; set; }
        public string UserAgent { get; set; }

        public PostRequest(string address)
        {
            _address = address;
            Headers = new Dictionary<string, string>();
        }

        public void Run(CookieContainer cookieContainer)
        {
            _request = (HttpWebRequest)WebRequest.Create(_address);
            _request.Method = "Post";
            _request.CookieContainer = cookieContainer;
            _request.Accept = Accept;
            _request.Host = Host;
            _request.Proxy = Proxy;
            _request.ContentType = ContentType;
            _request.Referer = Referer;
            _request.UserAgent = UserAgent;

            byte[] sentdata = Encoding.UTF8.GetBytes(Data);
            _request.ContentLength = sentdata.Length;
            Stream sendstream = _request.GetRequestStream();
            sendstream.Write(sentdata, 0, sentdata.Length);
            sendstream.Close();

            foreach (var pair in Headers)
            {
                _request.Headers.Add(pair.Key, pair.Value);
            }

            try
            {
                HttpWebResponse response = (HttpWebResponse)_request.GetResponse();
                var stream = response.GetResponseStream();
                if (stream != null) Response = new StreamReader(stream).ReadToEnd();
            }
            catch (Exception)
            {

            }
        }
    }
}
