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
        //must download ScrapySharp nuget to work -> look for another solution
        static ScrapingBrowser _scrapingBrowser = new ScrapingBrowser();
        static void Main(string[] args)
        {
            string termToSearchFor = "movie";
            //string url = "https://www.amazon.co.uk/s?k=" + termToSearchFor + "&ref=nb_sb_noss_1";

            //works with catagory page
            string url = "https://www.amazon.co.uk/s?i=electronics&bbn=560798&rh=n%3A560798%2Cp_6%3AA3P5ROKL5A1OLE&dc&qid=1612380873&rnid=419151031&ref=sr_pg_1";

            string searchTerm = "";

            //get details from each product on search page
            //ScrapePageDetails detailPageResults = new ScrapePageDetails();
            //var searchLinks = detailPageResults.getSearchPageLinks("https://www.amazon.co.uk/s?k=tv&ref=nb_sb_noss_1");
            //var lstpageDetails = detailPageResults.GetPageDetails(searchLinks, searchTerm);
            //detailPageResults.exportToCsv(lstpageDetails, searchTerm);

            //get product info from search page
            ScrapeSearchPage searchResults = new ScrapeSearchPage();
            var lstsearchPageDetails = searchResults.GetDetailsFromSearchResults(url);
            searchResults.exportToCsv(lstsearchPageDetails, searchTerm);

        }

    }
}
