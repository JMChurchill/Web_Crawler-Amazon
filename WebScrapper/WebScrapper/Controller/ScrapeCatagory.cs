using System;
using System.Collections.Generic;
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

            var htmlNode = GetHtml(url);
            var temp = htmlNode.OwnerDocument;

            var searchResults = temp.DocumentNode.SelectNodes("//html/body/div[@id='a-page']/div[@id='search']/div[@class='s-desktop-width-max s-desktop-content sg-row']/div[@class='sg-col-16-of-20 sg-col sg-col-8-of-12 sg-col-12-of-16']/div[@class='sg-col-inner']/span[@class='rush-component s-latency-cf-section']/div[@class='s-main-slot s-result-list s-search-results sg-row']/div[@data-component-type='s-search-result']");

            //var searchResults = temp.DocumentNode.SelectNodes("//*[@id='search']/div[1]/div[2]/div/span[3]/div[2]/div[2]");


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
            if (timesLooped < 11)
            {
                GetDetailsFromSearchResults(nextPageUrl, timesLooped);
            }

            Console.WriteLine("**done**");
            //return lstPageDetails;
        }



    }
}
