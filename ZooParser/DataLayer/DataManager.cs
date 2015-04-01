using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.Data.Entity;

namespace ZooParser.DataLayer
{
    internal static class DataManager
    {
        public delegate bool dbCondition(Post Source);

        public static List<Post> All
        {
            get
            {
                using (var db = new PostContext())
                {
                    try
                    {
                        var query = (from b
                                         in db.Posts
                                     select b).ToList();
                        return query;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                    return new List<Post>();
                }
            }
        }

        public static List<Post> Select(dbCondition[] Conditions)
        {
            if (Conditions.Length == 0)
                return DataManager.All;

            using (var db = new PostContext())
            {
                try
                {
                    //why entity do this?
                    //
                    //var Query = (from b in db.Posts
                    //             where Conditions[0](b)
                    //             select b);

                    //foreach (dbCondition Condition in Conditions.Skip(1))
                    //{
                    //    Query = from a in Query where Condition(a) select a;
                    //}

                    //return Query.ToList();
                    List<Post> Selected = new List<Post>();
                    foreach(Post Post in db.Posts)
                    {
                        bool add = true;
                        foreach (dbCondition Condition in Conditions)
                        {
                            if (!Condition(Post))
                                add = false;
                        }
                        if (add)
                            Selected.Add(Post);
                    }
                    return Selected;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            return new List<Post>();
        }

        public static Post Add
        {
            set
            {
                using (var db = new PostContext())
                {
                    Post b = value;
                    db.Configuration.AutoDetectChangesEnabled = false;
                    List<Post> Duplicates=new List<Post>();

                    //linq to entity framework is big bad present                    
                    //if(db.Posts.ToList().Count!=0)
                    //    Duplicates = (from a in db.Posts where a.Equals(b) select a).ToList();
                    Duplicates = db.Posts.ToList();
                    var d= Duplicates.Where(x => x.Equals(b)).ToList();

                    if (d.Count == 0)
                    {
                        db.Posts.Add(value);
                        db.ChangeTracker.DetectChanges();
                        db.SaveChanges();
                    }
                }
            }
        }

        /// <summary>
        /// cuz Add hurt perfomance
        /// </summary>
        public static List<Post> AddList
        {
            set
            {
                using (var db = new PostContext())
                {
                    List<Post> lb = value;
                    foreach (var b in lb)
                    {
                        db.Configuration.AutoDetectChangesEnabled = false;
                        List<Post> Duplicates = new List<Post>();

                        Duplicates = db.Posts.ToList();
                        var d = Duplicates.Where(x => x.Equals(b)).ToList();

                        if (d.Count == 0)
                        {
                            db.Posts.Add(b);                           
                        }
                    }
                    db.ChangeTracker.DetectChanges();
                    db.SaveChanges();
                }
            }
        }
    }

    public class PostContext : DbContext
    {
        public DbSet<Post> Posts { get; set; }
    }
}