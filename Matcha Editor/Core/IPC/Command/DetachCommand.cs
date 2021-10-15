using System.Text.Json;
using System.Text.Json.Serialization;

namespace Matcha_Editor.Core.IPC.Command
{
    class DetachCommand : CommandBase
    {
        public CommandLayout m_Command { get; }

        public class CommandLayout
        {
            public string Command { get; set; }
        }

        public DetachCommand()
        {
            m_Command = new CommandLayout
            {
                Command = "detach",
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
