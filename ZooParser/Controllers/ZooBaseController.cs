using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

using System.Dynamic;

using System.Web.Script.Serialization;

using ZooParser.Content;

namespace ZooParser.Controllers
{
    public class ZooBaseController : ApiController
    {
        [HttpGet]
        public IHttpActionResult IsOnline()
        {
            dynamic RunTimeXml = new ExpandoObject();
            RunTimeXml.Answer = "Yes";
            RunTimeXml.Stamp = DateTime.Now.ToString();
            return Ok(RunTimeXml);
        }

        [ActionName("database")]
        public IHttpActionResult Base([FromBody]String Condition)
        {
            Models.Condition condition = new JavaScriptSerializer().Deserialize<Models.Condition>(Condition);
            return Ok(ProcessingManager.PostsByCondition(condition));
        }

        [HttpGet]
        public IHttpActionResult UpdateDb()
        {
            String Answer = "Nothing";
            try
            {
                List<String> Urls = new List<string>();
                List<String> Animals = new List<string>()
                {
                    "dogs",
                    "cats",
                    "birds",
                    "rodents",
                    "reptile",
                    "fish",
                    "insects",
                    "horses",
                    "Домашний-скот",
                    "exotic"
                };
                using (var Parser = new PageParser())
                {
                    List<String> Types = new List<string>() { "sale", "buy", "love", "free" };

                    List<DataLayer.Post> ParsedPosts = new List<DataLayer.Post>();

                    foreach (String AnimalPage in Animals)
                    {
                        foreach (String Type in Types)
                        {
                            using (var ZooParser = new Content.ZooParser())
                            {
                                Parser.OnParsed = (String ParsedUrl) =>
                                    {
                                        if (ParsedUrl != "")
                                        {
                                            DataLayer.DealType Dt = DataLayer.DealType.All;
                                            Enum.TryParse<DataLayer.DealType>(Type, out Dt);

                                            DataLayer.Animal An = DataLayer.Animal.All;
                                            Enum.TryParse<DataLayer.Animal>(AnimalPage, out An);

                                            ZooParser.OnParsed = (DataLayer.Post ParsedPost) =>
                                                {
                                                    ParsedPosts.Add(ParsedPost);
                                                };

                                            if (AnimalPage != "Домашний-скот")
                                                ZooParser.ParsePost(ParsedUrl, An, Dt);
                                            else
                                                ZooParser.ParsePost(ParsedUrl, DataLayer.Animal.Home, Dt);
                                        }
                                    };
                                Parser.ParsePage("http://www.zoo-zoo.ru/" + AnimalPage + "/" + Type + "/");

                            }
                        }
                    }

                    DataLayer.DataManager.AddList = ParsedPosts;
                    Answer = "True";
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Answer = "False";
            }
            return Ok(Answer);
        }
    }

    internal static class ProcessingManager
    {
        internal static List<DataLayer.Post> PostsByCondition(Models.Condition Condition)
        {
            List<DataLayer.DataManager.dbCondition> SelectConditions = new List<DataLayer.DataManager.dbCondition>();

            if (Condition.DateRange != "")
                ProcessingManager.ByDate(Condition, ref SelectConditions);

            if (Condition.CostRange != "")
                ProcessingManager.ByCost(Condition, ref SelectConditions);

            if (Condition.Animal != "")
                ProcessingManager.ByAnimal(Condition, ref SelectConditions);

            if (Condition.Deal != "")
                ProcessingManager.ByDeal(Condition, ref SelectConditions);

            return DataLayer.DataManager.Select(SelectConditions.ToArray());

        }

        internal static void ByDate(Models.Condition Condition, ref List<DataLayer.DataManager.dbCondition> SelectConditions)
        {
            DateTime min = DateTime.MinValue;
            DateTime max = DateTime.Now;
            DateTime.TryParse(Condition.DateRange.Split('-')[0], out min);
            DateTime.TryParse(Condition.DateRange.Split('-')[1], out max);

            if (max == DateTime.MinValue)
                max = DateTime.Now;

            SelectConditions.Add((DataLayer.Post CurrentPost) =>
            {
                if (CurrentPost.Date > min && CurrentPost.Date < max)
                    return true;
                else
                    return false;
            });
        }

        internal static void ByCost(Models.Condition Condition, ref List<DataLayer.DataManager.dbCondition> SelectConditions)
        {
            Decimal min = Decimal.MinValue;
            Decimal max = Decimal.MaxValue;

            Decimal.TryParse(Condition.CostRange.Split('-')[0], out min);
            Decimal.TryParse(Condition.CostRange.Split('-')[1], out max);

            if (max == 0)
                max = Decimal.MaxValue;

            SelectConditions.Add((DataLayer.Post CurrentPost) =>
            {
                if (CurrentPost.Cost >= min && CurrentPost.Cost <= max)
                    return true;
                else
                    return false;
            });
        }

        internal static void ByAnimal(Models.Condition Condition, ref List<DataLayer.DataManager.dbCondition> SelectConditions)
        {
            DataLayer.Animal Animal = DataLayer.Animal.All;

            Enum.TryParse<DataLayer.Animal>(Condition.Animal, out Animal);

            SelectConditions.Add((DataLayer.Post CurrentPost) =>
            {
                if (Animal == DataLayer.Animal.All)
                    return true;
                else
                    if (CurrentPost.Type == Animal)
                        return true;
                    else
                        return false;
            });
        }

        internal static void ByDeal(Models.Condition Condition, ref List<DataLayer.DataManager.dbCondition> SelectConditions)
        {
            DataLayer.DealType DealType = DataLayer.DealType.All;

            Enum.TryParse<DataLayer.DealType>(Condition.Deal, out DealType);

            SelectConditions.Add((DataLayer.Post CurrentPost) =>
            {
                if (DealType == DataLayer.DealType.All)
                    return true;
                else
                    if (CurrentPost.DealType == DealType)
                        return true;
                    else
                        return false;
            });
        }

        //public static List<DataLayer.Post> DataByParse(DateTime Date)
        //{
        //    using (var Parser = new Content.ZooParser())
        //    {
        //        String Url = "http://www.zoo-zoo.ru/messages/263554-Shenki-foksterera.html";
        //        Parser.ParsePost(Date, Url, DataLayer.Animal.Dog, DataLayer.DealType.Buy);
        //    }

        //    return new List<DataLayer.Post>();
        //}
    }
}
