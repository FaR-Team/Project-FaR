using UnityEditor;
using UnityEngine;
using System.Linq;

public class ItemsIndexBuilder
{
    [MenuItem("FARUtils/Build Item Index", false, 142)]
    public static void BuildIndex()
    {
        string[] guids = AssetDatabase.FindAssets("t:InventoryItemData");
        var items = guids.Select(g => AssetDatabase.LoadAssetAtPath<InventoryItemData>(AssetDatabase.GUIDToAssetPath(g))).Where(i => i != null).ToArray();

        string resourcesPath = "Assets/Resources";
        if (!AssetDatabase.IsValidFolder(resourcesPath))
            AssetDatabase.CreateFolder("Assets", "Resources");

        string indexPath = System.IO.Path.Combine(resourcesPath, "ItemIndex.asset");
        ItemsIndex index = AssetDatabase.LoadAssetAtPath<ItemsIndex>(indexPath);
        if (index == null)
        {
            index = ScriptableObject.CreateInstance<ItemsIndex>();
            AssetDatabase.CreateAsset(index, indexPath);
        }

        index.items = items;
        EditorUtility.SetDirty(index);
        AssetDatabase.SaveAssets();
        Debug.Log($"ItemsIndex built with {items.Length} items at {indexPath}");
    }
}
