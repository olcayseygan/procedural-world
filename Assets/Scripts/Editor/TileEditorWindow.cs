using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using System;
using UnityEditor.TerrainTools;
using UnityEngine.UIElements;

public class TileEditor : EditorWindow {
    private TileScriptableObject tileScriptableObject;
    private Dictionary<TileScriptableObject.TileSide, List<GameObject>> gameObjectsBySides = new();
    private GameObject loadedTilePrefab;

    private int gapBetweenSideObjects = 1;
    private bool loaded = false;
    [MenuItem("Window/Tile Editor")]
    public static void ShowWindow() {
        GetWindow<TileEditor>("Tile Editor");
    }

    private void OnGUI() {
        GUILayout.BeginVertical();
        EditorGUI.BeginChangeCheck();
        tileScriptableObject = EditorGUILayout.ObjectField("Tile Definition", tileScriptableObject, typeof(TileScriptableObject), false) as TileScriptableObject;
        if (EditorGUI.EndChangeCheck()) {
            LoadTile();
        }

        if (Precheck()) {
            if (GUILayout.Button("Snap Object to Sides")) {
                SnapObjectToSides();
            }

            if (GUILayout.Button("Save Tile")) {
                SaveTile();
            }

            if (GUILayout.Button("Clear")) {
                Clear();
            }

            gapBetweenSideObjects = EditorGUILayout.IntField("Gap Between Side Objects", gapBetweenSideObjects);
        }
        GUILayout.EndVertical();
    }

    private bool Precheck() {
        return tileScriptableObject != null && loaded;
    }

    private void SaveTile() {
        SnapObjectToSides();
        foreach (var side in gameObjectsBySides.Keys.ToArray()) {
            tileScriptableObject.sides
                .First(side_ => side == side_).scripts = gameObjectsBySides[side]
                .Select(gameObject_ => gameObject_ != null ? Resources.Load<TileScriptableObject>("ScriptableObjects\\" + gameObject_.name.Replace("(Clone)", "").Split(' ').First()) : null)
                .Where(resource => resource != null)
                .ToList();
        }
    }

    private void SnapObjectToSides() {
        foreach (var side in gameObjectsBySides.Keys.ToArray()) {
            Vector3Int vector3 = side.GetVector3();
            foreach (GameObject gameObject_ in GetRootGameObjects().Where(gameObject_ => gameObject_.transform.position != Vector3.zero)) {
                Vector3 vectorB = gameObject_.transform.position;
                vectorB.y = 0;
                float cosTheta = CosineOfAngleBetweenVectors(vector3, vectorB);
                if (0.5f < cosTheta) {
                    if (!gameObjectsBySides[side].Contains(gameObject_)) {
                        gameObjectsBySides[side].Add(gameObject_);
                    }
                }
            }
        }

        RelocateSides();
    }

    private void LoadPrefab() {
        loadedTilePrefab = Instantiate(tileScriptableObject.prefab);
        loadedTilePrefab.transform.position = Vector3.zero;
        gameObjectsBySides = new();
        foreach (var direction in (TileScriptableObject.TileSide.Direction[])Enum.GetValues(typeof(TileScriptableObject.TileSide.Direction))) {
            if (!tileScriptableObject.sides.Any(side => side.direction == direction)) {
                TileScriptableObject.TileSide side = new() {
                    direction = direction,
                    scripts = new()
                };
                tileScriptableObject.sides.Add(side);
            }
        }

        foreach (var side in tileScriptableObject.sides) {
            gameObjectsBySides.Add(side, new List<GameObject>());
            for (int i = 0; i < side.scripts.Count; i++) {
                TileScriptableObject tile = side.scripts[i];
                GameObject tilePrefab = Instantiate(tile.prefab);
                gameObjectsBySides[side].Add(tilePrefab);
            }
        }
    }

    private void RelocateSides() {
        if (!Precheck()) return;
        foreach (var side in gameObjectsBySides.Keys.ToArray()) {
            Vector3Int vector3 = side.GetVector3();
            List<GameObject> gameObjectsInScene = gameObjectsBySides[side].Where(gameObject_ => gameObject_ != null).ToList();
            for (int i = 0; i < gameObjectsInScene.Count; i++) {
                gameObjectsInScene[i].transform.position = vector3 * (i + 1) + vector3 * ((i + 1) * gapBetweenSideObjects);
            }
        }
    }

    private List<GameObject> GetRootGameObjects() {
        return FindObjectsOfType<GameObject>().Where(gameObject_ => gameObject_.transform.parent == null).ToList();
    }

    private float CosineOfAngleBetweenVectors(Vector3 a, Vector3 b) {
        return Vector3.Dot(a, b) / (a.magnitude * b.magnitude);
    }

    private void LoadTile() {
        foreach (GameObject gameObject_ in GetRootGameObjects()) {
            DestroyImmediate(gameObject_);
        }

        LoadPrefab();
        RelocateSides();
        loaded = true;
    }
    private void Clear() {
        tileScriptableObject = null;
        loaded = false;
        foreach (GameObject gameObject_ in GetRootGameObjects()) {
            DestroyImmediate(gameObject_);
        }
    }
}
