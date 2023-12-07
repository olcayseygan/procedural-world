using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using TreeEditor;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.UIElements;

[CustomEditor(typeof(TreeSO))]
public class TreeSOEditor : Editor {
    SerializedProperty prefabProperty;
    SerializedProperty prefabsProperty;

    private void OnEnable() {
        prefabProperty = serializedObject.FindProperty("prefab");
        prefabsProperty = serializedObject.FindProperty("prefabs");
    }

    public override void OnInspectorGUI() {
        base.OnInspectorGUI();
        if (GUILayout.Button("Generate Variants")) {
            GameObject prefab = prefabProperty.objectReferenceValue as GameObject;
            int variantsCount = prefabsProperty.arraySize;
            ClearPrefabs();
            for (int i = 0; i < variantsCount; i++) {
                GameObject variantPrefab = CreateVariantPrefab(prefab, i);
                Tree tree = variantPrefab.GetComponent<Tree>();
                TreeData treeData = Instantiate(tree.data as TreeData);
                TreeGroupRoot root = treeData.root;
                root.seed = Random.Range(0, 9999999);
                tree.data = treeData;
                treeData.UpdateMesh(tree.transform.worldToLocalMatrix, out var outMaterials);
                MeshFilter meshFilter = variantPrefab.GetComponent<MeshFilter>();
                meshFilter.sharedMesh = treeData.mesh;

                AddPrefabToPrefabs(variantPrefab);
                Debug.LogFormat("[TreeOS](VariantGeneration) {0}/{1}({2}%)", i + 1, variantsCount, (i + 1) / variantsCount * 100);
            }

            AssetDatabase.Refresh();
        }

        void ClearPrefabs() {
            prefabsProperty.ClearArray();
            serializedObject.ApplyModifiedProperties();
        }

        void AddPrefabToPrefabs(GameObject variantPrefab) {
            int index = prefabsProperty.arraySize;
            prefabsProperty.InsertArrayElementAtIndex(index);
            SerializedProperty newElementProperty = prefabsProperty.GetArrayElementAtIndex(index);
            newElementProperty.objectReferenceValue = variantPrefab;
            serializedObject.ApplyModifiedProperties();
        }
    }

    private GameObject CreateVariantPrefab(GameObject prefab, int index) {
        string prefabPath = GetPrefabPath(prefab);
        string folderPath = CreateVariantTreeFolderIfNotExists(prefab, prefabPath);
        string variantPath = CopyPrefabFile(prefab, prefabPath, folderPath, index);
        AssetDatabase.Refresh();
        return AssetDatabase.LoadAssetAtPath<GameObject>(variantPath);
    }

    private string CopyPrefabFile(GameObject prefab, string prefabPath, string folderPath, int index) {
        string variantName = string.Format("{0}_{1}.prefab", prefab.name, index);
        string variantPath = Path.Join(folderPath, variantName);
        File.Copy(prefabPath, variantPath, true);
        return variantPath;
    }

    private string GetPrefabPath(GameObject prefab) {
        return AssetDatabase.GetAssetPath(prefab);
    }

    private string CreateVariantTreeFolderIfNotExists(GameObject prefab, string prefabPath) {
        string folderPath = Path.Join(Path.GetDirectoryName(prefabPath), prefab.name);
        if (true) {
            if (!Directory.Exists(folderPath)) {
                AssetDatabase.CreateFolder(Path.GetDirectoryName(prefabPath), prefab.name);
            }
        } else {
            // delete folder if exists
        }

        return folderPath;
    }
}
