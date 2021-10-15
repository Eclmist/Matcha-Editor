using System.Text.Json;
using System.Text.Json.Serialization;

namespace Matcha_Editor.Core.IPC.Command
{
    class GetTopLevelEntitiesCommand : CommandBase
    {
        public CommandLayout m_Command { get; }

        public class CommandLayout
        {
            public string Command { get; set; }
        }

        public class Response
        {
            public ResponseLayout ResponseData { get; set; }

            public Response(string rawResponse)
            {
                ResponseData = JsonSerializer.Deserialize<ResponseLayout>(rawResponse);
            }

            public class ResponseLayout
            {
                [JsonPropertyName("command")]
                public string Command { get; set; }

                [JsonPropertyName("args")]
                public ArgumentLayout Args { get; set; }

                public class ArgumentLayout
                {
                    [JsonPropertyName("entities")]
                    public EntityLayout[] Entities { get; set; }

                    public class EntityLayout
                    {
                        [JsonPropertyName("name")]
                        public string Name { get; set; }
                        [JsonPropertyName("guid")]
                        public string Guid { get; set; }
                        [JsonPropertyName("parent")]
                        public string Parent { get; set; }
                        [JsonPropertyName("enabled")]
                        public bool Enabled { get; set; }
                    }    
                }
            }
        }

        public GetTopLevelEntitiesCommand()
        {
            m_Command = new CommandLayout
            {
                Command = "gettoplevelentities"
            };
        }

        public override byte[] ToBytes()
        {
            return System.Text.Encoding.ASCII.GetBytes(ToJson());
        }

        public override string ToJson()
        {
            return JsonSerializer.Serialize(m_Command).ToLower();
        }
    }
}
