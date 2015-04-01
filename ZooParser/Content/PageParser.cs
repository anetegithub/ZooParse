using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ZooParser.Content
{
    public class PageParser : Parser
    {
        public delegate void PageParsedDelegate(String ParsedString);

        public PageParsedDelegate OnParsed;

        public void ParsePage(String Url)
        {
            this.Parsed = () =>
            {
                var OffersBlock = Elements.Where
                    (x => (x.Name == "div" && x.Attributes["class"] != null && x.Attributes["class"].Value.Contains("offerlist"))).ToList();
                if (OffersBlock.Count == 0)
                    this.OnParsed("");

                var Offers = OffersBlock[0].Descendants("div").Where
                    (x => x.Attributes["class"] != null && x.Attributes["class"].Value.Contains("offer_panel")).ToList();

                foreach (var Offer in Offers)
                {
                    OnParsed("http://www.zoo-zoo.ru/" + Offer.Descendants("h2").ToList()[0].ChildNodes[0].Attributes["href"].Value);
                }
            };
            this.Parse(Url);
        }
    }
}