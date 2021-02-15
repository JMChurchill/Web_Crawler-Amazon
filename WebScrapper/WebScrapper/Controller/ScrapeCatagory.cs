using HtmlAgilityPack;
using ScrapySharp.Extensions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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
            var lstPageDetails = new List<ProductDetailsForGroup>();

            var web = new HtmlWeb();

            // Optional: This is not absolutely necessary, but it's a good idea. See comments for the OnPreRequest handler
            web.PreRequest = new HtmlWeb.PreRequestHandler(OnPreRequest);

            // Optional: monitor some useful data we get back in the Post Repsonse:
            web.PostResponse = new HtmlWeb.PostResponseHandler(OnPostResponse);

            var temp = web.Load(url);

            var searchResults = temp.DocumentNode.SelectNodes("//html/body/div[@id='a-page']/div[@id='search']/div[@class='s-desktop-width-max s-desktop-content sg-row']/div[@class='sg-col-16-of-20 sg-col sg-col-8-of-12 sg-col-12-of-16']/div[@class='sg-col-inner']/span[@class='rush-component s-latency-cf-section']/div[@class='s-main-slot s-result-list s-search-results sg-row']/div[@data-component-type='s-search-result']");

            foreach (var result in searchResults)
            {
                var pageDetails = new ProductDetailsForGroup();

                //find link from each result
                var thisResult = result;

                var item = thisResult.SelectSingleNode(".//a[@class='a-link-normal a-text-normal']");
                string pName = item.SelectSingleNode(".//span[@class='a-size-medium a-color-base a-text-normal']").InnerText;
                pName = pName.Replace("  ", " ");
                
                pageDetails.Name = pName;

                pageDetails.Category = getCat(pName.ToLower());

                pageDetails.ImageURL = (thisResult.SelectSingleNode(".//img[@class='s-image']")).Attributes["src"].Value;
                //var tm = thisResult.SelectSingleNode(".//img[@class='s-image'");


                try
                {
                    pageDetails.Price = thisResult.SelectSingleNode(".//span[@class='a-offscreen']").InnerText;
                }
                catch (NullReferenceException ex)
                {}

                try
                {
                    var rating = thisResult.SelectSingleNode(".//span[@class='a-icon-alt']").InnerText;
                    pageDetails.Rating = rating.Replace(" out of 5 stars", "");
                }
                catch (NullReferenceException ex)
                {}

                string link = "https://www.amazon.co.uk" + item.Attributes["href"].Value;
                link = link.Replace("amp;", "");
                //pageDetails.Url = link;

                lstPageDetails.Add(pageDetails);

            }

            //write to csv
            //exportToCsv(lstPageDetails, "");

            //write to json file
            JSONWriter writer = new JSONWriter();

            string filePath = Path.GetFullPath("amazonProducts.Json");//file location may change

            writer.addToJson(lstPageDetails, filePath);

            timesLooped++;

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

            // use recursion
            if (timesLooped <= 200)
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

        public string getCat(string pName)
        {
            string category = "";
            //determin catagory
            if (pName.Contains("tv") && !(pName.Contains("stick")))
            {
                category = "TVs";
            }
            if (pName.Contains("streaming device") || pName.Contains("streaming stick") || (pName.Contains("streaming") && (pName.Contains("media player"))))
            {
                category = "Streaming Device";
            }
            else if (pName.Contains("airpods") || (pName.Contains("wireless") && (pName.Contains("earphones") || pName.Contains("headphones"))))
            {
                category = "Wireless Earphones";
            }
            else if (pName.Contains("phone") && !(pName.Contains("speaker")))
            {
                category = "Phones";
                if (pName.Contains("mobile") || pName.Contains("smart") || pName.Contains("iphone"))
                {
                    category = "Mobile Phones";
                }
                else if (pName.Contains("home"))
                {
                    category = "Home Phones";
                }
            }
            else if (pName.Contains("batteries") || pName.Contains("battery"))
            {
                category = "Batteries";
            }
            else if (pName.Contains("speaker"))
            {
                category = "Speakers";
                if (pName.Contains("smart"))
                {
                    category = "Smart Speaker";
                }
            }
            else if ((pName.Contains("power") && pName.Contains("adapter")) || pName.Contains("charger"))
            {
                category = "Power Adapters";
            }
            else if (pName.Contains("webcam"))
            {
                category = "Webcams";
            }
            else if ((pName.Contains("router") || pName.Contains("wi-fi") || pName.Contains("wifi")) && !(pName.Contains("laptop") || pName.Contains("tablet") || pName.Contains("pc") || pName.Contains("ipad")))
            {
                category = "Wi-Fi and Routers";
            }
            else if (pName.Contains("hdmi") || pName.Contains("cable") || pName.Contains("ethernet"))
            {
                category = "Cables";
                if (pName.Contains("eithernet") && pName.Contains("hub"))
                {
                    category = "Eithernet Hubs";
                }
            }
            else if (pName.Contains("usb"))
            {
                category = "USB devices";
                if (pName.Contains("hub"))
                {
                    category = "USB Hubs";
                }
            }
            else if (pName.Contains("earpods") || pName.Contains("earphones"))
            {
                category = "Earphones";
            }
            else if (pName.Contains("ink"))
            {
                category = "Ink";
            }
            else if (pName.Contains("tablet") || pName.Contains("ipad"))
            {
                category = "Tablets";
            }
            else if (pName.Contains("hard drive") || pName.Contains("hdd"))
            {
                category = "Hard Drives";
                if (pName.Contains("External"))
                {
                    category = "External Hard Drives";
                }
            }
            else if (pName.Contains("ssd") || pName.Contains("solid state drive"))
            {
                category = "Solid State Drives";
                if (pName.Contains("External"))
                {
                    category = "External Solid State Drives";
                }
            }
            else if ((pName.Contains("memory") && pName.Contains("card")) || (pName.Contains("sd") && pName.Contains("card")))
            {
                category = "SD cards";
            }
            else if (pName.Contains("laptop") || pName.Contains("macbook"))
            {
                category = "laptops";
            }
            else if (pName.Contains("headphone") || pName.Contains("headset"))
            {

                if (pName.Contains("wireless"))
                {
                    category = "Wireless";
                }
                if (pName.Contains("headset"))
                {
                    category += "Headsets";
                }
                else
                {
                    category += "Headphones";
                }
            }
            else if (pName.Contains("controller"))
            {
                category = "Controllers";
                if (pName.Contains("wireless"))
                {
                    category = "Wireless Controllers";
                }
            }
            else if (pName.Contains("tracker"))
            {
                category = "Activity trackers";
            }
            else if (pName.Contains("kindle") || pName.Contains("reader"))
            {
                category = "E-Readers";
            }
            else if ((pName.Contains("smart") || pName.Contains("apple")) && pName.Contains("watch"))
            {
                category = "Smart Watches";
            }
            else if (pName.Contains("mouse mat"))
            {
                category = "Mouse Mat";
            }
            else if (pName.Contains("mouse"))
            {
                category = "Mice";
                if (pName.Contains("wireless") || pName.Contains("wire-less") || pName.Contains("bluetooth"))
                {
                    category = "Wireless Mice";
                }
                if (pName.Contains("keyboard"))
                {
                    category += " and Keyboards";
                }
            }
            else if (pName.Contains("keyboard"))
            {
                category = "Keyboards";
                if (pName.Contains("wireless") || pName.Contains("wire-less") || pName.Contains("bluetooth"))
                {
                    category = "Wireless Keyboards";
                }
            }
            else if (pName.Contains("monitor"))
            {
                category = "Monitors";
            }
            else if (pName.Contains("camera"))
            {
                category = "Cameras";
                if (pName.Contains("security"))
                {
                    category = "Security Cameras";
                }
                else if (pName.Contains("smart"))
                {
                    category = "Smart Cameras";
                }
            }
            else if (pName.Contains("mac mini") || pName.Contains("pc") || (pName.Contains("desktop") && pName.Contains("computer")))
            {
                category = "Desktop Computers";
            }
            else if (pName.Contains("ram") || pName.Contains("ddr4") || pName.Contains("ddr3"))
            {
                category = "RAM";
            }
            else if (pName.Contains("psu") || pName.Contains("power supply"))
            {
                category = "Power Supplies";
            }
            else if ((pName.Contains("graphics") && pName.Contains("card")))
            {
                category = "Graphics Cards";
            }
            else if (pName.Contains("cpu") || pName.Contains("processor"))
            {
                category = "CPUs";
            }
            else if (pName.Contains("doorbell") || (pName.Contains("door") && pName.Contains("bell")))
            {
                category = "Doorbells";
            }
            else if (pName.Contains("adapter"))
            {
                category = "Adapters";
            }
            else if (pName.Contains("dvd"))
            {
                category = "DVDs";
                if (pName.Contains("player"))
                {
                    category = "DVD Players";
                }
            }
            else if (pName.Contains("remote"))
            {
                category = "Remotes";
            }
            else if (pName.Contains("smart"))
            {
                category = "Smart Devices";
            }
            else
            {
                category = "Electronics";
            }
            return category;
        }

        public void getDetailsV2(string url, int timesLooped)
        {


            Console.WriteLine(timesLooped);
            var lstPageDetails = new List<ProductDetailsForGroup>();
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
