using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.Data.Entity;

namespace ZooParser.DataLayer
{
    public class Post
    {
        public Int32 Id { get; set; }
        public String Title { get; set; }
        public String Information { get; set; }
        public Decimal Cost { get; set; }
        public String Autor { get; set; }
        public String Number { get; set; }
        public DateTime Date { get; set; }
        public Animal Type { get; set; }
        public DealType DealType { get; set; }
        public String ImageUrl { get; set; }
        public Byte[] Image { get; set; }

        public override bool Equals(object obj)
        {
            Post a = this;
            //unbox, bad perfomance
            Post b = (Post)obj;
            if (
                a.Autor == b.Autor &&
                a.Cost == b.Cost &&
                a.Date.Date.Day == b.Date.Date.Day &&
                a.Date.Date.Month == b.Date.Date.Month &&
                a.Date.Date.Year == b.Date.Date.Year &&
                             a.DealType == b.DealType &&
                             a.ImageUrl == b.ImageUrl &&
                             a.Information == b.Information &&
                             a.Number == b.Number &&
                             a.Title == b.Title &&
                             a.Type == b.Type
            )
                return true;
            else
                return false;
        }
    }

    public enum Animal
    {
        dogs = 0, cats = 1, birds = 2, rodents = 3, reptile = 4, fish = 5, insects = 6, horses = 7, Home = 8, exotic = 9, All = 10
    }

    public enum DealType
    { sale = 0, buy = 1, love = 2, All = 3, free = 4 }
}