using UnityEngine.Networking;

public class Master
{
    public enum ClientStatus
    {
        Free,
        Busy,
        Wait
    }

    public class ClientInstance
    {
        public ClientStatus status;
        public NetworkConnection connection;
    }

    public enum NetworkMasterServerEvent
    {
        RegistrationFailedGameName, // Registration failed because an empty game name was given.
        RegistrationFailedGameType, // Registration failed because an empty game type was given.
        RegistrationFailedNoServer, // Registration failed because no server is running.
        RegistrationSucceeded, // Registration to master server succeeded, received confirmation.
        UnregistrationSucceeded, // Unregistration to master server succeeded, received confirmation.
        UniqueIdReceived, // Received unique device ID
        RegisterMultiplayerReceived,
    }

    public const string WaitForPlayer = "wait";

    // -------------- client to masterserver Ids --------------

    public const short RegisterHostId = 100;
    public const short UnregisterHostId = 101;
    public const short RegisterUniqueId = 102;
    public const short RegisterMultiplayer = 103;
    public const short RegisterSyncData = 104;

    // -------------- masterserver to client Ids --------------

    public const short RegisteredHostId = 200;
    public const short UnregisteredHostId = 201;
    public const short RegisteredUniqueId = 202;
    public const short RegisteredMultiplayer = 203;
    public const short RegisteredSyncData = 204;

    // -------------- client to server messages --------------

    public class RegisterHostMessage : MessageBase
    {
        public string gameTypeName;
        public string gameName;
        public string comment;
        public bool passwordProtected;
        public int playerLimit;
        public int hostPort;
    }

    public class RegisterUniqueIdMessage : MessageBase
    {
        public string uniqueId;
    }

    public class RegisterMultiplayerMessage : MessageBase
    {
        public string uniqueId;
    }

    public class UnregisterHostMessage : MessageBase
    {
        public string gameTypeName;
        public string gameName;
    }

    public class RegisterSyncDataMessage : MessageBase
    {
        public string playerID;
        public int x; // x coord
        public int y; // y coord
    }

    // -------------- server to client messages --------------

    public class RegisteredUniqueIdMessage : MessageBase
    {
        public int resultCode;
    }

    public class RegisteredHostMessage : MessageBase
    {
        public int resultCode;
    }

    public class RegisteredMultiplayerMessage : MessageBase
    {
        public int resultCode;
    }

    public class RegisterStartMultiplayerMessage : MessageBase
    {
        public ClientStatus status;
    }

    public class RegisteredSyncDataMessage : MessageBase
    {
        public string playerID;
        public int x; // x coord
        public int y; // y coord
    }
}
