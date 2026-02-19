namespace FARUtils.Notes
{
#if UNITY_EDITOR
    using UnityEditor;
    using UnityEditor.SceneManagement;
    using UnityEngine.SceneManagement;

    public static class FaRNoteDeleteMenu
    {
        [MenuItem("FARUtils/FaR Notes/Delete All Notes")]
        private static void DeleteAllNotes()
        {
            bool confirm = EditorUtility.DisplayDialog(
                "Delete All Notes",
                "Are you sure you want to delete all Scene Notes in this scene?",
                "Yes",
                "Cancel"
            );

            if (!confirm)
                return;

            Scene activeScene = SceneManager.GetActiveScene();

            var db = FaRNoteDatabase.GetDatabase();
            int deletedCount = 0;

            db.notes.Clear();

            if (deletedCount > 0)
            {
                EditorSceneManager.MarkSceneDirty(activeScene);
                SceneView.RepaintAll();
            }
        }
    }
#endif
}
