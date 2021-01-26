using HtmlAgilityPack;
using ScrapySharp.Network;
using System;
using System.Collections.Generic;
using System.Text;
using WebScrapper.Model;

namespace WebScrapper.Controller
{
    class ScrapePageDetails : ScrapeController
    {

        public List<string> getSearchPageLinks(string url)
        {
            var searchPageLinks = new List<string>();
            var htmlNode = GetHtml(url);
            var temp = htmlNode.OwnerDocument;

            var searchResults = temp.DocumentNode.SelectNodes("//html/body/div[@id='a-page']/div[@id='search']/div[@class='s-desktop-width-max s-desktop-content sg-row']/div[@class='sg-col-16-of-20 sg-col sg-col-8-of-12 sg-col-12-of-16']/div[@class='sg-col-inner']/span[@class='rush-component s-latency-cf-section']/div[@class='s-main-slot s-result-list s-search-results sg-row']/div[@data-component-type='s-search-result']");

            foreach (var result in searchResults)
            {
                //find link from each result
                var thisResult = result;

                var item = thisResult.SelectSingleNode(".//a[@class='a-link-normal a-text-normal']");


                string link = "https://www.amazon.co.uk" + item.Attributes["href"].Value;
                link = link.Replace("amp;", "");
                searchPageLinks.Add(link);
            }
            return searchPageLinks;
        }


        public List<ProductDetails> GetPageDetails(List<string> urls, string searchTerm)
        {
            var lstPageDetails = new List<ProductDetails>();
            foreach (var url in urls)
            {
                var htmlNode = GetHtml(url);
                var pageDetails = new ProductDetails();

                try
                {
                    pageDetails.Name = htmlNode.OwnerDocument.DocumentNode.SelectSingleNode("//html/head/title").InnerText;
                    pageDetails.Name = pageDetails.Name.Replace("&#39;&#39;", "");
                    pageDetails.Name = pageDetails.Name.Replace("&quot;", "");
                    pageDetails.Name = pageDetails.Name.Replace(": Amazon.co.uk: TV", "");
                }

                catch (NullReferenceException e)
                {
                }


                //pageDetails.Price = htmlNode.OwnerDocument.DocumentNode.SelectSingleNode("//html/body/div[@id='a-page']/div[@id='dp']/div[@id='dp-container']/div[@id='ppd']/div[@id='centerCol']/div[@id='desktop_unifiedPrice']/div[@id='unifiedPrice_feature_div']/div[@id='price']/table[@class='a-lineitem']/tr[@id='priceblock_ourprice_row']/td[@class='a-span12']/span[@id='priceblock_ourprice']").InnerText;
                try
                {
                    pageDetails.Price = htmlNode.OwnerDocument.DocumentNode.SelectSingleNode("//html/body/div[@id='a-page']/div[@id='dp']/div[@id='dp-container']/div[@id='ppd']/div[@id='centerCol']/div[@id='desktop_unifiedPrice']/div[@id='unifiedPrice_feature_div']/div[@id='price']/table/tr/td/span").InnerText;
                }
                catch (NullReferenceException ex)
                {
                }

                var unformattedDescription = htmlNode.OwnerDocument.DocumentNode.SelectSingleNode("//div[@id='featurebullets_feature_div']/div[@id='feature-bullets']/ul").InnerText;
                var description = unformattedDescription.Replace("\n\n\n\n", "-");
                description = unformattedDescription.Replace("\n", "");
                description = description.Replace("This fits your&nbsp;. Make sure this fitsby entering your model number.P.when(\"ReplacementPartsBulletLoader\").execute(function(module){ module.initializeDPX(); })", "");
                //description = unformattedDescription.Replace("-","\n");

                pageDetails.Description = description;

                pageDetails.Url = url;


                var searchTermInTitle = pageDetails.Name.ToLower().Contains(searchTerm.ToLower());

                if (searchTermInTitle)
                {
                    lstPageDetails.Add(pageDetails);
                }
            }
            return lstPageDetails;
        }





    }
}
