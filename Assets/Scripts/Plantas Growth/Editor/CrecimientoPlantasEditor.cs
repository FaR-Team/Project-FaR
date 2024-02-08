using UnityEditor;

[CustomEditor(typeof(CrecimientoPlantas))]
public class CrecimientoPlantasEditor : Editor
{
    #region SerializedProperties
    SerializedProperty Fase1;
    SerializedProperty Fase2;
    SerializedProperty Fase3;
    SerializedProperty Fase4;
    SerializedProperty Fase5;
    SerializedProperty Fase6;
    SerializedProperty Fase7;
    SerializedProperty Fase8;
    SerializedProperty Fase9;
    SerializedProperty Fase10;

    bool FaseGroup = false;

    SerializedProperty Reloj;
    SerializedProperty Día;
    SerializedProperty YaCreció;
    SerializedProperty CropSaveData;

    bool MiscGroup = false;

    SerializedProperty Int1;
    SerializedProperty Int2;
    SerializedProperty Int3;
    SerializedProperty Int4;
    SerializedProperty Int5;
    SerializedProperty Int6;
    SerializedProperty Int7;
    SerializedProperty Int8;
    SerializedProperty Int9;

    bool IntGroup = false;
    #endregion

    void OnEnable() 
    {
        Fase1 = serializedObject.FindProperty("Fase1");
        Fase2 = serializedObject.FindProperty("Fase2");
        Fase3 = serializedObject.FindProperty("Fase3");
        Fase4 = serializedObject.FindProperty("Fase4");
        Fase5 = serializedObject.FindProperty("Fase5");
        Fase6 = serializedObject.FindProperty("Fase6");
        Fase7 = serializedObject.FindProperty("Fase7");
        Fase8 = serializedObject.FindProperty("Fase8");
        Fase9 = serializedObject.FindProperty("Fase9");
        Fase10 = serializedObject.FindProperty("Fase10");

        YaCreció = serializedObject.FindProperty("yacrecio");
        Día = serializedObject.FindProperty("Dia");
        Reloj = serializedObject.FindProperty("Reloj");
        CropSaveData = serializedObject.FindProperty("cropSaveData");

        Int1 = serializedObject.FindProperty("Int1");
        Int2 = serializedObject.FindProperty("Int2");
        Int3 = serializedObject.FindProperty("Int3");
        Int4 = serializedObject.FindProperty("Int4");
        Int5 = serializedObject.FindProperty("Int5");
        Int6 = serializedObject.FindProperty("Int6");
        Int7 = serializedObject.FindProperty("Int7");
        Int8 = serializedObject.FindProperty("Int8");
        Int9 = serializedObject.FindProperty("Int9");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        FaseGroup = EditorGUILayout.BeginFoldoutHeaderGroup(FaseGroup, "Fases");

        if(FaseGroup)
        {
            EditorGUILayout.PropertyField(Fase1);
            EditorGUILayout.PropertyField(Fase2);
            EditorGUILayout.PropertyField(Fase3);
            EditorGUILayout.PropertyField(Fase4);
            EditorGUILayout.PropertyField(Fase5);
            EditorGUILayout.PropertyField(Fase6);
            EditorGUILayout.PropertyField(Fase7);
            EditorGUILayout.PropertyField(Fase8);
            EditorGUILayout.PropertyField(Fase9);
            EditorGUILayout.PropertyField(Fase10);
        }
        EditorGUILayout.EndFoldoutHeaderGroup();

        IntGroup = EditorGUILayout.BeginFoldoutHeaderGroup(IntGroup, "Días para cambiar de Fase");

        if(IntGroup)
        {
            EditorGUILayout.PropertyField(Int1);
            EditorGUILayout.PropertyField(Int2);
            EditorGUILayout.PropertyField(Int3);
            EditorGUILayout.PropertyField(Int4);
            EditorGUILayout.PropertyField(Int5);
            EditorGUILayout.PropertyField(Int6);
            EditorGUILayout.PropertyField(Int7);
            EditorGUILayout.PropertyField(Int8);
            EditorGUILayout.PropertyField(Int9);
        }
        EditorGUILayout.EndFoldoutHeaderGroup();

        MiscGroup = EditorGUILayout.BeginFoldoutHeaderGroup(MiscGroup, "Misceláneo");

        if(MiscGroup)
        {
            EditorGUILayout.PropertyField(Reloj);
            EditorGUILayout.PropertyField(Día);
            EditorGUILayout.PropertyField(YaCreció);
            EditorGUILayout.PropertyField(CropSaveData);
        }
        EditorGUILayout.EndFoldoutHeaderGroup();

        serializedObject.ApplyModifiedProperties();
    }
}
