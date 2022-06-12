using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Matcha_Editor.MVVM.Model
{
    public class InspectorComponentFieldModel
    {
        [JsonPropertyName("name")]
        public String Name { get; set; }
        [JsonPropertyName("type")]
        public String Type { get; set; }
        [JsonPropertyName("values")]
        public dynamic Values { get; set; }
        [JsonPropertyName("properties")]
        public IDictionary<string, IDictionary<string, dynamic>> Properties { get; set; }
    }
}
