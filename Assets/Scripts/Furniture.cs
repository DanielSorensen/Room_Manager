using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

using TMPro;

[RequireComponent(typeof(Image))]
public class Furniture : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {

    string _name {
        get {
            return _name;
        }
        set {
            _name = value;
            if(nameText != null) {
                nameText.text = _name;
            }
        }
    }
    public new string name;
    public int index;
    Vector2 _size;
    public Vector2 size {
        get {
            return _size;
        }
        set {
            _size = value;
            image.rectTransform.sizeDelta = (!rotated) ? _size : new Vector2(_size.y, _size.x);
        }
    }

    Vector2 _position;
    public Vector2 position {
        get {
            return _position;
        }
        set {
            _position = value;
            Data data = RoomManager.currentRoom.furnitureData[index];
            data.SetTransform(_position, rotated);
            RoomManager.currentRoom.furnitureData[index] = data;
            image.rectTransform.localPosition = _position;
        }
    }

    Image _image;
    public Image image {
        get {
            if(_image == null) {
                _image = GetComponent<Image>();
            }
            return _image;
        }
    }

    bool _rotated;
    public bool rotated {
        get {
            return _rotated;
        }
        set {
            _rotated = value;
            Data data = RoomManager.currentRoom.furnitureData[index];
            data.SetTransform(position, _rotated);
            RoomManager.currentRoom.furnitureData[index] = data;
            size = _size;
        }
    }

    public static Color highlightColour = Color.blue;
    public static Color pressColour = Color.red;
    [HideInInspector]
    public Color normalColour;

    public TextMeshProUGUI nameText;

    public bool pointerOver { get; private set; }
    public bool pointerDown { get; private set; }

    Vector2 lastMousePosition;
    bool moving;
    bool lastMoving;

    void Awake() {
        normalColour = image.color;
    }

    void Update() {
        if(pointerOver) pointerDown = Input.GetMouseButton(0);

        if (pointerOver && pointerDown) moving = true;

        if(moving) {
            transform.SetAsLastSibling();
            position += (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition) - lastMousePosition;
        }
        if(Input.GetMouseButtonDown(1) && pointerOver) {
            rotated = !rotated;
            RoomManager.SaveRoom(RoomManager.currentRoom);
        }
        if(Input.GetKeyDown(KeyCode.Delete) && pointerOver) {
            RoomManager.currentRoom.furnitureData.RemoveAt(index);
            RoomManager.SaveRoom(RoomManager.currentRoom);
            Destroy(gameObject);
        }

        if (!pointerDown) moving = false;

        if (moving == false && lastMoving == true) {
            RoomManager.SaveRoom(RoomManager.currentRoom);
        }

        image.color = normalColour;
        if (pointerOver) image.color = highlightColour;
        if (pointerDown) image.color = pressColour;

        lastMousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        lastMoving = moving;
    }

    public static Furniture Spawn(string name, Vector2 size, Vector2 position, bool rotated) {
        Furniture f = Instantiate(Resources.Load<Furniture>("Furniture"));
        f.name = name;
        f.size = size;
        f.position = position;
        f.rotated = rotated;
        
        return f;
    }
    public static Furniture Spawn(Data data) {
        if (data.hasTransform) {
            return Spawn(data.name, new Vector2(data.width, data.length), new Vector2(data.positionX, data.positionY), data.rotated);
        }
        else {
            Debug.LogError("To spawn furniture, 'data' must have a transform");
            return null;
        }
    }

    public void OnPointerEnter(PointerEventData eventData) {
        pointerOver = true;
    }

    public void OnPointerExit(PointerEventData eventData) {
        pointerOver = false;
    }

    [System.Serializable]
    public struct Data {
        public string name;
        public float width, length;

        public bool hasTransform { get; private set; }
        public float positionX { get; private set; }
        public float positionY { get; private set; }
        public bool rotated { get; private set; }

        public Data(string name, Vector2 size) {
            this.name = name;
            width = size.x;
            length = size.y;

            hasTransform = false;
            positionX = 0;
            positionY = 0;
            rotated = false;
        }
        public Data(string name, Vector2 size, Vector2 position, bool rotated) {
            this.name = name;
            width = size.x;
            length = size.y;

            hasTransform = true;
            positionX = position.x;
            positionY = position.y;
            this.rotated = rotated;
        }
        public Data(Data data, Vector2 position, bool rotated) {
            name = data.name;
            width = data.width;
            length = data.length;

            hasTransform = true;
            positionX = position.x;
            positionY = position.y;
            this.rotated = rotated;
        }

        public void SetTransform(Vector2 position, bool rotated) {
            positionX = position.x;
            positionY = position.y;
            this.rotated = rotated;
            hasTransform = true;
        }
        public void RemoveTransform() {
            positionX = 0;
            positionY = 0;
            rotated = false;
            hasTransform = false;
        }
    }
}
