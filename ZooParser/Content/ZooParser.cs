using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using ZooParser.DataLayer;

using System.Threading.Tasks;

namespace ZooParser.Content
{
    public class ZooParser : Parser
    {
        public delegate void PostParsedDelegate(Post ParsedPost);

        public PostParsedDelegate OnParsed;

        public void ParsePost(String Url, Animal Animal, DealType DealType)
        {
            this.Parsed = () =>
            {
                DateTime PostDate = DateTime.Now;

                Post NewPost = new Post();

                NewPost.Title = Elements.Where
                    (x => (x.Name == "div" && x.Attributes["class"] != null && x.Attributes["class"].Value.Contains("topinfo"))).ToList()[0].Descendants("h1").ToList()[0].InnerText;

                var DateNotSplitted = Elements.Where
                    (x => (x.Name == "div" && x.Attributes["class"] != null && x.Attributes["class"].Value.Contains("more_infome"))).ToList()[0].Descendants("span").ToList()[0]
                    .Attributes[1].Value;
                DateNotSplitted = DateNotSplitted.Split(' ')[0] + " " + DateNotSplitted.Split(' ')[1] + ":00";
                if (!DateTime.TryParse(DateNotSplitted, out PostDate))
                    return;
                NewPost.Date = PostDate;

                NewPost.Information = Elements.Where
                    (x => (x.Name == "div" && x.Attributes["class"] != null && x.Attributes["class"].Value.Contains("message_once"))).ToList()[0].Descendants("div").ToList()[2].InnerText.Trim();

                var PriceBLock = Elements.Where
                    (x => (x.Name == "div" && x.Attributes["class"] != null && x.Attributes["class"].Value.Contains("once_price"))).ToList();
                if (PriceBLock.Count != 0)
                {
                    Decimal ParsedCost = 0.0M;
                    if (!Decimal.TryParse(PriceBLock[0].ChildNodes[1].ChildNodes[1].InnerText.Trim(), out ParsedCost))
                        return;
                    else
                        NewPost.Cost = ParsedCost;
                }

                //no autor block
                try
                {
                    var table = Elements.Where
                        (x => (x.Name == "div" && x.Attributes["class"] != null && x.Attributes["class"].Value.Contains("message_once"))).ToList()[0].Descendants("table").ToList();
                    NewPost.Autor = table[1].ChildNodes[1].ChildNodes[1].ChildNodes[1].ChildNodes[1].ChildNodes[3].InnerText;
                }
                catch { }

                //no additional block
                try
                {
                    var AdditionalUl = Elements.Where
                        (x => (x.Name == "ul" && x.Attributes["class"] != null && x.Attributes["class"].Value.Contains("once_list2"))).ToList()[0].Descendants("li").ToList();
                    foreach (var Element in AdditionalUl)
                    {
                        if (Element.Descendants("strong").ToList()[0].InnerText == "Телефон")
                            NewPost.Number = Element.InnerText.Replace("Телефон: ", "");
                    }
                }
                catch { }

                //no image
                try
                {
                    NewPost.ImageUrl = "http://www.zoo-zoo.ru" + Elements.Where
                        (x => (x.Name == "div" && x.Attributes["class"] != null && x.Attributes["class"].Value.Contains("message_images"))).ToList()[0].Descendants("table").ToList()[0]
                        .ChildNodes[1].ChildNodes[1].ChildNodes[1].Attributes[1].Value;
                }
                catch
                {
                    NewPost.Information = Elements.Where
                   (x => (x.Name == "div" && x.Attributes["class"] != null && x.Attributes["class"].Value.Contains("message_once"))).ToList()[0].Descendants("div").ToList()[1].InnerText.Trim();
                }

                NewPost.DealType = DealType;
                NewPost.Type = Animal;

                this.OnParsed(NewPost);
            };
            Parse(Url);
        }
    }
}