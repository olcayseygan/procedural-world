using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Biome")]
public class BiomeSO : ScriptableObject {
    public List<BiomeTreeSO> trees = new();
    public List<BiomeGrassSO> grasses = new();
    public List<TerrainLayer> terrainLayers = new();
}
