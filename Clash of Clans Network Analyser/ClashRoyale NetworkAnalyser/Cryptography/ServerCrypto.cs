using System;
using System.Net.Sockets;
using System.Linq;
using ClashRoyale_NetworkAnalyser.Messages;
using ClashRoyale_NetworkAnalyser.Utils;
using Sodium;
using Newtonsoft.Json.Linq;

namespace ClashRoyale_NetworkAnalyser
{
    public class ServerCrypto : Protocol
    {
        protected static KeyPair serverKey = PublicKeyBox.GenerateKeyPair(Utilities.HexToBinary("1891d401fadb51d25d3a9174d472a9f691a45b974285d47729c45c6538070d85"));

        public static void DecryptPacket(Socket socket, ServerState state, byte[] packet)
        {
            int messageId = BitConverter.ToInt32(new byte[2].Concat(packet.Take(2)).Reverse().ToArray(), 0);
            int payloadLength = BitConverter.ToInt32(new byte[1].Concat(packet.Skip(2).Take(3)).Reverse().ToArray(), 0);
            int unknown = BitConverter.ToInt32(new byte[2].Concat(packet.Skip(2).Skip(3).Take(2)).Reverse().ToArray(), 0);
            byte[] cipherText = packet.Skip(2).Skip(3).Skip(2).ToArray();
            byte[] plainText;

            if (messageId == 10100)
            {
                plainText = cipherText;
            }
            else if (messageId == 10101)
            {
                state.clientKey = cipherText.Take(32).ToArray();
                byte[] nonce = GenericHash.Hash(state.clientKey.Concat(state.serverKey.PublicKey).ToArray(), null, 24);
                cipherText = cipherText.Skip(32).ToArray();
                plainText = PublicKeyBox.Open(cipherText, nonce, state.serverKey.PrivateKey, state.clientKey);
                state.sessionKey = plainText.Take(24).ToArray();
                state.clientState.nonce = plainText.Skip(24).Take(24).ToArray();
                plainText = plainText.Skip(24).Skip(24).ToArray();
            }
            else
            {
                state.clientState.nonce = Utilities.Increment(Utilities.Increment(state.clientState.nonce));
                plainText = SecretBox.Open(new byte[16].Concat(cipherText).ToArray(), state.clientState.nonce, state.sharedKey);
            }

            try
            {
                JObject decoded = state.decoder.decode(messageId, 0, plainText);
                var msg = new Message(messageId, "[C > S] "+decoded["name"], plainText, unknown, state, decoded["fields"].ToString());
                MainWindow.Context.AddMessageToQueueLog(msg);
            }
            catch (Exception)
            {
                var msg = new Message(messageId, "[C > S] " + "Undefined", plainText, unknown, state, "");
                MainWindow.Context.AddMessageToQueueLog(msg);
            }


            ClientCrypto.EncryptPacket(state.clientState.socket, state.clientState, messageId, unknown, plainText);
        }

        public static void EncryptPacket(Socket socket, ServerState state, int messageId, int unknown, byte[] plainText)
        {
            byte[] cipherText;
            if (messageId == 20100 || (messageId == 20103 && state.sharedKey == null))
            {
                cipherText = plainText;
            }
            else if (messageId == 20103 || messageId == 20104)
            {
                byte[] nonce = GenericHash.Hash(state.clientState.nonce.Concat(state.clientKey).Concat(state.serverKey.PublicKey).ToArray(), null, 24);
                plainText = state.nonce.Concat(state.sharedKey).Concat(plainText).ToArray();
                cipherText = PublicKeyBox.Create(plainText, nonce, state.serverKey.PrivateKey, state.clientKey);
            }
            else
            {
                // nonce was already incremented in ClientCrypto.DecryptPacket
                cipherText = SecretBox.Create(plainText, state.nonce, state.sharedKey).Skip(16).ToArray();
            }
            byte[] packet = BitConverter.GetBytes(messageId).Reverse().Skip(2).Concat(BitConverter.GetBytes(cipherText.Length).Reverse().Skip(1)).Concat(BitConverter.GetBytes(unknown).Reverse().Skip(2)).Concat(cipherText).ToArray();
            socket.BeginSend(packet, 0, packet.Length, 0, new AsyncCallback(SendCallback), state);
        }
    }
}
