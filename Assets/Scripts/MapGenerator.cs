using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MapGenerator : MonoBehaviour {
    [SerializeField] private TileScriptableObject scriptableObject;
    [SerializeField] private TileScriptableObject placeholderScriptableObject;
    [SerializeField] private Vector2Int size;
    private Grid grid;
    Stack<Tile> stack = new();
    private void Start() {
        grid = new(size.x, size.y, scriptableObject);
        PlaceFirstTile(scriptableObject);
        StartCoroutine(GenerateEnumerator());
    }

    private IEnumerator GenerateEnumerator() {
        while (0 < stack.Count) {
            yield return new WaitForSeconds(0.1f);
            PlaceNextStep();
        }
    }

    public void PlaceFirstTile(TileScriptableObject scriptableObject) {
        if (!grid.TryPlaceTile(grid.tiles[0, 0], scriptableObject)) return;
        stack.Push(grid.tiles[0, 0]);
    }

    public void PlaceNextStep() {
        Debug.Log(stack.Count);
        if (stack.Count == 0) return;
        var tile = stack.Peek();
        var randomNeighbor = tile.GetRandomNeighbor();
        if (randomNeighbor == null) {
            stack.Pop();
            return;
        }

        if (!grid.TryPlaceTile(randomNeighbor, randomNeighbor.GetRandomValidScript())) {
            grid.PlaceTile(randomNeighbor, placeholderScriptableObject);
            stack.Pop();
            return;
        }
        stack.Push(randomNeighbor);
    }

    private void OnDrawGizmos() {
        if (!Application.isPlaying) return;
        if (grid == null) return;
        if (stack.Count == 0) return;
        foreach (var tile in grid.tiles) {
            if (tile == null) continue;
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(new Vector3(tile.x, 0f, tile.z), 0.1f);
        }

        Gizmos.color = Color.green;
        Tile tile_ = stack.Peek();
        Gizmos.DrawSphere(new Vector3(tile_.x, 0.5f, tile_.z), 0.2f);
    }
}
