using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class Grid {
    private readonly int size;
    private readonly GridSO gridSO;
    public Tile[] tiles;

    public Grid(GridSO gridSO) {
        size = gridSO.size % 2 == 0 ? gridSO.size + 1 : gridSO.size;
        this.gridSO = gridSO;
        CreateTiles();
        AddNeighbors();
        TileSO.LoadRepository(gridSO.title);
    }

    private void CreateTiles() {
        tiles = new Tile[size * size];
        for (int x = 0; x < size; x++) {
            for (int z = 0; z < size; z++) {
                tiles[gridSO.GetIndexByXZ(x, z)] = new Tile() { x = x, z = z };
            }
        }        
    }

    private void AddNeighbors() {
        for (int x = 0; x < size; x++) {
            for (int z = 0; z < size; z++) {
                Tile tile = tiles[gridSO.GetIndexByXZ(x, z)];
                if (z + 1 < size) { tile.neighbors[TileSO.Direction.Z_POSITIVE] = tiles[gridSO.GetIndexByXZ(x, z + 1)]; }
                if (x + 1 < size) { tile.neighbors[TileSO.Direction.X_POSITIVE] = tiles[gridSO.GetIndexByXZ(x + 1, z)]; }
                if (0 <= z - 1) { tile.neighbors[TileSO.Direction.Z_NEGATIVE] = tiles[gridSO.GetIndexByXZ(x, z - 1)]; }
                if (0 <= x - 1) { tile.neighbors[TileSO.Direction.X_NEGATIVE] = tiles[gridSO.GetIndexByXZ(x - 1, z)]; }
            }
        }
    }   

    public bool TryPlaceTile(Tile tile, TileSO tileSO) {
        tile.script = tileSO;
        if (!tile.TryPlace()) return false;
        tile.script.gameObject = Object.Instantiate(tileSO.prefab);
        tile.script.gameObject.transform.position = new Vector3(tile.x * gridSO.gap - size / 2 * gridSO.gap, 0f, tile.z * gridSO.gap - size / 2 * gridSO.gap);
        tile.script.gameObject.transform.Rotate(0f, tile.script.rotationY, 0f);
        return true;
    }

    public void PlaceTile(Tile tile, TileSO tileSO) {
        tile.script = tileSO;
        tile.script.gameObject = Object.Instantiate(tileSO.prefab);
        tile.script.gameObject.transform.position = new Vector3(tile.x * gridSO.gap - size / 2 * gridSO.gap, 0f, tile.z * gridSO.gap - size / 2 * gridSO.gap);
        tile.script.gameObject.transform.Rotate(0f, tile.script.rotationY, 0f);
    }
}
