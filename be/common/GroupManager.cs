

using System.Collections.Concurrent;

namespace WebSocketsSample.common;

public class GroupManager
{
    public ConcurrentDictionary<string, HashSet<string>> _groups { get; set; } = new ConcurrentDictionary<string, HashSet<string>>();

    public ConcurrentDictionary<string, HashSet<string>> GetAllGroup()
    {
        return _groups;
    }

    public KeyValuePair<string, HashSet<string>> GetGroupByName(string groupName)
    {
        return _groups.FirstOrDefault(g => g.Key.Equals(groupName));
    }

    public void AddConnectionToAGroup(string groupName, string connId)
    {
        var checkExistedGroup = _groups.Any(g => g.Key.Equals(groupName));
        if(checkExistedGroup)
        {
            var connections = _groups.FirstOrDefault(g => g.Key.Equals(groupName)).Value;
            connections.Add(connId); 
            _groups[groupName] = connections;
            return;
        }

        _groups.TryAdd(groupName, new HashSet<string>(){connId});
    }

    public void RemoveConnectionToAGroup(string groupName, string connId)
    {
        var checkExistedGroup = _groups.Any(g => g.Key.Equals(groupName));
        if(checkExistedGroup)
        {
            var connections = _groups.FirstOrDefault(g => g.Key.Equals(groupName)).Value;
            connections.Remove(connId); 
            _groups[groupName] = connections;
        }
    }

    public void RemoveGroup(string groupName)
    {
        var checkExistedGroup = _groups.Any(g => g.Key.Equals(groupName));
        if(checkExistedGroup)
        {
            _groups.TryRemove(_groups.FirstOrDefault(g => g.Key.Equals(groupName)));
        }
    }
}