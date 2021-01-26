using HtmlAgilityPack;
using ScrapySharp.Network;
using System;
using System.Collections.Generic;
using System.Text;
using WebScrapper.Model;

namespace WebScrapper.Controller
{
    class ScrapeSearchPage : ScrapeController
    {
        public List<ProductDetails> GetDetailsFromSearchResults(string url)
        {
            var lstPageDetails = new List<ProductDetails>();
            //var searchPageLinks = new List<string>();

            var htmlNode = GetHtml(url);
            var temp = htmlNode.OwnerDocument;

            var searchResults = temp.DocumentNode.SelectNodes("//html/body/div[@id='a-page']/div[@id='search']/div[@class='s-desktop-width-max s-desktop-content sg-row']/div[@class='sg-col-16-of-20 sg-col sg-col-8-of-12 sg-col-12-of-16']/div[@class='sg-col-inner']/span[@class='rush-component s-latency-cf-section']/div[@class='s-main-slot s-result-list s-search-results sg-row']/div[@data-component-type='s-search-result']");

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
            return lstPageDetails;
        }



    }
}
