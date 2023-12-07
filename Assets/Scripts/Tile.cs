using System.Collections.Generic;
using System.Linq;
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
                foreach (var script_ in scripts)
                    neighbor.Value.validScripts.Add(script_);
            }
        }
    }

    public TileSO GetRandomValidScript() {
        if (validScripts.Count == 0) return null;
        return validScripts.ElementAt(Random.Range(0, validScripts.Count));
    }
}
