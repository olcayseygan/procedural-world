using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/SimpleGrass")]
public class SimpleGrassSO : BiomeGrassSO {
    [SerializeField] private GameObject prefab;
    [SerializeField] private List<GameObject> prefabs = new();
}
