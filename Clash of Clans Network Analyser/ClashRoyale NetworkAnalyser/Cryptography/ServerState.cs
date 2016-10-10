﻿using Sodium;

namespace ClashRoyale_NetworkAnalyser
{
    public class ServerState : State
    {
        public ClientState clientState;

        public KeyPair serverKey;
        public byte[] clientKey, nonce, sessionKey, sharedKey;
    }
}
