using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Tree")]
public class TreeSO : BiomeTreeSO {
    [SerializeField] private GameObject prefab;
    [SerializeField] private List<GameObject> prefabs = new();
    public override List<GameObject> GetTreePrefabs() {
        return prefabs;
    }

    public override GameObject GetRandomTreePrefab() {
        return prefabs[Random.Range(0, prefabs.Count)];
    }
}
