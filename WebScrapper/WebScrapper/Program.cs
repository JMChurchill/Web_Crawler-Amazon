using System;
using System.Collections.Generic;
using HtmlAgilityPack;
using ScrapySharp.Exceptions;
using ScrapySharp.Extensions;
using ScrapySharp.Network;
using System.IO;
using System.Globalization;
using CsvHelper;
using WebScrapper.Controller;

namespace WebScrapper
{
    class Program
    {
        //must download ScrapySharp nuget
        //note is duplicating results
        static ScrapingBrowser _scrapingBrowser = new ScrapingBrowser();
        static void Main(string[] args)
        {
            string termToSearchFor = "movie";
            string url = "https://www.amazon.co.uk/s?k=" + termToSearchFor + "&ref=nb_sb_noss_1";

            string searchTerm = "";

            //get details from each product on search page
            ScrapePageDetails detailPageResults = new ScrapePageDetails();
            var searchLinks = detailPageResults.getSearchPageLinks("https://www.amazon.co.uk/s?k=tv&ref=nb_sb_noss_1");
            var lstpageDetails = detailPageResults.GetPageDetails(searchLinks, searchTerm);
            detailPageResults.exportToCsv(lstpageDetails, searchTerm);

            //get product info from search page
            ScrapeSearchPage searchResults = new ScrapeSearchPage();
            var lstsearchPageDetails = searchResults.GetDetailsFromSearchResults(url);
            searchResults.exportToCsv(lstsearchPageDetails, searchTerm);

            //var searchLinks = getSearchPageLinks("https://www.amazon.co.uk/s?k=tv&ref=nb_sb_noss_1");
            //var searchTerm = "";//temp
            ////var mainLinks = GetMainPageLinks("https://devon.craigslist.org/d/web-html-info-design/search/web");//craigs

            //var lstpageDetails = GetPageDetails(searchLinks, searchTerm);
            //exportToCsv(lstpageDetails, searchTerm);
        }

        //    static List<string> GetMainPageLinks(string url)
        //    {
        //        var homePageLinks = new List<string>();
        //        var html = GetHtml(url);
        //        var links = html.CssSelect("a");
        //        foreach(var link in links)
        //        {
        //            //if (link.Attributes["href"].Value.Contains(".html")) //craigs
        //            //{
        //            //    homePageLinks.Add(link.Attributes["href"].Value);
        //            //}
        //            if (link.Attributes["href"].Value.Contains("ref=sr"))
        //            {
        //                homePageLinks.Add(link.Attributes["href"].Value);
        //            }
        //        }
        //        return homePageLinks;
        //    }
        //    public static List<string> getSearchPageLinks(string url)
        //    {
        //        var searchPageLinks = new List<string>();
        //        var htmlNode = GetHtml(url);
        //        var temp = htmlNode.OwnerDocument;

        //        var searchResults = temp.DocumentNode.SelectNodes("//html/body/div[@id='a-page']/div[@id='search']/div[@class='s-desktop-width-max s-desktop-content sg-row']/div[@class='sg-col-16-of-20 sg-col sg-col-8-of-12 sg-col-12-of-16']/div[@class='sg-col-inner']/span[@class='rush-component s-latency-cf-section']/div[@class='s-main-slot s-result-list s-search-results sg-row']/div[@data-component-type='s-search-result']");

        //        foreach (var result in searchResults)
        //        {
        //            //find link from each result
        //            var thisResult = result;

        //            var item = thisResult.SelectSingleNode(".//a[@class='a-link-normal a-text-normal']");


        //            string link = "https://www.amazon.co.uk" + item.Attributes["href"].Value;
        //            link = link.Replace("amp;","");
        //            searchPageLinks.Add(link);
        //        }
        //        return searchPageLinks;
        //    }

        //    static List<ProductPageDetails> GetPageDetails(List<string> urls, string searchTerm)
        //    {
        //        var lstPageDetails = new List<ProductPageDetails>();
        //        foreach (var url in urls)
        //        {
        //            var htmlNode = GetHtml(url);
        //            var pageDetails = new ProductPageDetails();

        //            try
        //            {
        //                pageDetails.Name = htmlNode.OwnerDocument.DocumentNode.SelectSingleNode("//html/head/title").InnerText;
        //                pageDetails.Name = pageDetails.Name.Replace("&#39;&#39;", "");
        //                pageDetails.Name = pageDetails.Name.Replace("&quot;", "");
        //                pageDetails.Name = pageDetails.Name.Replace(": Amazon.co.uk: TV", "");
        //            }

        //            catch (NullReferenceException e)
        //            {
        //            }


        //            //pageDetails.Price = htmlNode.OwnerDocument.DocumentNode.SelectSingleNode("//html/body/div[@id='a-page']/div[@id='dp']/div[@id='dp-container']/div[@id='ppd']/div[@id='centerCol']/div[@id='desktop_unifiedPrice']/div[@id='unifiedPrice_feature_div']/div[@id='price']/table[@class='a-lineitem']/tr[@id='priceblock_ourprice_row']/td[@class='a-span12']/span[@id='priceblock_ourprice']").InnerText;
        //            try
        //            {
        //                pageDetails.Price = htmlNode.OwnerDocument.DocumentNode.SelectSingleNode("//html/body/div[@id='a-page']/div[@id='dp']/div[@id='dp-container']/div[@id='ppd']/div[@id='centerCol']/div[@id='desktop_unifiedPrice']/div[@id='unifiedPrice_feature_div']/div[@id='price']/table/tr/td/span").InnerText;
        //            }
        //            catch (NullReferenceException ex)
        //            {
        //            }

        //            pageDetails.Url = url;

        //            var searchTermInTitle = pageDetails.Name.ToLower().Contains(searchTerm.ToLower());

        //            if (searchTermInTitle)
        //            {
        //                lstPageDetails.Add(pageDetails);
        //            }
        //        }
        //        return lstPageDetails;
        //    }

        //static void exportToCsv(List<ProductPageDetails> lstPageDetails, string searchTerm)
        //{
        //    using (var writer = new StreamWriter($@"/users/joshu/downloads/{searchTerm}_{DateTime.Now.ToFileTime()}.csv"))
        //    using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
        //    {
        //        csv.WriteRecords(lstPageDetails);
        //    }
        //}

        //    static HtmlNode GetHtml(string url)
        //    {
        //        WebPage webpage = _scrapingBrowser.NavigateToPage(new Uri(url));
        //        return webpage.Html;
        //    }
    }

    //public class ProductPageDetails
    //{
    //    public string Name { get; set; }
    //    public string Price { get; set; }
    //    //public string Description { get; set; }
    //    public string Url { get; set; }
    //}
}
