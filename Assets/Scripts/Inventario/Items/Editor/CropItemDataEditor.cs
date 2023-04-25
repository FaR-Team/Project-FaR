using UnityEditor;

[CustomEditor(typeof(CropItemData))]
public class CropItemDataEditor : Editor
{
    #region SerializedProperties
    SerializedProperty ID;
    SerializedProperty Nombre;
    SerializedProperty Descripción;
    SerializedProperty Icono;
    SerializedProperty Valor;
    SerializedProperty ItemPrefab;
    SerializedProperty DirtPrefabGhost;
    SerializedProperty CropBoxPrefab;
    SerializedProperty SellSystem;
    SerializedProperty Usable;
    SerializedProperty Sellable;
    #endregion

    void OnEnable() 
    {
        ID = serializedObject.FindProperty("ID");
        Nombre = serializedObject.FindProperty("Nombre");
        Descripción = serializedObject.FindProperty("Descripción");
        Icono = serializedObject.FindProperty("Icono");
        Valor = serializedObject.FindProperty("Valor");
        ItemPrefab = serializedObject.FindProperty("ItemPrefab");
        DirtPrefabGhost = serializedObject.FindProperty("DirtPrefabGhost");
        CropBoxPrefab = serializedObject.FindProperty("CropBoxPrefab");
        SellSystem = serializedObject.FindProperty("_sellSystem");
        Usable = serializedObject.FindProperty("Usable");
        Sellable = serializedObject.FindProperty("Sellable");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(ID);
        EditorGUILayout.PropertyField(Nombre);
        EditorGUILayout.PropertyField(Descripción);
        EditorGUILayout.PropertyField(Icono);
        EditorGUILayout.PropertyField(Valor);
        EditorGUILayout.PropertyField(ItemPrefab);
        EditorGUILayout.PropertyField(DirtPrefabGhost);
        EditorGUILayout.PropertyField(CropBoxPrefab);
        EditorGUILayout.PropertyField(SellSystem);
        EditorGUILayout.PropertyField(Usable);
        EditorGUILayout.PropertyField(Sellable);

        serializedObject.ApplyModifiedProperties();
    }
}
