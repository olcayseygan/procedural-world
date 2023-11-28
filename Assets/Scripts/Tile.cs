using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class Tile {
    public int x, z;
    public List<Tile> neighbors = new();
    public TileScriptableObject script;
    public HashSet<TileScriptableObject> validScripts = new();
    public List<GameObject> placeholderGameObjects = new();

    public void ValidNeighbors() {
        if (script == null) return;
        foreach (Tile neighbor in neighbors) {
            if (neighbor.script != null) continue;
            int dX = neighbor.x - x;
            int dZ = neighbor.z - z;
            var side = script.sides.FirstOrDefault(side => {
                var vector3 = side.GetVector3();
                return vector3.x == dX && vector3.z == dZ;
            });
            if (side == null) continue;
            if (0 < neighbor.validScripts.Count) {
                neighbor.validScripts.IntersectWith(side.scripts);
            } else {
                neighbor.validScripts.AddRange(side.scripts);
            }
        }
    }

    public bool TryPlace() {
        if (script == null) return false;
        foreach (Tile neighbor in neighbors) {
            foreach (var placeholderGameObject in neighbor.placeholderGameObjects) {
                GameObject.Destroy(placeholderGameObject);
            }

            if (neighbor.script != null)  continue;
          
            int dX = neighbor.x - x;
            int dZ = neighbor.z - z;
            var side = script.sides.FirstOrDefault(side => {
                var vector3 = side.GetVector3();
                return vector3.x == dX && vector3.z == dZ;
            });
            if (side == null) continue;
            if (0 < neighbor.validScripts.Count) {
                neighbor.validScripts.IntersectWith(side.scripts);
            } else {
                neighbor.validScripts.AddRange(side.scripts);
            }

            for (int i = 0; i < neighbor.validScripts.Count; i++) {
                GameObject placeholderGameObject = GameObject.Instantiate(neighbor.validScripts.ToArray()[i].prefab);
                placeholderGameObject.transform.localScale = Vector3.one / 2f;
                placeholderGameObject.transform.position = new Vector3(neighbor.x, i + 1, neighbor.z);
                neighbor.placeholderGameObjects.Add(placeholderGameObject);
            }
        }

        foreach (var placeholderGameObject in placeholderGameObjects) {
            GameObject.Destroy(placeholderGameObject);
        }
        return true;
    }

    public Tile GetRandomNeighbor() {
        if (neighbors.Count == 0) return null;
        var validNeighbors = neighbors.Where(neighbor => neighbor.script == null).ToList();
        if (validNeighbors.Count == 0) return null;
        return validNeighbors[Random.Range(0, validNeighbors.Count)];
    }

    public TileScriptableObject GetRandomValidScript() {
        if (validScripts.Count == 0) return null;
        return validScripts.ElementAt(Random.Range(0, validScripts.Count));
    }
}
