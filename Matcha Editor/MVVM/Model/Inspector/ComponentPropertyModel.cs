using System;
using System.Text.Json.Serialization;

namespace Matcha_Editor.MVVM.Model
{
    public class ComponentPropertyModel
    {
        [JsonPropertyName("name")]
        public String Name { get; set; }
        [JsonPropertyName("type")]
        public String Type { get; set; }
        [JsonPropertyName("values")]
        public dynamic Values { get; set; }
    }
}
