using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WebSocketClient
{
    class Program
    {
        private static readonly Uri _serverUri = new Uri(@"ws://120.78.72.20:1935");


        static void Main(string[] args)
        {

#if NETCORE
            Console.Title = ".NET Core";

#else
            Console.Title = ".NET Framework";
#endif

            MainAsync(args).GetAwaiter().GetResult();
        }

        static async Task MainAsync(string[] args)
        {
            ClientWebSocket clientWebSocket = new ClientWebSocket();

            bool retry = true;
            while (retry)
            {
                try
                {
                    Console.WriteLine(clientWebSocket.State);
                    await clientWebSocket.ConnectAsync(_serverUri, CancellationToken.None);
                    retry = false;
                }
                catch (WebSocketException ex)
                {
                    Console.WriteLine(ex.Message);

                    // HACK: Fixes 'The WebSocket has already been started.' exception
                    clientWebSocket.Abort();
                    clientWebSocket = new ClientWebSocket();
                }
            }

            new Thread(async () =>
            {
                byte[] buffer = new byte[1024 * 4];

                while (clientWebSocket.State == WebSocketState.Open)
                {
                    WebSocketReceiveResult result = await clientWebSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

                    switch (result.MessageType)
                    {
                        case WebSocketMessageType.Binary:
                            break;
                        case WebSocketMessageType.Close:
                            await clientWebSocket.CloseOutputAsync(WebSocketCloseStatus.NormalClosure, "", CancellationToken.None);
                            break;
                        case WebSocketMessageType.Text:
                            string message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                            Console.WriteLine($"<<: {message}");
                            break;
                    }
                }
            }).Start();

            while (clientWebSocket.State == WebSocketState.Open)
            {
                //string message = "{ \"name\":\"mspace\",\"type\":205}";
                string message = Console.ReadLine();

                if (!string.IsNullOrEmpty(message))
                {
                    byte[] bytes = Encoding.UTF8.GetBytes(message);
                    await clientWebSocket.SendAsync(new ArraySegment<byte>(bytes), WebSocketMessageType.Text, true, CancellationToken.None);
                    Console.WriteLine($">>: {message}");
                }
            }
        }
    }
}
