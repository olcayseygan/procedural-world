using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static TileSO;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Tile", order = 1)]
public class TileSO : ScriptableObject {
    private static List<TileSO> repository_;
    public static List<TileSO> Repository => repository_;
    public static void LoadRepository(string title) {
        if (repository_ == null) {
            repository_ = Resources.LoadAll<TileSO>("ScriptableObjects/" + title).ToList();
            int count = repository_.Count;
            for (int i = 0; i < count; i++) {
                var variants = repository_[i].GenerateVariants();
                repository_.AddRange(variants);
            }
        }
    }

    private static readonly Vector3Int[] vector3s = { Vector3Int.forward, Vector3Int.right, Vector3Int.back, Vector3Int.left };

    private const int SIDES_NUM = 4;
    public GameObject prefab;
    [HideInInspector] public GameObject gameObject;

    [Serializable]
    public enum RotationType {
        CLOCKWISE,
        COUNTER_CLOCKWISE,
    }

    [Serializable]
    public enum Direction { Z_POSITIVE, X_POSITIVE, Z_NEGATIVE, X_NEGATIVE }

    [Serializable]
    private class Rotation {
        public RotationType[] types;
    }

    [Serializable]
    private class Identifier {
        public IdentifierSO[] identifiers;
    }

    public Vector3Int GetVector3(Direction direction) => vector3s[(int)direction];

    [SerializeField] private Rotation[] rotations;
    public float rotationY = 0f;
    [SerializeField] private int identifierLength = 2;
    [SerializeField] private Identifier[] identifiers = new Identifier[SIDES_NUM]; // since, there is four sides in 2D.

    public TileSO[] GenerateVariants() {
        if (rotations == null) return null;
        var variants = new List<TileSO>();
        foreach (var rotation in rotations) {
            var tileSO = Instantiate(this);
            foreach (var rotationType in rotation.types) {
                switch (rotationType) {
                    case RotationType.CLOCKWISE:
                        var lastElement = tileSO.identifiers[^1];
                        for (int i = tileSO.identifiers.Length - 1; i > 0; i--)
                            tileSO.identifiers[i] = tileSO.identifiers[i - 1];
                        tileSO.identifiers[0] = lastElement;
                        tileSO.rotationY += 90f;
                        break;
                    case RotationType.COUNTER_CLOCKWISE:
                        var firstElement = tileSO.identifiers[0];
                        for (int i = 0; i < tileSO.identifiers.Length - 1; i++)
                            tileSO.identifiers[i] = tileSO.identifiers[i + 1];
                        tileSO.identifiers[^1] = firstElement;
                        tileSO.rotationY -= 90f;
                        break;
                    default:
                        break;
                }
            }
            tileSO.name = name + " " + tileSO.rotationY.ToString();
            variants.Add(tileSO);
        }

        return variants.ToArray();
    }

    public Tuple<IdentifierSO[], IdentifierSO[]> GetIdentifiersByDirection(TileSO otherTileSO, Direction direction) {
        static int Module(int a, int b) {
            return (Math.Abs(a * b) + a) % b;
        }

        int index = (int)direction;
        int matchedIndex = Module(index + 2, SIDES_NUM);
        return new Tuple<IdentifierSO[], IdentifierSO[]>(identifiers[index].identifiers, otherTileSO.identifiers[matchedIndex].identifiers);
    }

    public bool IsMatched(TileSO otherTileSO, Direction direction) {
        var identifiers = GetIdentifiersByDirection(otherTileSO, direction);
        var identifiers1 = identifiers.Item1;
        var identifiers2 = identifiers.Item2;
        for (int i = 0; i < identifierLength; i++) {
            if (identifiers1[i] != identifiers2[i]) 
                return false;
        }

        return true;
    }

    public List<TileSO> GetAllMatchedTileSO(Direction direction) {
        return Repository.Where(tileSO => IsMatched(tileSO, direction)).ToList();
    }

    private void OnValidate() {
        if (identifiers == null || identifiers.Length != 4) {
            identifiers = new Identifier[4];
        }

        foreach (var identifier in identifiers) {
            if (identifier.identifiers == null || identifier.identifiers.Length != identifierLength) {
                identifier.identifiers = new IdentifierSO[identifierLength];
            }
        }
    }
}
