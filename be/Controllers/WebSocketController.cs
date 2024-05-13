using System.Net.Sockets;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using WebSocketsSample.common;

namespace WebSocketsSample.Controllers;

// <snippet>
public class WebSocketController : ControllerBase
{
    private readonly GroupManager _groupManager;
    private readonly WebSocketConnectionManager _websocketConnectionManager;

    public WebSocketController(GroupManager groupManager, WebSocketConnectionManager webSocketConnectionManager)
    {
        _groupManager = groupManager;
        _websocketConnectionManager = webSocketConnectionManager;
    }

    [HttpGet("/ws")]
    public async Task Get([FromQuery] string username)
    {
        if (HttpContext.WebSockets.IsWebSocketRequest)
        {
            var connId = HttpContext.Connection.Id;
            using var webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();

            // Add connection to managed colelction
            _websocketConnectionManager.AddSocket(webSocket, connId, username);

            await Echo(webSocket, connId, _groupManager, _websocketConnectionManager);
        }
        else
        {
            HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
        }
    }
    // </snippet>

    private static async Task Echo(WebSocket webSocket, string connId, GroupManager groupManager, WebSocketConnectionManager webSocketConnectionManager)
    {
        var buffer = new byte[1024 * 4];
        var receiveResult = await webSocket.ReceiveAsync(
            new ArraySegment<byte>(buffer), CancellationToken.None);

        var workingUser = webSocketConnectionManager.GetSocketById(connId);

        while (!receiveResult.CloseStatus.HasValue)
        {
            if(receiveResult.MessageType == WebSocketMessageType.Text){
                KeyValuePair<string, HashSet<string>> room = new();
                IEnumerable<WebSocket> roomSockets = new List<WebSocket>();
                ICollection<WebSocket> sockets = new List<WebSocket>();
                WebSocket? socket = null;
                string? jsonString = Encoding.UTF8.GetString(buffer, 0, receiveResult.Count);
                MessageType? message = JsonSerializer.Deserialize<MessageType>(jsonString);
                if (message is not null)
                {
                    MessageSend messageSend = new MessageSend()
                    {
                        Data = message.Data,
                        From = workingUser.Username
                    };

                    switch (message.Event)
                    {
                        case "JoinRoom":
                            groupManager.AddConnectionToAGroup(message.RoomName, connId);
                            var notifyMessage = new MessageSend()
                            {
                                Data = $"{workingUser.Username} joined group {message.RoomName} successfully",
                                Event = "JoinRoom",
                                RoomName = message.RoomName,
                                From = workingUser.Username
                            };
                            room = groupManager.GetGroupByName(message.RoomName);
                            roomSockets = webSocketConnectionManager.GetSocketsByListConnId(room.Value).Select(u => u.Websocket);
                            foreach (var socketItem in roomSockets)
                            {
                                await SendMessageToSocketAsync(notifyMessage.ToString(), socketItem);
                            }
                            break;
                        case "SendToRoom":
                            room = groupManager.GetGroupByName(message.RoomName);
                            roomSockets = webSocketConnectionManager.GetSocketsByListConnId(room.Value).Select(u => u.Websocket);
                            messageSend.Event = "MessageToRoom";
                            messageSend.RoomName = message.RoomName;
                            foreach (var socketItem in roomSockets)
                            {
                                await SendMessageToSocketAsync(messageSend.ToString(), socketItem);
                            }
                            break;
                        case "SendToReceiver":
                            socket = webSocketConnectionManager.GetSocketByUsername(message.Receiver)?.Websocket;
                            messageSend.Event = "MessageToReceiver";
                            await SendMessageToSocketAsync(messageSend.ToString(), socket);
                            break;
                        case "SendToAll":
                            sockets = webSocketConnectionManager.GetAllSockets().Values.Select(u => u.Websocket).ToList();
                            messageSend.Event = "MessageToAll";
                            foreach (var socketItem in sockets)
                            {
                                await SendMessageToSocketAsync(messageSend.ToString(), socketItem);
                            }
                            break;
                        case "NotifyToRoom":
                            room = groupManager.GetGroupByName(message.RoomName);
                            roomSockets = webSocketConnectionManager.GetSocketsByListConnId(room.Value).Select(u => u.Websocket);
                            messageSend.Event = "NotifyToRoom";
                            foreach(var socketItem in roomSockets)
                            {
                                await SendMessageToSocketAsync(messageSend.ToString(), socketItem);
                            }
                            break;
                        case "NotifyToReceiver":
                            socket = webSocketConnectionManager.GetSocketById(message.Receiver)?.Websocket;
                            messageSend.Event = "NotifyToReceiver";
                            await SendMessageToSocketAsync(messageSend.ToString(), socket);
                            break;
                        case "NotifyToAll":
                            sockets = webSocketConnectionManager.GetAllSockets().Values.Select(u => u.Websocket).ToList();
                            messageSend.Event = "NotifyToAll";
                            foreach (var socketItem in sockets)
                            {
                                await SendMessageToSocketAsync(messageSend.ToString(), socketItem);
                            }
                            break;
                        default:
                            break;
                    }
                }
            }

            receiveResult = await webSocket.ReceiveAsync(
                new ArraySegment<byte>(buffer), CancellationToken.None);
        }

        await webSocket.CloseAsync(
            receiveResult.CloseStatus.Value,
            receiveResult.CloseStatusDescription,
            CancellationToken.None);
    }

    private static async Task SendMessageToSocketAsync(string? message, WebSocket? socket)
    {
        if(socket is null || message is null)
        {
            return;
        }
        var buffer = Encoding.UTF8.GetBytes(message);
        await socket.SendAsync(
            new ArraySegment<byte>(buffer, 0, message.Length),
            WebSocketMessageType.Text,
            true,
            CancellationToken.None
        );
    }
}

public class MessageType{
    public string Event { get; set; } = string.Empty;
    public string RoomName { get; set; } = string.Empty;
    public string Receiver { get; set; } = string.Empty;
    public string Data { get; set; } = string.Empty;
}

public class MessageSend {
    public string Event { get; set; } = string.Empty;
    public string Data { get; set; } = string.Empty;
    public string From { get; set; } = string.Empty;
    public string RoomName { get; set; } = string.Empty;
    public override string? ToString()
    {
        if(this is not null)
            return JsonSerializer.Serialize(this);

        return null;
    }
}