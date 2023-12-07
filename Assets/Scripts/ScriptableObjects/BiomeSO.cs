using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Biome")]
public class BiomeSO : ScriptableObject {
    public List<BiomeObjectSO> objects = new();
    public List<TerrainLayer> terrainLayers = new();
}
