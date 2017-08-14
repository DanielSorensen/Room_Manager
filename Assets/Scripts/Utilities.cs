using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utilities {

	public static void ClearChildren(this Transform t, bool onlyActiveChilren) {
        for (int i = 0; i < t.childCount; i++) {
            GameObject child = t.GetChild(i).gameObject;

            if(child.activeSelf || !onlyActiveChilren) {
                Object.Destroy(child);
            }
        }
    }
}
