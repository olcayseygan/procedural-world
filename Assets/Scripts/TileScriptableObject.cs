using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static TileScriptableObject;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Tile", order = 1)]
public class TileScriptableObject : ScriptableObject {
    public GameObject prefab;
    [HideInInspector] public GameObject gameObject;

    [Serializable]
    public class TileSide {
        public enum Direction { Z_POSITIVE, X_POSITIVE, Z_NEGATIVE, X_NEGATIVE }
        private static readonly Vector3Int[] vector3s = { Vector3Int.forward, Vector3Int.right, Vector3Int.back, Vector3Int.left };

        public Direction direction;
        public List<TileScriptableObject> scripts;

        public Vector3Int GetVector3() => vector3s[(int)direction];
    }

    public List<TileSide> sides;
}
