using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectSpawner : MonoBehaviour {
    private TerrainData terrainData;
    private int alphamapWidth, alphamapHeight;
    private float[,,] splatMapdata;
    private int numTextures;
    [SerializeField] private BiomeSO biomeSO;
    private Terrain terrain;
    private List<Vector3> positions = new();
    private int totalObject = 5;
    private List<GameObject> spawningGameObjects = new();
    private float step = 1f;

    private void Start() {
    }

    public void GetTerrainProperties() {
        terrain = GetComponent<Terrain>();
        terrainData = terrain.terrainData;
        alphamapWidth = terrainData.alphamapWidth;
        alphamapHeight = terrainData.alphamapHeight;
        splatMapdata = terrainData.GetAlphamaps(0, 0, alphamapWidth, alphamapHeight);
        numTextures = splatMapdata.Length / (alphamapWidth * alphamapHeight);
    }

    public void InitializePositions() {
        for (float x = 0f; x < terrainData.size.x; x += step) {
            for (float z = 0f; z < terrainData.size.z; z += step) {
                Vector3 position = new(x, 0f, z);
                int idx = GetTextureIdxAtPosition(position);
                TerrainLayer terrainLayer = terrainData.terrainLayers[idx];
                if (biomeSO.terrainLayers.Contains(terrainLayer)) {
                    positions.Add(position);
                }
            }
        }
    }

    private Vector3 ConvertPositionToSplatMapCoordinate(Vector3 position) {
        Vector3 vector3 = Vector3.zero;
        Vector3 terrainPosition = transform.position;
        vector3.x = position.x / terrainData.size.x * alphamapWidth;
        vector3.z = position.z / terrainData.size.z * alphamapHeight;
        return vector3;
    }


    private int GetTextureIdxAtPosition(Vector3 position) {
        Vector3 vector3 = ConvertPositionToSplatMapCoordinate(position);
        int activeTerrainTextureIdx = 0;
        float largestOpacity = 0f;
        for (int i = 0; i < numTextures; i++) {
            if (largestOpacity < splatMapdata[(int)vector3.z, (int)vector3.x, i]) {
                activeTerrainTextureIdx = i;
                largestOpacity = splatMapdata[(int)vector3.z, (int)vector3.x, i];
            }
        }

        return activeTerrainTextureIdx;
    }

    

    public bool SpawnRandomObject() {
        if (totalObject <= spawningGameObjects.Count) return false;
        Vector3 position = positions[Random.Range(0, positions.Count)];
        if (biomeSO.objects.Count == 0) return false;
        GameObject gameObject_ = Instantiate(biomeSO.objects[Random.Range(0, biomeSO.objects.Count)].GetRandomGameObject(), position, Quaternion.identity);
        gameObject_.transform.SetParent(transform, false);
        gameObject_.transform.localPosition = position;
        gameObject_.transform.localScale = Vector3.one * Random.Range(0.75f, 1.25f);
        gameObject_.transform.localRotation = Quaternion.Euler(0f, Random.Range(0f, 360f), 0f);
        spawningGameObjects.Add(gameObject_);
        return true;
    }
}
