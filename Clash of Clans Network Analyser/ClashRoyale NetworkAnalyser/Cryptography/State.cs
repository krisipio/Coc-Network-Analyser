using System.Net.Sockets;
using coc_messages_csharp;

namespace ClashRoyale_NetworkAnalyser
{
    public class State
    {
        public Socket socket = null;
        public const int BufferSize = 1024;
        public byte[] buffer = new byte[BufferSize];
        public byte[] packet = new byte[0];
        public Decoder decoder;

        public State()
        {
            this.decoder = new Decoder();
        }

        public void ReloadDefinitions()
        {
            this.decoder.ReloadDefinitions();
        }
    }
}
