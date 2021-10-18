using System;
using System.Text.Json.Serialization;

namespace Matcha_Editor.MVVM.Model
{
    public class ComponentModel
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }
        [JsonPropertyName("guid")]
        public string Guid { get; set; }
        [JsonPropertyName("enabled")]
        public bool Enabled { get; set; }
        [JsonPropertyName("isfixed")]
        public bool IsFixed { get; set; }

        [JsonPropertyName("properties")]
        public ComponentPropertyModel[] Properties { get; set; }
    }
}
