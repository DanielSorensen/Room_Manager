using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

[ExecuteInEditMode]
[RequireComponent(typeof(LayoutGroup))]
public class AutoLayoutGroup : MonoBehaviour {

    public LayoutGroup layoutGroup { get; private set; }
    public new RectTransform transform { get; private set; }

    public float width;
    public float height;

    [ContextMenu("Re-Calculate")]
    public void ReCalculate() {
        if (!gameObject.activeInHierarchy) return;

        if (!layoutGroup) layoutGroup = GetComponent<LayoutGroup>();

        if (!transform) transform = gameObject.transform as RectTransform;
        int children = transform.childCount;

        for (int i = 0; i < transform.childCount; i++) {
            if (!transform.GetChild(i).gameObject.activeSelf) children -= 1;
        }

        if (layoutGroup is HorizontalLayoutGroup) {
            float w = (width * children) + (((HorizontalLayoutGroup)layoutGroup).spacing * (children - 1)) + layoutGroup.padding.left + layoutGroup.padding.right;
            float h = height + layoutGroup.padding.top + layoutGroup.padding.bottom;

            if (height == 0) h = transform.sizeDelta.y;

            transform.sizeDelta = new Vector2(w, h);
        }
        else {
            float w = width + layoutGroup.padding.left + layoutGroup.padding.right;
            float h = (height * children) + (((VerticalLayoutGroup)layoutGroup).spacing * (children - 1)) + layoutGroup.padding.top + layoutGroup.padding.bottom;

            if (width == 0) w = transform.sizeDelta.x;

            transform.sizeDelta = new Vector2(w, h);
        }

        LayoutRebuilder.MarkLayoutForRebuild(transform);

    }

    [ContextMenu("Clear Children")]
    public void ClearChildren(bool mustBeActive=false) {
        if (!gameObject.activeInHierarchy) return;
        if (!transform) transform = gameObject.transform as RectTransform;

        List<Transform> children = new List<Transform>();
        foreach (Transform child in transform) {
            if(!mustBeActive || child.gameObject.activeSelf) children.Add(child);
        }

        foreach(Transform child in children) {
            if (Application.isPlaying) Destroy(child.gameObject);
            else DestroyImmediate(child.gameObject);
        }
    }

    public int GetActiveChildCount() {
        int count = 0;

        for (int i = 0; i < transform.childCount; i++) {
            if (transform.GetChild(i).gameObject.activeSelf) count++;
        }

        return count;
    }

    void Update() {
        if(transform.hasChanged) {
            ReCalculate();
        }
    }

    void OnValidate() {
        ReCalculate();
    }

    void Start() {
        ReCalculate();
    }
}
