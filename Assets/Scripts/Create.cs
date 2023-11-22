using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Create : MonoBehaviour {
    [SerializeField] private NodeScriptableObject node;

    private void Start() {
        Instantiate(node.prefab, transform.position, Quaternion.identity);

        Stack<NodeScriptableObject> stack = new();
        GameObject gameObject_ = Instantiate(node.prefab, transform.position, Quaternion.identity);
        node.gameObject = gameObject_;
        stack.Push(node);

        int i = 0;
        while (0 < stack.Count && i++ < 30) {
            NodeScriptableObject currentNode = stack.Peek();
            NodeScriptableObject.NodeSide side = currentNode.GetRandomNodeSide();
            NodeScriptableObject randomSideNode = side?.GetRandomSideNode();
            Debug.Log(stack.Count);
            if (randomSideNode == null) {
                stack.Pop();
                continue;
            }

            stack.Push(randomSideNode);
            GameObject gameObject__ = Instantiate(randomSideNode.prefab);
            gameObject__.transform.position = currentNode.gameObject.transform.position + side.GetVector3();
        }
    }
}
