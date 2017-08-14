using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DragHandler : MonoBehaviour, IDragHandler {

    [System.Serializable]
    public class Event : UnityEngine.Events.UnityEvent { }
    public Event OnDragEvent = new Event();

    public void OnDrag(PointerEventData eventData) {
        if (OnDragEvent != null) {
            OnDragEvent.Invoke();
        }
    }
}
