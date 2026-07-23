#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.ShortcutManagement;
using System.Reflection;
using System.Linq;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;
using UnityEditor.Experimental.SceneManagement;
using Type = System.Type;
using static FaRHierarchy.FaRHierarchyData;
using static FaRHierarchy.Libs.VUtils;
using static FaRHierarchy.Libs.VGUI;


namespace FaRHierarchy
{
    [ExecuteInEditMode]
    public abstract class FaRHierarchyDataComponent : MonoBehaviour, ISerializationCallbackReceiver
    {
        public void Awake()
        {
            void register()
            {
                FaRHierarchy.dataComponents_byScene[gameObject.scene] = this;
            }
            void handleSceneDuplication()
            {
                if (sceneData == null) return;
                if (!sceneData.goDatas_byGlobalId.Any()) return;


                var curSceneGuid = gameObject.scene.path.ToGuid();
                var dataSceneGuid = sceneData.goDatas_byGlobalId.Keys.First().guid;

                if (curSceneGuid == dataSceneGuid) return;


                var newDic = new SerializableDictionary<GlobalID, GameObjectData>();

                foreach (var kvp in sceneData.goDatas_byGlobalId)
                    newDic[new GlobalID(kvp.Key.ToString().Replace(dataSceneGuid, curSceneGuid))] = kvp.Value;


                sceneData.goDatas_byGlobalId = newDic;


                EditorSceneManager.MarkSceneDirty(gameObject.scene);
                EditorSceneManager.SaveScene(gameObject.scene);

            }

            register();
            handleSceneDuplication();

        }


        public SceneData sceneData;


        public void OnBeforeSerialize() => FaRHierarchy.goDataCache.Clear();
        public void OnAfterDeserialize() => FaRHierarchy.goDataCache.Clear();



        [CustomEditor(typeof(FaRHierarchyDataComponent), true)]
        class Editor : UnityEditor.Editor
        {
            public override void OnInspectorGUI()
            {
                var style = new GUIStyle(EditorStyles.label) { wordWrap = true };


                void teamModeOn()
                {
                    if (!FaRHierarchyData.teamModeEnabled) return;

                    SetGUIEnabled(false);
                    BeginIndent(0);

                    Space(4);
                    EditorGUILayout.LabelField("This component stores FaRHierarchy's data about which icons and colors are assigned to objects in this scene", style);

                    // Space(6);
                    // EditorGUILayout.LabelField("You can disable Team Mode to store icon/color data in FaRHierarchy Data scriptable object, as it is done by default", style);


                    Space(2);

                    EndIndent(10);
                    ResetGUIEnabled();




                    // Space(10);

                    // if (!GUILayout.Button("Disable Team Mode", GUILayout.Height(27))) return;

                    // FaRHierarchy.data?.DisableTeamMode();

                }
                void teamModeOff()
                {
                    if (FaRHierarchyData.teamModeEnabled) return;

                    SetGUIEnabled(false);
                    BeginIndent(0);

                    Space(4);
                    EditorGUILayout.LabelField("Enable Team Mode to store icon/color data for this scene in this component", style);

                    Space(2);

                    EndIndent(10);
                    ResetGUIEnabled();



                    Space(4);

                    if (!GUILayout.Button("Enable Team Mode", GUILayout.Height(27))) return;

                    FaRHierarchy.data?.EnableTeamMode();

                }

                teamModeOn();
                teamModeOff();


            }
        }

}
}
#else
namespace FaRHierarchy
{
    public abstract class FaRHierarchyDataComponent : UnityEngine.MonoBehaviour {}
}
#endif