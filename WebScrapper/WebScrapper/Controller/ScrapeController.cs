using CsvHelper;
using HtmlAgilityPack;
using ScrapySharp.Network;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using WebScrapper.Model;

namespace WebScrapper.Controller
{
    class ScrapeController
    {
        protected static ScrapingBrowser _scrapingBrowser = new ScrapingBrowser();
        protected HtmlNode GetHtml(string url)
        {
            WebPage webpage = _scrapingBrowser.NavigateToPage(new Uri(url));
            return webpage.Html;
        }

        public void exportToCsv(List<ProductDetails> lstPageDetails, string searchTerm)
        {
            using (var writer = new StreamWriter($@"/users/joshu/downloads/{searchTerm}_{DateTime.Now.ToFileTime()}.csv"))
            using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
            {
                csv.WriteRecords(lstPageDetails);
            }
        }

    }
}
