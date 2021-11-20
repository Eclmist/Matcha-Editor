using Matcha_Editor.Core.IPC.Command;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Matcha_Editor.Core.IPC.Commands
{
    internal class ImportAssetCommand : CommandBase
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
                [JsonPropertyName("path")]
                public string AssetPath { get; set; }
            }
        }

        public ImportAssetCommand(string path)
        {
            m_Command = new CommandLayout
            {
                Command = "importasset",
                Args = new CommandLayout.ArgumentLayout { AssetPath = path }
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
