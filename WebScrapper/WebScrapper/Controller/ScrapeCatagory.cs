using HtmlAgilityPack;
using ScrapySharp.Extensions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using WebScrapper.Model;

namespace WebScrapper.Controller
{
    class ScrapeCatagory : ScrapeController
    {
        public void GetDetailsFromSearchResults(string url, int timesLooped)
        {
            Console.WriteLine(timesLooped);
            var lstPageDetails = new List<ProductDetails>();
            //var searchPageLinks = new List<string>();

            var web = new HtmlWeb();

            // Optional: This is not absolutely necessary, but it's a good idea. See comments for the OnPreRequest handler
            web.PreRequest = new HtmlWeb.PreRequestHandler(OnPreRequest);

            // Optional: monitor some useful data we get back in the Post Repsonse:
            web.PostResponse = new HtmlWeb.PostResponseHandler(OnPostResponse);

            var temp = web.Load(url);



            //var htmlNode = GetHtml(url);
            //var temp = htmlNode.OwnerDocument;

            var searchResults = temp.DocumentNode.SelectNodes("//html/body/div[@id='a-page']/div[@id='search']/div[@class='s-desktop-width-max s-desktop-content sg-row']/div[@class='sg-col-16-of-20 sg-col sg-col-8-of-12 sg-col-12-of-16']/div[@class='sg-col-inner']/span[@class='rush-component s-latency-cf-section']/div[@class='s-main-slot s-result-list s-search-results sg-row']/div[@data-component-type='s-search-result']");

            //var searchResults = temp.DocumentNode.CssSelect("");

            //var searchResults = temp.DocumentNode.SelectNodes("//*[@id='search']/div[1]/div[2]/div/span[3]/div[2]/div[2]");

            //if (searchResults == null)
            //{
            //    searchResults = temp.DocumentNode.SelectNodes("//html/body/div[@id='a-page']/div[@id='search']/div[@class='s-desktop-width-max s-desktop-content sg-row']/div[@class='sg-col-16-of-20 sg-col sg-col-8-of-12 sg-col-12-of-16']/div[@class='sg-col-inner']/span[@class='rush-component s-latency-cf-section']/div[@class='s-main-slot s-result-list s-search-results sg-row']/div[@data-component-type='s-search-result']");
            //}




            foreach (var result in searchResults)
            {
                var pageDetails = new ProductDetails();

                //find link from each result
                var thisResult = result;

                var item = thisResult.SelectSingleNode(".//a[@class='a-link-normal a-text-normal']");
                pageDetails.Name = item.SelectSingleNode(".//span[@class='a-size-medium a-color-base a-text-normal']").InnerText;

                try
                {
                    pageDetails.Price = thisResult.SelectSingleNode(".//span[@class='a-offscreen']").InnerText;
                }
                catch (NullReferenceException ex)
                {
                }

                try
                {
                    var rating = thisResult.SelectSingleNode(".//span[@class='a-icon-alt']").InnerText;
                    pageDetails.Rating = rating.Replace(" out of 5 stars", "");
                }
                catch (NullReferenceException ex)
                {
                }

                string link = "https://www.amazon.co.uk" + item.Attributes["href"].Value;
                link = link.Replace("amp;", "");
                pageDetails.Url = link;

                lstPageDetails.Add(pageDetails);

            }

            exportToCsv(lstPageDetails, "");
            timesLooped++;
            //get next pages url
            //var nextBtn = temp.DocumentNode.SelectSingleNode("//*[@id='search']/div[1]/div[2]/div/span[3]/div[2]/div[25]/span/div/div/ul/li[7]/a");
            //string nextPageUrl = "https://www.amazon.co.uk" + nextBtn.Attributes["href"].Value;
            //nextPageUrl = nextPageUrl.Replace("amp;", "");
            //Console.WriteLine(nextPageUrl);
            string nextPageUrl = "";
            if (timesLooped == 2)
            {
                nextPageUrl = url.Replace("dc&", "dc&page=2&");
            }
            else
            {
                string oldP = "page=" + (timesLooped-1);
                string newP = "page=" + timesLooped;
                nextPageUrl = url.Replace(oldP, newP);
            }


            Console.WriteLine(nextPageUrl);
            
            // use recusion
            if (timesLooped <= 399)
            {
                if (nextPageUrl != null)
                {
                    GetDetailsFromSearchResults(nextPageUrl, timesLooped);
                }
                else
                {
                    Console.WriteLine(timesLooped + "next url was null ");
                }

            }

            Console.WriteLine("**done**");
            //return lstPageDetails;
        }

        public void getDetailsV2(string url, int timesLooped)
        {


            Console.WriteLine(timesLooped);
            var lstPageDetails = new List<ProductDetails>();
            //var searchPageLinks = new List<string>();


            var web = new HtmlWeb();

            // Optional: This is not absolutely necessary, but it's a good idea. See comments for the OnPreRequest handler
            web.PreRequest = new HtmlWeb.PreRequestHandler(OnPreRequest);

            // Optional: monitor some useful data we get back in the Post Repsonse:
            web.PostResponse = new HtmlWeb.PostResponseHandler(OnPostResponse);

            var temp = web.Load(url);


            // The result of the web.Load w


            //var htmlNode = GetHtml(url);
            //var temp = htmlNode.OwnerDocument;


            //var allPageItems = temp.DocumentNode.CssSelect(".s-latency-cf-section").ToList();
            //var allname = temp.DocumentNode.CssSelect(".a-color-base.a-text-normal");

            //foreach (var pageItem in allPageItems)
            //{
            //    var name = pageItem.CssSelect(".a-color-base.a-text-normal");

            //    var price = pageItem.CssSelect(".a-offscreen");
            //}

            //name.a - color - base.a - text - normal
            var allNames = temp.DocumentNode.CssSelect(".a-color-base.a-text-normal").ToList();

            //price
            var allPrices = temp.DocumentNode.CssSelect(".a-offscreen");


            //allNames[1];
            //rating


            //url
            foreach (var name in allNames)
            {
                string theName = name.InnerText;
                string thePrice = allPrices.Single().InnerText;



            }



        }


        private static bool OnPreRequest(HttpWebRequest request)
        {
            // Let's not wait more than 6 seconds for a request to complete.
            request.Timeout = 6000;

            // Allow the auto redirect... no point in blocking this
            request.AllowAutoRedirect = true;

            // Setting a User Agent can decrease the risk of rejection
            request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/56.0.2924.87 Safari/537.36";

            // Set whatever else might help to make you look like a genuine browser. I find the following helps, especially the ContentType             
            request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
            request.ContentType = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
            request.KeepAlive = true;
            request.Method = "GET";
            request.Headers.Add("Accept-Language", "en-US,en;q=0.5");
            request.Headers.Add("Accept-Encoding", "gzip, deflate");


            return true;
        }

        private static void OnPostResponse(HttpWebRequest request, HttpWebResponse response)
        {

            // see what cookies came back:
            Debug.WriteLine("Cookies: ");
            foreach (Cookie cookie in response.Cookies)
            {
                Debug.WriteLine($"{cookie.Name}: {cookie.Value}");
            }
            Debug.WriteLine("-----");


            // Headers: this is useful if you want to see what kind server the site runs on, among other things...
            Debug.WriteLine("Headers: ");
            foreach (var key in response.Headers.AllKeys)
            {
                Debug.WriteLine($"{key}: {response.Headers[key]}");
            }
            Debug.WriteLine("-----");


        }



    }
}
