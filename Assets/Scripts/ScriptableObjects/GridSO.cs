using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Grid")]
public class GridSO : ScriptableObject {
    public int size = 1;
    public int gap = 1;
    public string title = "";
    public TileSO beginningTileSO;

    public int GetXByIndex(int index) {
        return index % size;
    }

    public int GetZByIndex(int index) {
        return index / size;
    }

    public int GetIndexByXZ(int x, int z) {
        return z * size + x;
    }

    private void OnValidate() {
        if (size < 1) size = 1;
    }
}
