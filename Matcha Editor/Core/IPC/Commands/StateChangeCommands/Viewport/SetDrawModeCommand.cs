using System.Text.Json;
using System.Text.Json.Serialization;

namespace Matcha_Editor.Core.IPC.Command
{
    class SetDrawModeCommand : CommandBase
    {
        public CommandLayout m_Command { get; }

        public class CommandLayout
        {
            public string Command { get; set; }

            public ArgumentLayout Args { get; set; }

            public class ArgumentLayout
            {
                public string DrawMode { get; set; }
            }
        }

        public SetDrawModeCommand(string drawMode)
        {
            m_Command = new CommandLayout
            {
                Command = "setdrawmode",
                Args = new CommandLayout.ArgumentLayout {
                    DrawMode = drawMode
                } 
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
