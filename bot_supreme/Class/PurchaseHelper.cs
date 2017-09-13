using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace bot_supreme.Class
{
    class PurchaseHelper
    {
        private UserPreference UserPreference { get; set; }
        public PurchaseHelper(UserPreference userPreference)
        {
          
            UserPreference = userPreference;
        }


        private bool Available(HtmlNode article)
        {
            if (article.ChildNodes[0].ChildNodes[0].InnerText == "sold out")
            {
                return false;
            }
            return true;
        }

        private string FindArticlePage(ProductInfo product, IEnumerable<HtmlNode> articles)
        {
            foreach (var article in articles)
            {
                if (!Available(article))
                    continue;
                if (article.SelectSingleNode("div/h1/a").InnerText == product.Name &&
                    article.SelectSingleNode("div/p/a").InnerText == product.Color)
                {
                    return article.SelectSingleNode("div/p/a").Attributes["href"].Value;
                }
            }
            return null;
        }

        public bool addToCard(HTTPRequest httpRequest ,HtmlDocument doc)
        {
          
            var form = doc.DocumentNode.SelectSingleNode("//*[@id='cart-addf']");
            //*[@id="cart-addf"]/input[1]
          
            var list = doc.DocumentNode.Descendants("input").ToList();
        //    var utf8 = list[0].Attributes["value"].Value;
            var style = list[1].Attributes["value"].Value;
            var size = doc.DocumentNode.Descendants("option").ToList();
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://www.supremenewyork.com" + form.Attributes["action"].Value);
            request.KeepAlive = true;

            WebHeaderCollection headerCollection = new WebHeaderCollection();
            headerCollection.Add("Origin: http://www.supremenewyork.com");
            headerCollection.Add($"X-CSRF-Token: {doc.DocumentNode.SelectSingleNode("//*[@name='csrf-token']").Attributes["content"].Value}");

            request.UserAgent =
              "User-Agent: Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/58.0.3029.110 Safari/537.36";
            request.ContentType = "application/x-www-form-urlencoded; charset=UTF-8";
            request.Accept = "*/*;q=0.5, text/javascript, application/javascript, application/ecmascript, application/x-ecmascript";
            headerCollection.Add("Accept-Encoding: gzip, deflate, sdch");
            headerCollection.Add("Accept-Language: en-US,en;q=0.8");
            headerCollection.Add("X-Requested-With: XMLHttpRequest");
           
           
            request.Headers = headerCollection;

            httpRequest.MakeRequest(ref request, "POST", Encoding.Default.GetBytes($"utf8=%E2%9C%93&style={style}&size={size[0].Attributes["value"].Value}&commit=ajouter"));
            var price = doc.DocumentNode.SelectSingleNode("//*[@id='details']/p[3]/span");
            var pureCart = "{\"" + size[0].Attributes["value"].Value + "\":1,\"cookie\":\"1 item--" + size[0].Attributes["value"].Value + "," + style + "\",\"total\":\"" +
                            price.InnerText + "\"}";
            pureCart =
                "%7B%2235797%22%3A1%2C%22cookie%22%3A%221%20item--35797%2C18235%22%2C%22total%22%3A%22%E2%82%AC144%22%7D";

            Cookie pureCartCookie = new Cookie("pure_cart", pureCart) { Domain = "www.supremenewyork.com" };
            Cookie hasShownCookieNotice = new Cookie("hasShownCookieNotice", "1") { Domain = "www.supremenewyork.com" };
            Cookie __utma = new Cookie("__utma", "74692624.1018831755.1495478988.1495478988.1495478988.1") { Domain = "www.supremenewyork.com" };
            Cookie __utmb = new Cookie("__utmb", "74692624.5.10.1495478988") { Domain = "www.supremenewyork.com" };
            Cookie __utmc = new Cookie("__utmc", "74692624") { Domain = "www.supremenewyork.com" };
            Cookie __utmt = new Cookie("__utmt", "1") { Domain = "www.supremenewyork.com" };
            Cookie __utmz = new Cookie("__utmz", "74692624.1495478988.1.1.utmcsr=(direct)|utmccn=(direct)|utmcmd=(none)") { Domain = "www.supremenewyork.com" };
            Cookie __ga = new Cookie("_ga", "GA1.2.1018831755.1495478988") { Domain = "www.supremenewyork.com" };
            Cookie _gat = new Cookie("_gat", "1") { Domain = "www.supremenewyork.com" };
            Cookie _gid = new Cookie("_gid", "GA1.2.2134555435.1495478988") { Domain = "www.supremenewyork.com" };

            httpRequest._cookieContainer.Add(__utma);
            httpRequest._cookieContainer.Add(__utmb);
            httpRequest._cookieContainer.Add(__utmc);
            httpRequest._cookieContainer.Add(__utmt);
            httpRequest._cookieContainer.Add(__utmz);
            httpRequest._cookieContainer.Add(__ga);
            httpRequest._cookieContainer.Add(_gat);
            httpRequest._cookieContainer.Add(_gid);
            httpRequest._cookieContainer.Add(pureCartCookie);
            httpRequest._cookieContainer.Add(hasShownCookieNotice);
            return true;
        }

        public bool goToCheckout(HTTPRequest httpRequest)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://www.supremenewyork.com/checkout");
            WebHeaderCollection headerCollection = new WebHeaderCollection();
            request.KeepAlive = true;

            headerCollection.Add("Accept-Encoding: gzip, deflate, sdch, br");
            headerCollection.Add("Accept-Language: fr-FR,fr;q=0.8,en-US;q=0.6,en;q=0.4");
            headerCollection.Add("Upgrade-Insecure-Requests: 1");
            request.Headers = headerCollection;
            request.UserAgent =
         "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/58.0.3029.110 Safari/537.36";
            request.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8";
            request.KeepAlive = true;

            httpRequest.MakeRequest(ref request, "GET", null);
            return true;
        }
        public bool Purchase()
        {
            try
            {

                HTTPRequest httpRequest = new HTTPRequest();
                WebHeaderCollection headerCollection = new WebHeaderCollection();
                HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
                headerCollection.Add("Accept-Encoding: gzip, deflate, sdch");
                headerCollection.Add("Accept-Language: en-US,en;q=0.8");
                httpRequest.MakeRequest("http://www.supremenewyork.com/shop/all", "GET", null, headerCollection);

                foreach (var itm in UserPreference.ProductList)
                {

                    doc.LoadHtml(httpRequest.MakeRequest($"http://www.supremenewyork.com/shop/all/{itm.Category}", "GET", null, null));
                    string articlePage = FindArticlePage(itm, doc.DocumentNode.Descendants("article"));
                    if (articlePage == null)
                        continue;
                    doc.LoadHtml(httpRequest.MakeRequest($"http://www.supremenewyork.com{articlePage}", "GET", null, headerCollection));
                    addToCard(httpRequest, doc);
                    Thread.Sleep(800);
                }

                goToCheckout(httpRequest);

            //    doc.LoadHtml();

                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }
        }
    }
}
