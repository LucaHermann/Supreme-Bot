using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace bot_supreme
{
    public class _2Captcha
    {
        private string ApiKey { get; set; }
        private string SiteKey { get; set; }

        public _2Captcha(string siteKey, string apikey = "")
        {
            ApiKey = apikey;
            SiteKey = siteKey;
        }

        private string MakeRequest(string postData, string Method)
        {
            try
            {
                if (Method == "POST")
                {
                    var request = (HttpWebRequest)WebRequest.Create("https://2captcha.com/in.php");
                    var data = Encoding.ASCII.GetBytes(postData);

                    request.Method = "POST";
                    request.ContentType = "application/x-www-form-urlencoded";
                    request.ContentLength = data.Length;

                    using (var stream = request.GetRequestStream())
                    {
                        stream.Write(data, 0, data.Length);
                    }

                    var response = (HttpWebResponse)request.GetResponse();

                    return new StreamReader(response.GetResponseStream()).ReadToEnd();
                 
                }
                else if (Method == "GET")
                {
                    var request = (HttpWebRequest)WebRequest.Create("https://2captcha.com/res.php?" + postData);

                    var response = (HttpWebResponse)request.GetResponse();

                    return  new StreamReader(response.GetResponseStream()).ReadToEnd();
                }

                return null;

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }

        public string ResolveCaptcha()
        {   
            string response = MakeRequest($"key={ApiKey}&method=userrecaptcha&googlekey={SiteKey}&here=now&pageurl=supremenewyork.com","POST");
            string status = response.Split('|')[0];
            if (status == "OK")
            {
                string id = response.Split('|')[1];
                string finalresponse = "CAPCHA_NOT_READY";
                Stopwatch sw = Stopwatch.StartNew();
                while (!finalresponse.StartsWith("OK"))
                {
                    Application.DoEvents();
                    Thread.Sleep(2500);
                    finalresponse = MakeRequest($"key={ApiKey}&action=get&id={id}", "GET");
                }
                sw.Stop();
                return finalresponse.Split('|')[1];
            }


            return null;
        }
    }
}