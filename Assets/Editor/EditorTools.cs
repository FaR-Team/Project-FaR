using UnityEngine;
using UnityEditor;
using System.Collections.Generic;


public class EditorTools : Editor
{
    [MenuItem("FARUtils/Remesh Skin")]
    static void RemeshSkin()
    {
        Selection.activeGameObject.transform.GetComponent<SkinnedMeshRenderer>().ResetBounds();
        Selection.activeGameObject.transform.GetComponent<SkinnedMeshRenderer>().bones = BuildBonesArray(Selection.activeGameObject.transform.GetComponent<SkinnedMeshRenderer>().rootBone, Selection.activeGameObject.transform.GetComponent<SkinnedMeshRenderer>().bones);
    }

    static Transform[] BuildBonesArray(Transform rootBone, Transform[] bones)
    {
        List<Transform> boneList = new List<Transform>();
        ExtractBonesRecursively(rootBone, ref boneList);

        List<Transform> Reorder = new List<Transform>();
        foreach(Transform bone in bones)
        {
            foreach(Transform extractbone in boneList)
            {
                if(bone.name == extractbone.name)
                {
                    Reorder.Add(extractbone);
                }
            }

        }

        return Reorder.ToArray();
    }

    static void ExtractBonesRecursively(Transform bone, ref List<Transform> boneList)
    {
        boneList.Add(bone);

        for (int i = 0; i < bone.childCount; i++)
        {
            ExtractBonesRecursively(bone.GetChild(i), ref boneList);
        }
    }

    [MenuItem("GameObject/FARUtils/Instantiate Empty", false, 10)]
    static void CreateTGPButton(MenuCommand menuCommand) {
        var asset = AssetDatabase.LoadAssetAtPath("Assets/Prefabs/Empty.prefab", typeof(GameObject));
        var obj = PrefabUtility.InstantiatePrefab(asset) as GameObject;
        GameObjectUtility.SetParentAndAlign(obj, menuCommand.context as GameObject);
        Undo.RegisterCreatedObjectUndo(obj, "Create Empty");
        Selection.activeObject = obj;
    }

}





