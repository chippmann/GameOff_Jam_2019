using System;

namespace GameOff_2019.Data {
    public class Tweet {
        public string created_at = DateTime.Now.ToString("ddd MMM dd HH:mm:ss zzzz yyyy");
        public long id { get; set; }
        public string text { get; set; }
        public Entities entities { get; set; }
        public User user { get; set; }
        public int retweet_count { get; set; }
        public int favorite_count { get; set; }
    }
}