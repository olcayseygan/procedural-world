using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class TileInitializer : MonoBehaviour {
    private TerrainData terrainData;
    private TerrainCollider terrainCollider;
    private int alphamapWidth, alphamapHeight;
    private float[,,] splatMapdata;
    private int numTextures;
    [SerializeField] private BiomeSO biomeSO;
    private Terrain terrain;
    private int totalObject = 5;
    private float step = 1f;
    [SerializeField] private int grassDensity = 5;

    public void GetTerrainProperties() {
        terrain = GetComponent<Terrain>();
        terrainCollider = GetComponent<TerrainCollider>();
        terrainData = Instantiate(terrain.terrainData);
        terrainCollider.terrainData = terrainData;
        terrain.terrainData = terrainData;
        alphamapWidth = terrainData.alphamapWidth;
        alphamapHeight = terrainData.alphamapHeight;
        splatMapdata = terrainData.GetAlphamaps(0, 0, alphamapWidth, alphamapHeight);
        numTextures = splatMapdata.Length / (alphamapWidth * alphamapHeight);
    }

    public List<Vector3> InitializePositions() {
        List<Vector3> positions = new();
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
        return positions;
    }

    private Vector3 ConvertPositionToSplatMapCoordinate(Vector3 position) {
        Vector3 vector3 = Vector3.zero;
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

    public bool SpawnRandomObjects(List<Vector3> inPositions, out List<Vector3> outPositions) {
        outPositions = inPositions;
        if (biomeSO.objects.Count == 0) return false;
        for (int i = 0; i < totalObject; i++) {
            Vector3 position = outPositions[Random.Range(0, outPositions.Count)];
            GameObject gameObject_ = Instantiate(biomeSO.objects[Random.Range(0, biomeSO.objects.Count)].GetRandomGameObject(), position, Quaternion.identity);
            gameObject_.transform.SetParent(transform, false);
            gameObject_.transform.localPosition = position;
            gameObject_.transform.localScale = Vector3.one * Random.Range(0.75f, 1.25f);
            gameObject_.transform.localRotation = Quaternion.Euler(0f, Random.Range(0f, 360f), 0f);
            outPositions.Remove(position);
        }
        return true;
    }

    public bool SpawnRandomGrass(List<Vector3> inPositions, out List<Vector3> outPositions) {
        outPositions = inPositions;
        int[,] detailMap = new int[terrainData.detailWidth, terrainData.detailHeight];
        for (int i = 0; i < 5; i++) {
            Vector3 position = outPositions[Random.Range(0, outPositions.Count)];
            detailMap[(int)position.x, (int)position.z] = 204;
            outPositions.Remove(position);
        }

        terrainData.SetDetailLayer(0, 0, 0, detailMap);
        return true;
    }
}
