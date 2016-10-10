using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace ClashRoyale_NetworkAnalyser
{
    public class Server : ServerCrypto
    {
        private int port;
        private static ManualResetEvent allDone = new ManualResetEvent(false);

        private static ServerState srvrState;
        private static ClientState clientState;

        public Server(int port)
        {
            this.port = port;
        }

        public void StartServer()
        {
            try
            {
                IPEndPoint localEndPoint = new IPEndPoint(IPAddress.Any, this.port);
                Socket listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                listener.Bind(localEndPoint);
                listener.Listen(100);

                Console.WriteLine("Listening on 0.0.0.0:{0} ...", this.port);

                while (true)
                {
                    Server.allDone.Reset();
                    listener.BeginAccept(new AsyncCallback(Server.AcceptCallback), listener);
                    Server.allDone.WaitOne();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: {0}", e.Message);
            }
            Console.WriteLine("\nPress ENTER to continue...");
            Console.Read();
        }

        public static void AcceptCallback(IAsyncResult ar)
        {
            Server.allDone.Set();
            try
            {
                Socket listener = (Socket)ar.AsyncState;
                Socket socket = listener.EndAccept(ar);

                srvrState = new ServerState();
                srvrState.socket = socket;
                srvrState.serverKey = ServerCrypto.serverKey;

                Console.WriteLine("Connection from {0} ...", socket.RemoteEndPoint.ToString());

                Client client = new Client(srvrState);
                client.StartClient();
                client.state.serverState = srvrState;
                clientState = client.state;
                srvrState.clientState = client.state;

                socket.BeginReceive(srvrState.buffer, 0, State.BufferSize, 0, new AsyncCallback(Protocol.ReceiveCallback), srvrState);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                throw;
            }
        }

        public static void ReloadDefinitions()
        {
            srvrState?.ReloadDefinitions();
            clientState?.ReloadDefinitions();
        }
    }
}
