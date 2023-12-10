using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.Mathf;

public class WorldGenerator : MonoBehaviour {
    private Terrain terrain;

    [Header("Map Settings")]
    [SerializeField] private Vector3Int size = Vector3Int.zero;

    [Header("Noise Settings")]
    [SerializeField] private float seed = -1;
    [SerializeField] private Vector3 offset = Vector3Int.zero;
    [SerializeField, Range(1f, 20f)] private float scale = 20f;
    [SerializeField, Range(1f, 7f)] private float frequency = 1f;
    [SerializeField, Range(1, 8)] private int octaves = 3;
    [SerializeField, Range(.01f, 10f)] private float redistribution = 1f;
    [SerializeField, Range(1, 8)] private float lacunarity = 1;
    [SerializeField] private float fudgefactor = 1f;

    [Header("Spot Settings")]
    [SerializeField, Range(0f, 1f)] float heigthThreshold = 0.05f;
    [SerializeField, Range(0f, 1f)] float slopeThreshold = 0.005f;
    [SerializeField, Range(2, 5)] int square = 5;

    List<Vector3> vector3s;
    private void Awake() {
        terrain = GetComponent<Terrain>();
        terrain.terrainData = GenerateTerrainData(terrain.terrainData, out var heights);
        vector3s = FindBestSpotsToPlace(heights);

        foreach (var position in vector3s) {
            int halfSquare = square / 2 + 1;
            for (int nx = -halfSquare; nx < halfSquare; nx++) {
                for (int nz = -halfSquare; nz < halfSquare; nz++) {
                    float flatHeight = position.y / size.y;
                    heights[(int)position.z + nz, (int)position.x + nx] = flatHeight;
                }
            }
        }

        terrain.terrainData.SetHeights(0, 0, heights);
    }
    private List<Vector3> FindBestSpotsToPlace(float[,] heights) {
        List<Vector3> positions = ExtractPlaceableSpots(heights);
        positions = FindBestSpots(positions);
        return positions;
    }

    private List<Vector3> FindBestSpots(List<Vector3> positions) {
        for (int i = 0; i < positions.Count; i++) {
            Vector3 position = positions[Random.Range(0, positions.Count)];
            for (int j = positions.Count - 1; 0 <= j; j--) {
                if (position == positions[j]) continue;
                if (Vector3.Distance(position, positions[j]) < 100f) {
                    positions.RemoveAt(j);
                }
            }
        }

        return positions;
    }

    private List<Vector3> ExtractPlaceableSpots(float[,] heights) {
        List<Vector3> positions = new();
        int halfSquare = square / 2;
        for (int x = halfSquare; x < size.x - halfSquare; x++) {
            for (int z = halfSquare; z < size.z - halfSquare; z++) {
                if (heigthThreshold < heights[x, z]) continue;
                Vector3 position = new(z, heights[x, z] * size.y, x);
                bool isGonnaBreak = false;
                for (int nx = -halfSquare; nx < halfSquare; nx++) {
                    for (int nz = -halfSquare; nz < halfSquare; nz++) {
                        if (slopeThreshold < Abs(heights[x, z] - heights[x + nx, z + nx]) * size.y) {
                            isGonnaBreak = true;
                            break;
                        }
                    }

                    if (isGonnaBreak) break;
                }

                if (isGonnaBreak) continue;
                positions.Add(position);
            }
        }

        return positions;
    }

    private TerrainData GenerateTerrainData(TerrainData terrainData, out float[,] heights) {
        terrainData.heightmapResolution = size.x + 1;
        terrainData.size = size;
        heights = GenerateHeights();
        terrainData.SetHeights(0, 0, heights);
        return terrainData;
    }

    private float[,] GenerateHeights() {
        if (seed < 0)
            seed = Random.Range(0f, 100f);

        float[,] heights = new float[size.x, size.z];
        float maxValue = float.NegativeInfinity;
        float minValue = float.PositiveInfinity;
        for (int x = 0; x < size.x; x++) {
            for (int z = 0; z < size.z; z++) {
                heights[x, z] = GeneratePerlinNoise(x, z);
                maxValue = Max(maxValue, heights[x, z]);
                minValue = Min(minValue, heights[x, z]);
                // Create a flat 5x5 square in the center without breaking the surface
                //int centerX = size.x / 2;
                //int centerY = size.z / 2;
                //int halfSquareSize = 2;

                //if (x >= centerX - halfSquareSize && x <= centerX + halfSquareSize &&
                //    z >= centerY - halfSquareSize && z <= centerY + halfSquareSize) {
                //    float flatHeight = 00f; // Adjust this value to control the flatness
                //    heights[x, z] = Mathf.Lerp(heights[x, z], flatHeight, 0.7f); // Adjust the blend factor for a smooth transition
                //}
            }
        }

        for (int x = 0; x < size.x; x++) {
            for (int z = 0; z < size.z; z++) {
                float height = heights[x, z];
                height = InverseLerp(minValue, maxValue, height);
                heights[x, z] = height;
            }
        }

        return heights;
    }

    private float GeneratePerlinNoise(int x, int z) {
        float nx = ((float)(x - size.x / 2f) / size.x) / scale * frequency + offset.x * frequency;
        float nz = ((float)(z - size.z / 2f) / size.z) / scale * frequency - offset.z * frequency;

        float e = 0f;
        float n = 0f;
        for (float i = 0, d = 1f, m = 1f; i < octaves; i++, d /= 2f, m *= 2f) {
            e += d * PerlinNoise(lacunarity * m * nx + seed, lacunarity * m * nz + seed);
            n += d;
        }

        e /= n;
        e = Pow(e * fudgefactor, redistribution);
        float perlinValue = e;
        return perlinValue;
    }

    private void OnDrawGizmos() {
        if (!Application.isPlaying) return;

        Gizmos.color = Color.red;
        foreach (var vector3 in vector3s) {
            Gizmos.DrawWireSphere(vector3, 1f);
        }
    }
}
