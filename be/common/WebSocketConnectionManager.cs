using System.Collections.Concurrent;
using System.Net.WebSockets;

namespace WebSocketsSample.common;

public class WebSocketConnectionManager
{
    private ConcurrentDictionary<string, User> _sockets = new ConcurrentDictionary<string, User>();

    public ConcurrentDictionary<string, User> GetAllSockets()
    {
        return _sockets;
    }

    public bool AddSocket(WebSocket socket, string connId, string username)
    {
        return _sockets.TryAdd(connId, new User(socket, username));
    }

    public User GetSocketById(string connId)
    {
        return _sockets.FirstOrDefault(s => s.Key == connId).Value;
    }

    public User GetSocketByUsername(string username)
    {
        return _sockets.FirstOrDefault(s => s.Value.Username.Equals(username)).Value;
    }

    public IEnumerable<User> GetSocketsByListConnId(HashSet<string> connIds)
    {
        return _sockets.Where(s => connIds.Contains(s.Key)).Select(kv => kv.Value);
    }
}

public class User
{
    public string Username { get; set; } = string.Empty;
    public WebSocket Websocket { get; set; }
    public User(WebSocket ws, string username)
    {
        Username = username;
        Websocket = ws;
    }
}