using UnityEngine;
using UnityEngine.Networking;

public class NetworkMasterServer : MonoBehaviour
{
	public int MasterServerPort;

	public void InitializeServer()
	{
		if (NetworkServer.active)
		{
			Debug.LogError("Already Initialized");
			return;
		}

		NetworkServer.Listen(MasterServerPort);

		// system msgs
		NetworkServer.RegisterHandler(MsgType.Connect, ServerEvents.OnServerConnect);
		NetworkServer.RegisterHandler(MsgType.Disconnect, ServerEvents.OnServerDisconnect);
		NetworkServer.RegisterHandler(MsgType.Error, ServerEvents.OnServerError);

		// application msgs
		NetworkServer.RegisterHandler(Master.RegisterHostId, ServerEvents.OnServerRegisterHost);
		NetworkServer.RegisterHandler(Master.UnregisterHostId, ServerEvents.OnServerUnregisterHost);
        NetworkServer.RegisterHandler(Master.RegisterUniqueId, ServerEvents.OnServerUniqueId);
		NetworkServer.RegisterHandler(Master.RegisterMultiplayer, ServerEvents.onRegisterMultiplayer);
        NetworkServer.RegisterHandler(Master.RegisterSyncData, ServerEvents.OnServerRegisterSyncData);

        DontDestroyOnLoad(gameObject);
	}

	public void ResetServer()
	{
		NetworkServer.Shutdown();
	}

    void OnGUI()
    {
        if (NetworkServer.active)
        {
            GUI.Label(new Rect(400, 0, 200, 20), "Online port:" + MasterServerPort);
            if (GUI.Button(new Rect(400, 20, 200, 20), "Reset  Master Server"))
            {
                ResetServer();
            }
        }
        else
        {
            if (GUI.Button(new Rect(400, 20, 200, 20), "Init Master Server"))
            {
                InitializeServer();
            }
        }

        int y = 100;
        foreach (var room in Rooms.rooms)
        {
            GUI.Label(new Rect(400, y, 200, 20), "Room: ");
            y += 22;
            GUI.Label(new Rect(420, y, 200, 20), "Player 1: " + room.playerUniqueID1);
            y += 22;
            GUI.Label(new Rect(420, y, 200, 20), "Player 2: " + room.playerUniqueID2);
            y += 22;
        }
    }
}