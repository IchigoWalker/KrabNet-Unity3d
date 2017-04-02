using System.Collections.Generic;
using System.Linq;
using UnityEngine.Networking;

class Clients
{
    public static Dictionary<string, Master.ClientInstance> clients = new Dictionary<string, Master.ClientInstance>();

    public static void AddClient(Master.RegisterUniqueIdMessage msg, NetworkMessage netMsg)
    {
        clients.Add(msg.uniqueId, new Master.ClientInstance { status = Master.ClientStatus.Free, connection = netMsg.conn });
    }

    public static bool DeleteClient(NetworkMessage netMsg)
    {
        foreach (var client in clients.Values)
        {
            if (client.connection == netMsg.conn)
            {
                var key = clients.FirstOrDefault(x => x.Value == client).Key;

                if (client.status == Master.ClientStatus.Busy)
                {
                    Rooms.DeleteRoom(key);
                }
                
                clients.Remove(key);
                return true;
            }
        }

        return false;
    }

    public static Master.ClientInstance GetClient(string playerID)
    {
        return clients[playerID];
    }

    public static void ChangeClientStatus(string key, Master.ClientStatus status)
    {
        clients[key].status = status;
    }

    public static string GetWaitClient(string uniqueId)
    {
        foreach (var client in clients.Values)
        {
            if (client.status == Master.ClientStatus.Wait)
            {
                var key = clients.FirstOrDefault(x => x.Value == client).Key;
                if (key != uniqueId)
                {
                    return key;
                }
            }
        }

        return Master.WaitForPlayer;
    }
}