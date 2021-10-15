using System.Text.Json;
using System.Text.Json.Serialization;

namespace Matcha_Editor.Core.IPC.Command
{
    class InitializeCommand : CommandBase
    {
        public CommandLayout m_Command { get; }

        public class CommandLayout
        {
            public string Command { get; set; }

            public ArgumentLayout Args { get; set; }

            public class ArgumentLayout
            {
                public long Hwnd { get; set; }
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
                public string command { get; set; } // Not used but required for backwards compatibility with Cauldron.
                                                    // TODO: Remove?

                public ArgumentLayout args { get; set; }

                public class ArgumentLayout
                {
                    public long childhwnd { get; set; }
                }
            }
        }

        public InitializeCommand(System.IntPtr viewportHostHwnd)
        {
            m_Command = new CommandLayout
            {
                Command = "initialize",
                Args = new CommandLayout.ArgumentLayout {
                    Hwnd = viewportHostHwnd.ToInt64() 
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
