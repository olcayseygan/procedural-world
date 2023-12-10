using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BiomeTreeSO : ScriptableObject {
    public virtual List<GameObject> GetTreePrefabs() { return null; }
    public virtual GameObject GetRandomTreePrefab() { return null; }
}
