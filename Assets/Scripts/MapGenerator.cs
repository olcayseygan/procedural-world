using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;

public class MapGenerator : MonoBehaviour {
    [SerializeField] private GridSO gridSO;
    private Grid grid;

    private void Start() {
        grid = new(gridSO);
        PlaceFirstTile(gridSO.beginningTileSO);
    }

    private void Update() {
        PlaceNextStepByMinimum();
    }

    public void PlaceFirstTile(TileSO tileSO) {
        grid.TryPlaceTile(grid.tiles[grid.tiles.Length / 2], tileSO);
    }

    public void PlaceNextStepByMinimum() {
        var gridOrdered = grid.tiles.Where(tile => tile.script == null && tile.validScripts.Count != 0).OrderBy(tile => tile.validScripts.Count).ToList();
        if (gridOrdered.Count == 0) return;
        int stopIndex = gridOrdered.FindIndex(tile => gridOrdered[0].validScripts.Count < tile.validScripts.Count);
        if (stopIndex == -1) stopIndex = 0;
        gridOrdered = gridOrdered.Take(stopIndex + 1).ToList();
        if (gridOrdered.Count == 0) return;
        var randomTile = gridOrdered[Random.Range(0, gridOrdered.Count)];
        if (!grid.TryPlaceTile(randomTile, randomTile.GetRandomValidScript())) return;
    }
}
