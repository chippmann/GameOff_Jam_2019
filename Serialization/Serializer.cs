using Newtonsoft.Json;

namespace GameOff_2019.Serialization {
    public class Serializer {
        public static string Serialize<T>(T obj) {
            return JsonConvert.SerializeObject(obj);
        }

        public static T Deserialize<T>(string json) {
            return JsonConvert.DeserializeObject<T>(json);
        }
    }
}