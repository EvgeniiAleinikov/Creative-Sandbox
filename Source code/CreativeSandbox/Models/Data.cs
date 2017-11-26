using Newtonsoft.Json;

namespace CreativeSandbox.Models
{
    public class Data
    {
        [JsonProperty("startX")]
        public float StartX { get; set; }
        [JsonProperty("startY")]
        public float StartY { get; set; }
        [JsonProperty("endX")]
        public float EndX { get; set; }
        [JsonProperty("endY")]
        public float EndY { get; set; }
    }
}