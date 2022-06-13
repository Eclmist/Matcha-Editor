using System;
using System.Text.Json.Serialization;

namespace Matcha_Editor.MVVM.Model
{
    public class InspectorComponentModel
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }
        [JsonPropertyName("guid")]
        public string Guid { get; set; }
        [JsonPropertyName("entity_guid")]
        public string EntityGuid { get; set; }
        [JsonPropertyName("enabled")]
        public bool Enabled { get; set; }

        [JsonPropertyName("fields")]
        public InspectorComponentFieldModel[] Fields { get; set; }
    }
}
