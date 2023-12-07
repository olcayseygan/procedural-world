using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Rendering;

public class MapGenerator : MonoBehaviour {
    [SerializeField] private GridSO gridSO;
    private Grid grid;
    [SerializeField, Range(1, 10)] private int generationSpeed = 1;
    [SerializeField] private float animationSpeed = 0.5f;

    private void Start() {
        transform.position = new Vector3(-gridSO.size / 2f, 0f, -gridSO.size / 2f);
        grid = new(gridSO);
        StartCoroutine(GenerateWorld());
    }

    private IEnumerator GenerateWorld() {
        PlaceFirstTile(gridSO.beginningTileSO);
        yield return new WaitForSeconds(animationSpeed);
        bool isDone = false;
        while (!isDone) {
            for (int i = 0; i < generationSpeed && !isDone; i++, isDone = !PlaceNextStepByMinimum()) ;
            yield return new WaitForSeconds(animationSpeed);
        }


    }

    public void PlaceFirstTile(TileSO tileSO) {
        grid.TryPlaceTile(grid.tiles[grid.tiles.Length / 2], tileSO, transform);
    }

    public bool PlaceNextStepByMinimum() {
        var gridOrdered = grid.tiles.Where(tile => tile.script == null && tile.validScripts.Count != 0).OrderBy(tile => tile.validScripts.Count).ToList();
        if (gridOrdered.Count == 0) return false;
        int stopIndex = gridOrdered.FindIndex(tile => gridOrdered[0].validScripts.Count < tile.validScripts.Count);
        if (stopIndex == -1) stopIndex = 0;
        gridOrdered = gridOrdered.Take(stopIndex + 1).ToList();
        if (gridOrdered.Count == 0) return false;
        var randomTile = gridOrdered[Random.Range(0, gridOrdered.Count)];
        var randomValidScript = randomTile.GetRandomValidScript();
        if (!grid.TryPlaceTile(randomTile, randomValidScript, transform)) return false;
        TileInitializer objectSpawner = randomValidScript.gameObject.GetComponent<TileInitializer>();
        if (objectSpawner != null) {
            objectSpawner.GetTerrainProperties();
            var positions = objectSpawner.InitializePositions();
            objectSpawner.SpawnRandomObjects(positions, out positions);
            objectSpawner.SpawnRandomGrass(positions, out positions);
        }

        return true;
    }
}
