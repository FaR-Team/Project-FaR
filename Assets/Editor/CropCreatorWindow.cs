using UnityEngine;
using UnityEditor;
using System.IO;

public enum CropType
{
    Tuber,
    Tree,
    Bush
}

public class CropCreatorWindow : EditorWindow
{
    private string cropName = "";
    private Sprite cropIcon;
    private int growthStages = 3;
    private TypeOfItem itemType = TypeOfItem.CropSeed;
    private CropType cropType = CropType.Tuber;
    private int sellPrice = 1;
    private GameObject basePrefab;
    
    // Arrays to store mesh and material for each stage
    private Mesh[] stageMeshes;
    private Material[] stageMaterials;
    private Vector2 scrollPosition;
    // Add these fields to store the days
    private int[] minDays;
    private int[] maxDays;

    private void OnEnable()
    {
        stageMeshes = new Mesh[5];
        stageMaterials = new Material[5];
        minDays = new int[5];
        maxDays = new int[5];
    }

    [MenuItem("FARUtils/Crop Creator")]
    public static void ShowWindow()
    {
        GetWindow<CropCreatorWindow>("Crop Creator");
    }

    private void OnGUI()
    {
        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
        
        GUILayout.Label("Crop Creator", EditorStyles.boldLabel);
        
        // Basic properties
        cropName = EditorGUILayout.TextField("Crop Name", cropName);
        cropIcon = (Sprite)EditorGUILayout.ObjectField("Crop Icon", cropIcon, typeof(Sprite), false);
        
        // Growth stages with dynamic mesh/material fields
        EditorGUI.BeginChangeCheck();
        growthStages = EditorGUILayout.IntSlider("Growth Stages", growthStages, 1, 5);
        if (EditorGUI.EndChangeCheck())
        {
            if (stageMeshes == null || stageMeshes.Length != 5)
            {
                stageMeshes = new Mesh[5];
                stageMaterials = new Material[5];
                minDays = new int[5];
                maxDays = new int[5];
            }
        }

        GUILayout.Label("Stage Properties", EditorStyles.boldLabel);
        for (int i = 0; i < growthStages; i++)
        {
            EditorGUILayout.BeginVertical("box");
            GUILayout.Label($"Stage {i + 1}", EditorStyles.boldLabel);
            
            stageMeshes[i] = (Mesh)EditorGUILayout.ObjectField($"Stage {i + 1} Mesh", stageMeshes[i], typeof(Mesh), false);
            stageMaterials[i] = (Material)EditorGUILayout.ObjectField($"Stage {i + 1} Material", stageMaterials[i], typeof(Material), false);
            
            EditorGUILayout.BeginHorizontal();
            minDays[i] = EditorGUILayout.IntField("Min Days", minDays[i]);
            maxDays[i] = EditorGUILayout.IntField("Max Days", maxDays[i]);
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.EndVertical();
        }

        cropType = (CropType)EditorGUILayout.EnumPopup("Crop Type", cropType);
        sellPrice = EditorGUILayout.IntField("Sell Price", sellPrice);
        basePrefab = (GameObject)EditorGUILayout.ObjectField("Base Prefab", basePrefab, typeof(GameObject), false);

        itemType = cropType == CropType.Tree ? TypeOfItem.TreeSeed : TypeOfItem.CropSeed;

        if (GUILayout.Button("Create Crop Assets"))
        {
            CreateCropAssets();
        }

        EditorGUILayout.EndScrollView();
    }

    private void CreateCropAssets()
    {
        // Validation
        if (string.IsNullOrEmpty(cropName) || cropIcon == null || basePrefab == null)
        {
            EditorUtility.DisplayDialog("Error", "Please fill all required fields", "OK");
            return;
        }

        for (int i = 0; i < growthStages; i++)
        {
            if (stageMeshes[i] == null || stageMaterials[i] == null)
            {
                EditorUtility.DisplayDialog("Error", $"Please assign mesh and material for stage {i + 1}", "OK");
                return;
            }
        }

        // Create directories and assets
        string baseDir = "Assets/Crops/" + cropName;
        CreateDirectory(baseDir);
        CreateDirectory(baseDir + "/States");
        CreateDirectory(baseDir + "/Prefabs");

        // Create SeedItemData
        var seedData = CreateInstance<SeedItemData>();
        seedData.Nombre = cropName + " Seeds";
        seedData.Icono = cropIcon;
        seedData.typeOfItem = itemType;
        AssetDatabase.CreateAsset(seedData, $"{baseDir}/{cropName}SeedData.asset");

        // Create CropItemData
        var cropData = CreateInstance<CropItemData>();
        cropData.Nombre = cropName;
        cropData.Icono = cropIcon;
        cropData.Valor = sellPrice;
        AssetDatabase.CreateAsset(cropData, $"{baseDir}/{cropName}CropData.asset");

        // Create Growing States with assigned meshes and materials
        for (int i = 0; i < growthStages; i++)
        {
            var state = CreateInstance<GrowingState>();
            state.mesh = stageMeshes[i];
            state.material = stageMaterials[i];
            state.minimalDay = minDays[i];
            state.maximalDay = maxDays[i];
            state.isLastPhase = (i == growthStages - 1);
            AssetDatabase.CreateAsset(state, $"{baseDir}/States/{cropName}State_{i}.asset");
        }

        // Create Prefabs
        CreatePrefabVariant(basePrefab, $"{baseDir}/Prefabs/{cropName}Ghost.prefab", "Ghost");
        CreatePrefabVariant(basePrefab, $"{baseDir}/Prefabs/{cropName}Dirt.prefab", "Dirt");
        CreatePrefabVariant(basePrefab, $"{baseDir}/Prefabs/{cropName}Drop.prefab", "Drop");
        CreatePrefabVariant(basePrefab, $"{baseDir}/Prefabs/{cropName}SellBox.prefab", "SellBox");

        if (cropType == CropType.Tuber)
        {
            CreatePrefabVariant(basePrefab, $"{baseDir}/Prefabs/{cropName}Interactable.prefab", "Interactable");
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        EditorUtility.DisplayDialog("Success", $"Created crop assets for {cropName}", "OK");
    }
    private void CreateDirectory(string path)
    {
        if (!Directory.Exists(path))
            Directory.CreateDirectory(path);
    }

    private void CreatePrefabVariant(GameObject original, string path, string type)
    {
        GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(original);
        instance.name = $"{cropName}{type}";
        PrefabUtility.SaveAsPrefabAsset(instance, path);
        DestroyImmediate(instance);
    }
}
