using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BiomeGrassSO : ScriptableObject {
    public virtual List<GameObject> GetGameObjects() { return null; }
    public virtual GameObject GetRandomGameObject() { return null; }
}
