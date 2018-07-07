using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;
using WebSocketServer.Managers;

namespace WebSocketServer.Handlers
{
    public class ExampleWebSocketHandler : WebSocketHandler
    {
        public ExampleWebSocketHandler(WebSocketConnectionManager webSocketConnectionManager) : base(webSocketConnectionManager)
        {

        }

        public override async Task OnConnected(WebSocket socket)
        {
            await base.OnConnected(socket);
            string token = WebSocketConnectionManager.GetId(socket);
            await SendMessageToAllAsync($"{token} connected.");
        }

        public override async Task OnDisconnected(WebSocket socket)
        {
            await base.OnDisconnected(socket);
            string token = WebSocketConnectionManager.GetId(socket);
            await SendMessageToAllAsync($"{token} disconnected.");
        }

        public override async Task ReceiveAsync(WebSocket socket, WebSocketReceiveResult result, byte[] buffer)
        {
            string message = $"{Encoding.UTF8.GetString(buffer, 0, result.Count)}";
            await SendMessageToAllAsync(message);
        }
    }
}
