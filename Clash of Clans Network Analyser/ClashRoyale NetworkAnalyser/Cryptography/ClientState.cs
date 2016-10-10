using Sodium;

namespace ClashRoyale_NetworkAnalyser
{
    public class ClientState : State
    {
        public ServerState serverState;

        public KeyPair clientKey;
        public byte[] serverKey, nonce;
    }
}
