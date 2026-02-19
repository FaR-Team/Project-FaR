namespace FARUtils.Notes
{

#if UNITY_EDITOR
    using UnityEngine;
    using UnityEditor;
    using UnityEditor.SceneManagement;
    using System.Collections.Generic;
    using System.IO;

    public class FaRNoteDatabase : ScriptableObject
    {
        public List<FaRNoteData> notes = new List<FaRNoteData>();
        public int selectedIndex = -1;
        public FaRNoteIconConfig iconConfig;
        public float globalIconSize = 32f;

        private void OnEnable()
        {
            if (iconConfig == null)
            {
                string[] guids = AssetDatabase.FindAssets("t:FaRNoteIconConfig");

                if (guids.Length > 0)
                {
                    string path = AssetDatabase.GUIDToAssetPath(guids[0]);
                    iconConfig = AssetDatabase.LoadAssetAtPath<FaRNoteIconConfig>(path);
                }
            }
        }


        public static FaRNoteDatabase GetDatabase()
        {
            var scene = EditorSceneManager.GetActiveScene();

            if (string.IsNullOrEmpty(scene.path))
                return null;

            string folder = "Assets/Resources/FARUtils/FaR Notes/FaRNotes";

            string assetPath = $"{folder}/{scene.name}_Notes.asset";

            var db = AssetDatabase.LoadAssetAtPath<FaRNoteDatabase>(assetPath);

            if (db == null)
            {
                if (!Directory.Exists(folder))
                    Directory.CreateDirectory(folder);

                db = CreateInstance<FaRNoteDatabase>();
                AssetDatabase.CreateAsset(db, assetPath);
                AssetDatabase.SaveAssets();
            }

            return db;
        }
    }
#endif
}