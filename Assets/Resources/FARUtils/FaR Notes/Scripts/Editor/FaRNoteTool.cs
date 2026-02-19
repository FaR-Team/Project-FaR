
namespace FARUtils.Notes
{


#if UNITY_EDITOR
    using System;
    using UnityEditor;
    using UnityEditor.SceneManagement;
    using UnityEngine;
    using UnityEngine.SceneManagement;

    [InitializeOnLoad]
    public static class FaRNoteTool
    {
        private static bool isCreating;
        private static FaRNoteDatabase db;
        private static FaRNoteStyleConfig styleConfig;

        static FaRNoteTool()
        {
            SceneView.duringSceneGui += OnSceneGUI;
            EditorSceneManager.activeSceneChangedInEditMode += OnSceneChanged;
        }

        private static void OnSceneChanged(Scene oldScene, Scene newScene)
        {
            db = null;
            SceneView.RepaintAll();
        }


        public static void SetCreateMode(bool value)
        {
            isCreating = value;
        }


        private static void OnSceneGUI(SceneView view)
        {
            if (db == null)
                db = FaRNoteDatabase.GetDatabase();

            if (db == null)
                return;

            LoadStyleConfig();

            if (styleConfig == null)
                return;

            if (isCreating)
                HandleCreation();

            DrawNotes(view);
        }


        private static void HandleCreation()
        {
            Event e = Event.current;

            if (e.type == EventType.MouseDown && e.button == 0 && !e.alt)
            {
                Ray ray = HandleUtility.GUIPointToWorldRay(e.mousePosition);
                Vector3 position = ray.origin + ray.direction * 5f;

                Undo.RecordObject(db, "Create Scene Note");

                FaRNoteData note = new FaRNoteData
                {
                    position = position,
                    text = ""
                };
                note.Initialize(Environment.UserName, GitHead.GetGitBranch());


                db.notes.Add(note);
                db.selectedIndex = db.notes.Count - 1;

                EditorUtility.SetDirty(db);

                Selection.activeObject = db;
                EditorGUIUtility.PingObject(db);
                SceneView.RepaintAll();

                isCreating = false;

                e.Use();
            }
        }
        private static void LoadStyleConfig()
        {
            if (styleConfig != null)
                return;

            string[] guids = AssetDatabase.FindAssets("t:FaRNoteStyleConfig");

            if (guids.Length > 0)
            {
                string path = AssetDatabase.GUIDToAssetPath(guids[0]);
                styleConfig = AssetDatabase.LoadAssetAtPath<FaRNoteStyleConfig>(path);
            }
        }
        private static Material material;
        private static Mesh quad;
        private static void DrawNotes(SceneView view)
        {
            if (db == null || db.iconConfig == null || styleConfig == null)
                return;

            Event e = Event.current;
            Camera cam = view.camera;
            
            int controlID = GUIUtility.GetControlID(FocusType.Passive);
            
            if (quad == null)
                CreateQuad();

            if (material == null)
                material = new Material(Shader.Find("Unlit/Transparent"));

            Ray mouseRay = HandleUtility.GUIPointToWorldRay(e.mousePosition);
            bool anyHover = false;

            for (int i = 0; i < db.notes.Count; i++)
            {
                FaRNoteData note = db.notes[i];
                if (!FaRNoteOverlay.filterState.IsVisible(note.category)) continue;

                Vector3 notePos = note.WorldPosition;
                Quaternion rotation = Quaternion.LookRotation(cam.transform.forward);
                Vector3 forward = rotation * Vector3.forward;
                
                float distanceToCamera = Vector3.Distance(cam.transform.position, notePos);
                float scale = styleConfig.constantIconSize ? distanceToCamera * 0.05f * styleConfig.iconSize : styleConfig.iconSize;
                
                Plane plane = new Plane(forward, notePos);
                if (plane.Raycast(mouseRay, out float enter))
                {
                    Vector3 hitPoint = mouseRay.GetPoint(enter);
                    float halfSize = scale * 0.5f;
                    Vector3 local = Quaternion.Inverse(rotation) * (hitPoint - notePos);
                    if (Mathf.Abs(local.x) <= halfSize && Mathf.Abs(local.y) <= halfSize)
                    {
                        anyHover = true;
                        break;
                    }
                }
            }

            if (e.type == EventType.Layout && (isCreating || anyHover))
            {
                HandleUtility.AddDefaultControl(controlID);
            }

            for (int i = 0; i < db.notes.Count; i++)
            {
                FaRNoteData note = db.notes[i];

                if (!FaRNoteOverlay.filterState.IsVisible(note.category))
                    continue;

                Sprite icon = db.iconConfig.GetIcon(note.category);
                if (icon == null)
                    continue;

                Quaternion rotation = Quaternion.LookRotation(cam.transform.forward);
                Vector3 forward = rotation * Vector3.forward;
                Vector3 notePos = note.WorldPosition;

                float distanceToCamera = Vector3.Distance(cam.transform.position, notePos);
                float scale = styleConfig.constantIconSize ? distanceToCamera * 0.05f * styleConfig.iconSize : styleConfig.iconSize;
                float finalScale = scale;

                // Constant text size logic
                float textScaleFactor = styleConfig.constantTextSize ? 1.0f : (1.0f / (distanceToCamera * 0.05f));
                int finalFontSize = Mathf.Max(4, Mathf.RoundToInt(styleConfig.inspectorFontSize * textScaleFactor));

                GUIStyle centeredStyle = new(EditorStyles.boldLabel)
                {
                    alignment = TextAnchor.MiddleCenter,
                    fontSize = finalFontSize
                };
                centeredStyle.normal.textColor = styleConfig.inspectorTextColor;

                Handles.Label(notePos + 0.6f * finalScale * Vector3.up, note.title, centeredStyle);

                bool isHover = false;
                bool isSelected = db.selectedIndex == i;

                Plane plane = new Plane(forward, notePos);

                if (plane.Raycast(mouseRay, out float enter))
                {
                    Vector3 hitPoint = mouseRay.GetPoint(enter);
                    float halfSize = scale * 0.5f;

                    Vector3 local = Quaternion.Inverse(rotation) * (hitPoint - notePos);

                    if (Mathf.Abs(local.x) <= halfSize && Mathf.Abs(local.y) <= halfSize)
                    {
                        isHover = true;
                    }
                }

                if (isHover)
                    finalScale *= styleConfig.iconHoverScale;

                if (isSelected)
                    finalScale *= styleConfig.iconSelectedScale;

                material.mainTexture = icon.texture;
                material.color = isSelected ? Color.yellow : Color.white;
                material.SetPass(0);

                Matrix4x4 matrix = Matrix4x4.TRS(
                    notePos,
                    rotation,
                    new Vector3(finalScale, finalScale, 1f)
                );

                Graphics.DrawMeshNow(quad, matrix);

                if (isHover && e.type == EventType.MouseDown && e.button == 0 && !e.alt)
                {
                    db.selectedIndex = i;
                    Selection.activeObject = db;
                    EditorUtility.SetDirty(db);
                    e.Use();
                }
            }

            // Draw movement handles for the selected note
            if (db.selectedIndex >= 0 && db.selectedIndex < db.notes.Count)
            {
                FaRNoteData selectedNote = db.notes[db.selectedIndex];
                
                // Only show handles if the note is NOT locked (editable)
                if (!selectedNote.isLocked)
                {
                    Vector3 currentPos = selectedNote.WorldPosition;
                    EditorGUI.BeginChangeCheck();
                    Vector3 newPos = Handles.PositionHandle(currentPos, Quaternion.identity);
                    if (EditorGUI.EndChangeCheck())
                    {
                        Undo.RecordObject(db, "Move FaR Note");
                        selectedNote.WorldPosition = newPos;
                        EditorUtility.SetDirty(db);
                        SceneView.RepaintAll();
                    }
                }
            }
        }

        private static void CreateQuad()
        {
            quad = new Mesh();

            quad.vertices = new Vector3[]
            {
            new(-0.5f, -0.5f, 0),
            new(0.5f, -0.5f, 0),
            new(-0.5f, 0.5f, 0),
            new(0.5f, 0.5f, 0)
            };

            quad.uv = new Vector2[]
            {
            new(0,0),
            new(1,0),
            new(0,1),
            new(1,1)
            };

            quad.triangles = new int[] { 0, 2, 1, 2, 3, 1 };
        }
    }

#endif
}