using Matcha_Editor.Core.IPC.Command;
using Matcha_Editor.MVVM.Model;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Matcha_Editor.Core.IPC.Commands.DataSetCommands
{
    class SetComponentCommand : CommandBase
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
                [JsonPropertyName("component")]
                public InspectorComponentModel Component { get; set; }
            }
        }

        public SetComponentCommand(InspectorComponentModel component)
        {
            m_Command = new CommandLayout
            {
                Command = "setcomponent",
                Args = new CommandLayout.ArgumentLayout
                {
                    Component = component
                }
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
