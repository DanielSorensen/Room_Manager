using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public static class RoomManager {
    
    public static List<Furniture.Data> furniture;
    public static List<Room> rooms;

    public static Room currentRoom = new Room("Default Room", new Vector2(400, 300));

    public static Room CreateRoom(string name, Vector2 size) {
        Room room = new Room(name, size);
        SaveObject("Rooms/" + name, room);
        rooms.Add(room);

        Debug.Log("Created room: " + name);

        return room;
    }

    public static void DeleteRoom(Room room) {
        if(rooms.Contains(room)) {
            rooms.Remove(room);
        }
        if(File.Exists(Application.persistentDataPath + "/Rooms/" + room.name + ".dat")) {
            File.Delete(Application.persistentDataPath + "/Rooms/" + room.name + ".dat");
        }
        else {
            Debug.LogError("Cannot delete the room: " + room.name + "'s data becuase it does not exist");
        }
    }

    public static void SaveRoom(Room room) {
        SaveObject("Rooms/" + room.name, room);
        Debug.Log("Saved room: " + room.name);
    }

    #region Serialization
    public static void SaveObject(string name, object o) {
        BinaryFormatter bf = new BinaryFormatter();
        name = Application.persistentDataPath + "/" + name.Trim(Path.GetInvalidFileNameChars()) + ".dat";
        FileStream stream = File.Open(name, FileMode.OpenOrCreate);

        bf.Serialize(stream, o);
        stream.Close();
    }
    public static object LoadObject(string name) {
        name = name.Trim(Path.GetInvalidFileNameChars());
        if(File.Exists(Application.persistentDataPath + "/" + name + ".dat")) {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream stream = File.Open(Application.persistentDataPath + "/" + name + ".dat", FileMode.Open);

            object o = bf.Deserialize(stream);
            stream.Close();

            return o;
        }
        else {
            Debug.LogError("The object: " + name + " cannot be loaded because it's data file does not exist");
            return null;
        }
    }
    #endregion

    public static List<Room> LoadRooms() {
        rooms = new List<Room>();
        if(!Directory.Exists(Application.persistentDataPath + "/Rooms")) {
            Directory.CreateDirectory(Application.persistentDataPath + "/Rooms");
            Debug.Log("Creating rooms folder");
        }
        foreach(FileInfo file in new DirectoryInfo(Application.persistentDataPath + "/Rooms").GetFiles()) {
            if(file.Extension == ".dat") {
                Room room = (Room)LoadObject("Rooms/" + Path.GetFileNameWithoutExtension(file.Name));
                rooms.Add(room);
                Debug.Log("Found room: " + room.name);
            }
        }

        Debug.Log("Loaded " + rooms.Count + " rooms");

        return rooms;
    }

    public static List<Furniture.Data> LoadFurniture() {
        if(!File.Exists(Application.persistentDataPath + "/Furniture.dat")) {
            SaveFurniture();
            Debug.Log("Creating furniture data file");
        }
        furniture = (List<Furniture.Data>)LoadObject("Furniture");
        Debug.Log("Loaded " + furniture.Count + " furniture");
        return furniture;
    }
    public static void SaveFurniture() {
        if(furniture == null) {
            furniture = new List<Furniture.Data>();
        }
        SaveObject("Furniture", furniture);
        Debug.Log("Saved " + furniture.Count + " furniture");
    }

    public static void CreateFurniture(string name, Vector2 size) {
        furniture.Add(new Furniture.Data(name, size));
        SaveFurniture();
    }
    public static void DeleteFurniture(Furniture.Data furniture) {
        RoomManager.furniture.Remove(furniture);
        SaveFurniture();
    }
}
