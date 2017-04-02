using System;
using System.Collections;
using System.Collections.Generic;

public class Rooms
{
    public class Room
    {
        public string playerUniqueID1 { get; set; }
        public string playerUniqueID2 { get; set; }

        public string GetSecondPlayer(string playerUniqueID)
        {
            if (playerUniqueID == playerUniqueID1)
                return playerUniqueID2;

            return playerUniqueID1;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as string);
        }

        public bool Equals(string playerID)
        {
            return (playerUniqueID1 == playerID) || (playerUniqueID2 == playerID);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

    public static List<Room> rooms = new List<Room>();

    public static void CreateRoom(string playerUniqueID1, string playerUniqueID2)
    {
        Clients.ChangeClientStatus(playerUniqueID1, Master.ClientStatus.Busy);
        Clients.ChangeClientStatus(playerUniqueID2, Master.ClientStatus.Busy);

        Room room = new Room { playerUniqueID1 = playerUniqueID1, playerUniqueID2 = playerUniqueID2 };
        rooms.Add(room);
    }

    public static void DeleteRoom(string playerUniqueID)
    {
        for (int i = 0; i < rooms.Count; ++i)
        {
            if (rooms[i].Equals(playerUniqueID))
            {
                Clients.ChangeClientStatus(rooms[i].playerUniqueID1, Master.ClientStatus.Free);
                Clients.ChangeClientStatus(rooms[i].playerUniqueID2, Master.ClientStatus.Free);
                rooms.RemoveAt(i);
                break;
            }
        }
    }

    public static Master.ClientInstance FindSecondRoomPlayer(string playerUniqueID)
    {
        for (int i = 0; i < rooms.Count; ++i)
        {
            if (rooms[i].Equals(playerUniqueID))
            {
                string secondPlayer = rooms[i].GetSecondPlayer(playerUniqueID);
                return Clients.GetClient(secondPlayer);
            }
        }

        return null;
    }
}