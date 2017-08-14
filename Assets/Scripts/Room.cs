using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Room {

    public string name;
    public float width, height;

    public List<Furniture.Data> furnitureData;

    public Room(string name, Vector2 size, List<Furniture.Data> furnitureData) {
        this.name = name;
        width = size.x;
        height = size.y;

        this.furnitureData = furnitureData;
    }
    public Room(string name, Vector2 size) {
        this.name = name;
        width = size.x;
        height = size.y;

        furnitureData = new List<Furniture.Data>();
    }

    public void SpawnFurniture(int index, Transform parent) {
        Furniture.Data data = furnitureData[index];
        if (data.hasTransform) {
            Furniture f = Furniture.Spawn(data);
            f.index = index;
            if (parent != null) {
                f.transform.SetParent(parent);
            }
        }
        else {
            Debug.LogError("Cannot spawn furniture: " + data.name + " because it does not have a position");
        }
    }
    public void SpawnAllFurniture(Transform parent) {
        for (int i = 0; i < furnitureData.Count; i++) {
            SpawnFurniture(i, parent);
        }
    }
}
