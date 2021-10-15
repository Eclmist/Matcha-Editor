namespace Matcha_Editor.Core.IPC.Command
{
    abstract class CommandBase
    {
        public abstract byte[] ToBytes();
        public abstract string ToJson();
    }
}
