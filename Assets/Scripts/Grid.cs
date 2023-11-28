using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class Grid {
    public int width, depth;

    public Tile[,] tiles;

    public Grid(int width, int depth, TileScriptableObject scriptableObject) {
        this.width = width;
        this.depth = depth;
        CreateTiles();
        AddNeighbors();
    }

    private void CreateTiles() {
        tiles = new Tile[width, depth];
        for (int x = 0; x < width; x++) {
            for (int z = 0; z < depth; z++) {
                tiles[x, z] = new Tile() { x = x, z = z };
            }
        }        
    }

    private void AddNeighbors() {
        for (int x = 0; x < width; x++) {
            for (int z = 0; z < depth; z++) {
                Tile tile = tiles[x, z];
                if (0 <= x - 1) { tile.neighbors.Add(tiles[x - 1, z]); }
                if (x + 1 < width) { tile.neighbors.Add(tiles[x + 1, z]); }
                if (0 <= z - 1) { tile.neighbors.Add(tiles[x, z - 1]); }
                if (z + 1 < depth) { tile.neighbors.Add(tiles[x, z + 1]); }
            }
        }
    }   

    public bool TryPlaceTile(Tile tile, TileScriptableObject scriptableObject) {
        tile.script = scriptableObject;
        if (!tile.TryPlace()) return false;
        tile.script.gameObject = Object.Instantiate(scriptableObject.prefab);
        tile.script.gameObject.transform.position = new Vector3(tile.x, 0f, tile.z);
        return true;
    }

    public void PlaceTile(Tile tile, TileScriptableObject scriptableObject) {
        tile.script = scriptableObject;
        tile.script.gameObject = Object.Instantiate(scriptableObject.prefab);
        tile.script.gameObject.transform.position = new Vector3(tile.x, 0f, tile.z);
    }
}
