using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Node", order = 1)]
public class NodeScriptableObject : ScriptableObject {
    public GameObject prefab;
    [HideInInspector] public GameObject gameObject;

    [Serializable]
    public class NodeSide {
        public enum Direction {  FORWARD, RIGTH, BACK, LEFT }
        private static readonly Vector3[] vector3s = { Vector3.forward, Vector3.right, Vector3.back, Vector3.left };

        public Direction direction;
        public NodeScriptableObject[] sideNodes;

        public NodeScriptableObject GetRandomSideNode() => 0 < sideNodes.Length ? sideNodes[UnityEngine.Random.Range(0, sideNodes.Length)] : null;

        public Vector3 GetVector3() => vector3s[(int)direction];
    }

    public NodeSide[] nodeSides;

    public NodeSide GetRandomNodeSide() => 0 < nodeSides.Length ? nodeSides[UnityEngine.Random.Range(0, nodeSides.Length)] : null;
}
