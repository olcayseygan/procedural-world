using UnityEngine;

public class GrassPainter : MonoBehaviour {
    public Terrain terrain;
    public int grassTextureIndex = 0; // Index of the grass texture in the terrain's splatmap
    public int grassDensity = 5; // Density of the grass
    public Texture2D grassTexture;

    void Start() {
        if (terrain == null) {
            Debug.LogError("Terrain not assigned to GrassPainter script!");
            return;
        }

        // Get the terrain data
        TerrainData terrainData = terrain.terrainData;
        var a = terrainData.detailPrototypes;
        var b = terrainData.terrainLayers;
        var c = terrainData.GetDetailLayer(0, 0, terrainData.detailWidth, terrainData.detailHeight, 0);

        // Set the grass density map
        int[,] detailMap = new int[terrainData.detailWidth, terrainData.detailHeight];
        for (int i = 0; i < terrainData.detailWidth; i += grassDensity) {
            for (int j = 0; j < terrainData.detailHeight; j += grassDensity) {
                detailMap[i, j] = 204; // Set grass at this position
            }
        }

        // Apply the grass density map to the terrain
        terrainData.SetDetailLayer(0, 0, grassTextureIndex, detailMap);
    }
}
