using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.Net.Http;
using System.Net;

using HtmlAgilityPack;

using System.Text;

using System.Threading.Tasks;

namespace ZooParser.Content
{
    public class Parser : IParser
    {
        public Action Parsed;

        public List<HtmlNode> Elements { get; set; }

        public void Parse(String Http)
        {
            //404
            try
            {
                WebClient client = new WebClient();

                var response = client.DownloadData(Http);

                String source = Encoding.GetEncoding("utf-8").GetString(response, 0, response.Length - 1);
                source = WebUtility.HtmlDecode(source);
                HtmlDocument resultat = new HtmlDocument();
                resultat.LoadHtml(source);

                Elements = resultat.DocumentNode.Descendants().ToList();

                Parsed();
            }
            catch { }
        }

        public void Dispose()
        { }
    }
}