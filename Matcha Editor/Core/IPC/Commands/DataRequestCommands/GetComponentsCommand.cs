using Matcha_Editor.MVVM.Model;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Matcha_Editor.Core.IPC.Command
{
    class GetComponentsCommand : CommandBase
    {
        public CommandLayout m_Command { get; }

        public class CommandLayout
        {
            [JsonPropertyName("command")]
            public string Command { get; set; }

            [JsonPropertyName("args")]
            public ArgumentLayout Args { get; set; }

            public class ArgumentLayout
            {
                [JsonPropertyName("entityguid")]
                public string EntityGuid { get; set; }
            }
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
                    [JsonPropertyName("components")]
                    public ComponentModel[] Components { get; set; }
                }
            }
        }

        public GetComponentsCommand(string guid)
        {
            m_Command = new CommandLayout
            {
                Command = "getcomponents",
                Args = new CommandLayout.ArgumentLayout { EntityGuid = guid }
            };
        }

        public override byte[] ToBytes()
        {
            return System.Text.Encoding.ASCII.GetBytes(ToJson());
        }

        public override string ToJson()
        {
            return JsonSerializer.Serialize(m_Command);
        }
    }
}
