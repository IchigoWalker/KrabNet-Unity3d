using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

class ServerEvents
{
    public static Dictionary<string, Rooms> gameTypeRooms = new Dictionary<string, Rooms>();

    public static void OnServerConnect(NetworkMessage netMsg)
    {
        Debug.Log("Master received client");
    }

    public static void OnServerError(NetworkMessage netMsg)
    {
        Debug.Log("ServerError from Master");
    }

    public static void OnServerDisconnect(NetworkMessage netMsg)
    {
        Debug.Log("Master lost client");

        if (!Clients.DeleteClient(netMsg))
        {
            Debug.Log("Cant find client");
        }
    }

    public static void OnServerUniqueId(NetworkMessage netMsg)
    {
        Debug.Log("OnServerUniqueId");
        var msg = netMsg.ReadMessage<Master.RegisterUniqueIdMessage>();

        Clients.AddClient(msg, netMsg);

        var response = new Master.RegisteredUniqueIdMessage();
        response.resultCode = 1;
        netMsg.conn.Send(Master.RegisteredUniqueId, response);
    }

    public static void OnServerRegisterHost(NetworkMessage netMsg)
    {
        Debug.Log("OnServerRegisterHost");
        var msg = netMsg.ReadMessage<Master.RegisterHostMessage>();

        var result = (int)Master.NetworkMasterServerEvent.RegistrationSucceeded;

        var response = new Master.RegisteredHostMessage();
        response.resultCode = result;
        netMsg.conn.Send(Master.RegisteredHostId, response);
    }

    public static void onRegisterMultiplayer(NetworkMessage netMsg)
    {
        var msg = netMsg.ReadMessage<Master.RegisterMultiplayerMessage>();
        Debug.Log("UniqueId: " + msg.uniqueId);

        Clients.ChangeClientStatus(msg.uniqueId, Master.ClientStatus.Wait);

        var response = new Master.RegisterStartMultiplayerMessage();
        var secondUniqueID = Clients.GetWaitClient(msg.uniqueId);

        if (secondUniqueID != Master.WaitForPlayer)
        {
            Rooms.CreateRoom(msg.uniqueId, secondUniqueID);
            response.status = Master.ClientStatus.Busy;
            var secondClient = Clients.GetClient(secondUniqueID);
            netMsg.conn.Send(Master.RegisteredMultiplayer, response);
            secondClient.connection.Send(Master.RegisteredMultiplayer, response);
        }
    }

    public static void OnServerUnregisterHost(NetworkMessage netMsg)
    {
        Debug.Log("OnServerUnregisterHost");
        var msg = netMsg.ReadMessage<Master.UnregisterHostMessage>();

        var response = new Master.RegisteredHostMessage();
        response.resultCode = (int)Master.NetworkMasterServerEvent.UnregistrationSucceeded;
        netMsg.conn.Send(Master.UnregisteredHostId, response);
    }

    public static void OnServerRegisterSyncData(NetworkMessage netMsg)
    {
        Debug.Log("OnServerRegisterSyncData");
        var msg = netMsg.ReadMessage<Master.RegisterSyncDataMessage>();

        Master.ClientInstance secondClient = Rooms.FindSecondRoomPlayer(msg.playerID);

        secondClient.connection.Send(Master.RegisteredSyncData, msg);
    }
}