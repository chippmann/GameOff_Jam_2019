using Godot;

namespace GameOff_2019.Data {
    public class RestResponse {
        public HTTPRequest.Result result { get; set; }
        public int responseCode { get; set; }
        public string[] headers { get; set; }
        public string body { get; set; }
    }
}