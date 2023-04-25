using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class Shortcuts : MonoBehaviour
{
    [MenuItem("GameObject/FARUtils/Instantiate Empty", false, 10)]
    static void CreateTGPButton(MenuCommand menuCommand) {
        var asset = AssetDatabase.LoadAssetAtPath("Assets/Prefabs/Empty.prefab", typeof(GameObject));
        var obj = PrefabUtility.InstantiatePrefab(asset) as GameObject;
        GameObjectUtility.SetParentAndAlign(obj, menuCommand.context as GameObject);
        Undo.RegisterCreatedObjectUndo(obj, "Create Empty");
        Selection.activeObject = obj;
    }
}
