using System.Collections.Generic;

namespace Planty.Data {
    public class UserMention {
        public string screen_name { get; set; }
        public List<int> indices { get; set; }
    }
}