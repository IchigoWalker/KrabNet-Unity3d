using UnityEngine;
using UnityEngine.Networking;

public class NetworkMasterClient : MonoBehaviour
{
	public bool dedicatedServer;
	public string MasterServerIpAddress;
	public int MasterServerPort;
	public int updateRate;
	public string gameTypeName;
	public string gameName;
	public int gamePort;

    Master.RegisterSyncDataMessage syncMessage = new Master.RegisterSyncDataMessage { playerID = "", x = 0, y = 0 };

	[SerializeField]
	public int yoffset = 0;

	string HostGameType = "";
	string HostGameName = "";

	public string uniqueId;

	public NetworkClient client = null;

	static NetworkMasterClient singleton;

	void Awake()
	{
		if (singleton == null)
		{
			singleton = this;
			DontDestroyOnLoad(gameObject);
		}
		else
		{
			Destroy(gameObject);
		}
	}

	public void InitializeClient()
	{
		if (client != null)
		{
			Debug.LogError("Already connected");
			return;
		}

		client = new NetworkClient();
		client.Connect(MasterServerIpAddress, MasterServerPort);

		// system msgs
		client.RegisterHandler(MsgType.Connect, OnClientConnect);
		client.RegisterHandler(MsgType.Disconnect, OnClientDisconnect);
		client.RegisterHandler(MsgType.Error, OnClientError);

		// application msgs
		client.RegisterHandler(Master.RegisteredHostId, OnRegisteredHost);
		client.RegisterHandler(Master.UnregisteredHostId, OnUnregisteredHost);
        client.RegisterHandler(Master.RegisteredUniqueId, OnRegisteredUniqueId);
		client.RegisterHandler(Master.RegisteredMultiplayer, OnRegisterMultiplater);
        client.RegisterHandler(Master.RegisteredSyncData, OnRegisterSyncData);

        DontDestroyOnLoad(gameObject);
	}

    public void StartMultiplayer()
    {
        Master.RegisterSyncDataMessage syncData = new Master.RegisterSyncDataMessage
        {
            playerID = uniqueId,
            x = Random.Range(1, 1920),
            y = Random.Range(1, 1080)
        };
        syncMessage = syncData;
        client.Send(Master.RegisterSyncData, syncData);
    }

	public void ResetClient()
	{
		if (client == null)
			return;

		client.Disconnect();
		client = null;
	}

	public bool isConnected
	{
		get
		{
			if (client == null) 
				return false;
			else 
				return client.isConnected;
		}
	}

	// --------------- System Handlers -----------------

	void OnClientConnect(NetworkMessage netMsg)
	{
		Debug.Log("Client Connected to Master");

		if (!isConnected)
		{
			Debug.LogError("RequestHostList not connected");
			return;
		}

		var msg = new Master.RegisterUniqueIdMessage();
		msg.uniqueId = uniqueId;
		client.Send(Master.RegisterUniqueId, msg);
	}

	void OnClientDisconnect(NetworkMessage netMsg)
	{
		Debug.Log("Client Disconnected from Master");
		ResetClient();
		OnFailedToConnectToMasterServer();
	}

	void OnClientError(NetworkMessage netMsg)
	{
		Debug.Log("ClientError from Master");
		OnFailedToConnectToMasterServer();
	}

	// --------------- Application Handlers -----------------

	void OnRegisteredHost(NetworkMessage netMsg)
	{
		var msg = netMsg.ReadMessage<Master.RegisteredHostMessage>();
		OnServerEvent((Master.NetworkMasterServerEvent)msg.resultCode);
	}

	void OnUnregisteredHost(NetworkMessage netMsg)
	{
		var msg = netMsg.ReadMessage<Master.RegisteredHostMessage>();
		OnServerEvent((Master.NetworkMasterServerEvent)msg.resultCode);
	}

    void OnRegisteredUniqueId(NetworkMessage netMsg)
    {
        var msg = netMsg.ReadMessage<Master.RegisteredUniqueIdMessage>();
        OnServerEvent(Master.NetworkMasterServerEvent.UniqueIdReceived);
    }

    void OnRegisterMultiplater(NetworkMessage netMsg)
    {
        Debug.Log("Wait for player");
        // wait scene
        var msg = netMsg.ReadMessage<Master.RegisterStartMultiplayerMessage>();
        OnServerEvent(Master.NetworkMasterServerEvent.RegisterMultiplayerReceived);
        // start multiplayer scene
        Debug.Log("Start Multiplayer");
        StartMultiplayer();
    }

    void OnRegisterSyncData(NetworkMessage netMsg)
    {
        var msg = netMsg.ReadMessage<Master.RegisteredSyncDataMessage>();
        Debug.Log(msg.playerID + " " + msg.x + " " + msg.y);
        Master.RegisterSyncDataMessage syncData = new Master.RegisterSyncDataMessage
        {
            playerID = uniqueId,
            x = Random.Range(1, 1920),
            y = Random.Range(1, 1080)
        };
        syncMessage = syncData;
        netMsg.conn.Send(Master.RegisterSyncData, syncData);
    }

	public virtual void OnFailedToConnectToMasterServer()
	{
		Debug.Log("OnFailedToConnectToMasterServer");
	}

	public virtual void OnServerEvent(Master.NetworkMasterServerEvent evt)
	{
        switch (evt)
        {
            case Master.NetworkMasterServerEvent.UniqueIdReceived:
                Debug.Log("UniqueId answer: " + evt);
                break;
            case Master.NetworkMasterServerEvent.RegisterMultiplayerReceived:
                Debug.Log("RegisterMultiplayer answer: " + evt);
                break;
            case Master.NetworkMasterServerEvent.RegistrationSucceeded:
                if (NetworkManager.singleton != null)
                    NetworkManager.singleton.StartHost();
                break;
            case Master.NetworkMasterServerEvent.UnregistrationSucceeded:
                if (NetworkManager.singleton != null)
                    NetworkManager.singleton.StopHost();
                break;
        }
	}

	void OnGUI()
	{
        GUI.Label(new Rect(100, yoffset, 300, 20), "Sync: " + syncMessage.playerID + " x: " + syncMessage.x + " y: " + syncMessage.y);

        if (client != null && client.isConnected)
		{
			if (GUI.Button(new Rect(100, 20+yoffset, 200, 20), "MasterClient Disconnect"))
			{
				ResetClient();
				if (NetworkManager.singleton != null)
				{
					NetworkManager.singleton.StopServer();
					NetworkManager.singleton.StopClient();
				}
				HostGameType = "";
				HostGameName = ""; 
			}

			if (GUI.Button(new Rect(100, 20 + yoffset + 100, 200, 20), "Start Multiplayer"))
			{
				var msg = new Master.RegisterMultiplayerMessage();
				msg.uniqueId = uniqueId;
				client.Send(Master.RegisterMultiplayer, msg);

				Debug.Log("Send MultiplayerRegister");
            }

		}
		else
		{
			if (GUI.Button(new Rect(100, 20+yoffset, 200, 20), "MasterClient Connect"))
			{
				InitializeClient();
			}

			GUI.Label(new Rect(100, 50 + yoffset + 200, 80, 20), "UniqueId:");
			uniqueId = GUI.TextField(new Rect(180, 50 + yoffset + 200, 200, 20), uniqueId);

			return;
		}
	}
}
