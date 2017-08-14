using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ClickHandler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {

    [System.Serializable] public class Event : UnityEngine.Events.UnityEvent { }
    public Event OnRightClick = new Event();

    public bool mouseOver { get; private set; }

    void Update() {
        if(Input.GetMouseButtonUp(1) && mouseOver) {
            OnRightClick.Invoke();
        }
    }

    public void OnPointerEnter(PointerEventData eventData) {
        mouseOver = true;
    }

    public void OnPointerExit(PointerEventData eventData) {
        mouseOver = false;
    }
}
