using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Linq;

using TMPro;

public class Menu : MonoBehaviour {

    [Header("Main Menu")]
    public TMP_InputField nameField;
    public TMP_InputField sizeXField;
    public TMP_InputField sizeYField;
    [Space]
    public Transform rooms;
    public Button roomTemplate;
    [Header("Room")]
    public Vector2 extraCameraFactor;
    public Image room;
    public TextMeshProUGUI roomName;
    [Header("Furniture Window")]
    public RectTransform furnitureWindow;
    public RectTransform furnitureWindowContent;
    public Button furnitureItemTemplate;
    public Button addFurnitureButton;
    [Header("Add Furniture Window")]
    public TMP_InputField furnitureNameField;
    public TMP_InputField furnitureSizeXField;
    public TMP_InputField furnitureSizeYField;
    [Header("Edit Furniture Window")]
    public GameObject editFurnitureWindow;
    public TMP_InputField editFurnitureNameField;
    public TMP_InputField editFurnitureSizeXField;
    public TMP_InputField editFurnitureSizeYField;

    void Start() {
        if (SceneManager.GetActiveScene().name == "Main Menu") {
            LoadRooms();
        }
        else if (SceneManager.GetActiveScene().name == "Room") {
            RoomManager.LoadRooms();
            room.rectTransform.sizeDelta = new Vector2(RoomManager.currentRoom.width, RoomManager.currentRoom.length);
            RoomManager.currentRoom.SpawnAllFurniture(room.transform);
            roomName.text = RoomManager.currentRoom.name;
            LoadFurniture();
            if (RoomManager.ScanForNewFurniture(RoomManager.currentRoom)) {
                LoadFurniture();
            }
            ResetCamera();
        }
    }

    void Update() {
        if(SceneManager.GetActiveScene().name == "Room") {

        }
    }

    public void ResetCamera() {
        if (RoomManager.currentRoom.width / RoomManager.currentRoom.length > Camera.main.aspect) {
            Camera.main.orthographicSize = RoomManager.currentRoom.width / 2 * extraCameraFactor.x;
        }
        else {
            Camera.main.orthographicSize = RoomManager.currentRoom.length / 2 * extraCameraFactor.y;
        }
    }

    public void CreateRoom() {
        float width = float.Parse(sizeXField.text);
        float height = float.Parse(sizeYField.text);
        if (!string.IsNullOrEmpty(nameField.text) && width > 0 && height > 0) {
            RoomManager.currentRoom = RoomManager.CreateRoom(nameField.text, new Vector2(width, height));
            SceneManager.LoadScene("Room");
        }
    }

    public void LoadRooms() {
        rooms.ClearChildren(true);
        foreach (Room room in RoomManager.LoadRooms()) {
            Button roomItem = Instantiate(roomTemplate, rooms);
            roomItem.gameObject.SetActive(true);
            roomItem.GetComponentInChildren<TextMeshProUGUI>().text = room.name;
        }
    }

    public void LoadRoom(TextMeshProUGUI name) {
        List<string> names = RoomManager.rooms.Select((Room room) => room.name).ToList();
        if(names.Contains(name.text)) {
            RoomManager.currentRoom = RoomManager.rooms[names.IndexOf(name.text)];
            SceneManager.LoadScene("Room");
        }
        else {
            Debug.LogError("The room: " + name.text + " does not exist");
        }
    }
    public void DeleteRoom(TextMeshProUGUI name) {
        List<string> names = RoomManager.rooms.Select((Room room) => room.name).ToList();
        if (names.Contains(name.text)) {
            RoomManager.DeleteRoom(RoomManager.rooms[names.IndexOf(name.text)]);
            LoadRooms();
        }
        else {
            Debug.LogError("The room: " + name.text + " does not exist");
        }
    }

    public void LoadFurniture() {
        if (addFurnitureButton != null) addFurnitureButton.gameObject.SetActive(false);
        furnitureWindowContent.ClearChildren(true);
        if (addFurnitureButton != null) addFurnitureButton.gameObject.SetActive(true);
        foreach(Furniture.Data furniture in RoomManager.LoadFurniture()) {
            Button item = Instantiate(furnitureItemTemplate, furnitureWindowContent);
            item.gameObject.SetActive(true);
            item.GetComponentInChildren<TextMeshProUGUI>().text = furniture.name;
        }
        if (addFurnitureButton != null) addFurnitureButton.transform.SetAsLastSibling();
        CloseFurnitureWindow();
    }

    public void SpawnFurniture(TextMeshProUGUI name) {
        List<string> names = RoomManager.furniture.Select((Furniture.Data f) => f.name).ToList();
        if(names.Contains(name.text)) {
            Furniture.Data data = new Furniture.Data(RoomManager.furniture[names.IndexOf(name.text)], Vector2.zero, false);
            RoomManager.currentRoom.furnitureData.Add(data);
            RoomManager.currentRoom.SpawnFurniture(RoomManager.currentRoom.furnitureData.Count -1, room.rectTransform);
            SaveRooom();
            CloseFurnitureWindow();
        }
        else {
            Debug.LogError("The furniture: " + name.text + " does not exists");
        }
    }

    public void OpenFurnitureWindow() {
        furnitureWindow.anchoredPosition = new Vector2(0, 0);
    }
    public void CloseFurnitureWindow() {
        furnitureWindow.anchoredPosition = new Vector2(450, 0);
    }
    public void ToggleFurnitureWindow() {
        if(furnitureWindow.anchoredPosition.x == 0) {
            CloseFurnitureWindow();
        }
        else {
            OpenFurnitureWindow();
        }
    }

    public void AddFurniture() {
        RoomManager.CreateFurniture(furnitureNameField.text, new Vector2(float.Parse(furnitureSizeXField.text), float.Parse(furnitureSizeYField.text)));
        LoadFurniture();
    }

    public void EditFurniture(TextMeshProUGUI name) {
        List<string> names = RoomManager.furniture.Select((Furniture.Data f) => f.name).ToList();
        if(names.Contains(name.text)) {
            Furniture.Data data = RoomManager.furniture[names.IndexOf(name.text)];
            editFurnitureNameField.text = data.name;
            editFurnitureSizeXField.text = data.width.ToString();
            editFurnitureSizeYField.text = data.length.ToString();
            OpenEditFurnitureWindow();
        }
        else {
            Debug.LogError("The furniture: " + name.text + " does not exist");
        }
    }
    public void FinishEditFurniture() {
        List<string> names = RoomManager.furniture.Select((Furniture.Data f) => f.name).ToList();
        if (names.Contains(editFurnitureNameField.text)) {
            Furniture.Data data = RoomManager.furniture[names.IndexOf(editFurnitureNameField.text)];
            data.width = float.Parse(editFurnitureSizeXField.text);
            data.length = float.Parse(editFurnitureSizeYField.text);
            if (!string.IsNullOrEmpty(data.name) && data.width > 0 && data.length > 0) {
                RoomManager.furniture[names.IndexOf(data.name)] = data;
                RoomManager.SyncAllFurniture();
                RoomManager.currentRoom.SpawnAllFurniture(room.rectTransform);
                CloseEditFurnitureWindow();
                OpenFurnitureWindow();
            }
        }
        else {
            Debug.LogError("The furniture: " + editFurnitureNameField.text + " does not exist");
        }
    }

    public void OpenEditFurnitureWindow() {
        editFurnitureWindow.SetActive(true);
    }
    public void CloseEditFurnitureWindow() {
        editFurnitureWindow.SetActive(false);
    }

    public void SaveRooom() {
        RoomManager.SaveRoom(RoomManager.currentRoom);
    }
    public void ExitRoom() {
        SceneManager.LoadScene("Main Menu");
    }

    public void DeleteFurniture() {
        List<string> names = RoomManager.furniture.Select((Furniture.Data f) => f.name).ToList();
        if (names.Contains(editFurnitureNameField.text)) {
            Furniture.Data data = RoomManager.furniture[names.IndexOf(editFurnitureNameField.text)];
            RoomManager.DeleteFurniture(data);
            LoadFurniture();
            CloseEditFurnitureWindow();
        }
        else {
            Debug.LogError("The furniture: " + editFurnitureNameField.text + " does not exist");
        }
    }
}
