using System.Collections.Generic;

namespace Planty.Data {
    public class Entities {
        public List<Hashtag> hashtags { get; set; }
        public List<Url> urls { get; set; }
        public List<object> symbols { get; set; }
        public List<UserMention> user_mentions { get; set; }
    }
}