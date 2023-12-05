using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class Tile {
    public int x, z;
    public Dictionary<TileSO.Direction, Tile> neighbors = new();
    public TileSO script;
    public HashSet<TileSO> validScripts = new();

    public bool TryPlace() {
        if (script == null) return false;
        ValidNeighbors();
        return true;
    }

    private void ValidNeighbors() {
        if (script == null) return;
        foreach (var neighbor in neighbors) {
            if (neighbor.Value.script != null) continue;
            var scripts = script.GetAllMatchedTileSO(neighbor.Key);
            if (0 < neighbor.Value.validScripts.Count) {
                neighbor.Value.validScripts.IntersectWith(scripts);
            } else {
                neighbor.Value.validScripts.AddRange(scripts);
            }
        }
    }

    public TileSO GetRandomValidScript() {
        if (validScripts.Count == 0) return null;
        return validScripts.ElementAt(Random.Range(0, validScripts.Count));
    }
}
