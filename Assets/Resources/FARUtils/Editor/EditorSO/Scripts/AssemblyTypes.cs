using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;

namespace FaRUtils.Assembly
{
    public static class AssemblyTypes
    {
        public static System.Type[] ReturnTypes()
        {
            List<System.Type> types = new List<System.Type>();
            var assemblies = System.AppDomain.CurrentDomain.GetAssemblies().Where(x => !x.IsDynamic);
            foreach (var item in assemblies)
            {
                switch (true)
                {
                    case bool x when item.CodeBase.Contains("Assembly-CSharp.dll"):
                        types = types.Concat(item.GetTypes()).ToList();
                        break;
                }
            }

            return types.ToArray();
        }

        public static ScriptableObject[] GetAllInstancesOfType(string activePath, System.Type activeType)
        {
            string[] guids = AssetDatabase.FindAssets("t:" + activeType.Name, new[] { activePath });
            ScriptableObject[] a = new ScriptableObject[guids.Length];
            for (int i = 0; i < guids.Length; i++)
            {
                string path = AssetDatabase.GUIDToAssetPath(guids[i]);
                a[i] = (ScriptableObject)AssetDatabase.LoadAssetAtPath(path, activeType);
            }
            return a;
        }

        public static System.Type[] GetAllTypes()
        {
            System.Type[] types = ReturnTypes();
            System.Type[] possible = (from System.Type type in types where type.IsSubclassOf(typeof(ScriptableObject)) select type).ToArray();

            return possible;
        }

        public static ScriptableObject FindObject(ScriptableObject[] objects, string property)
        {
            return objects.Where(x => x.name == property).First();
        }

        public static Object CreateObject(System.Type type)
        {
            return ScriptableObject.CreateInstance(type);
        }

        public static Rect CenterOnOriginWindow(Rect window, Rect origin)
        {
            var pos = window;
            float w = (origin.width - pos.width) * 0.5f;
            float h = (origin.height - pos.height) * 0.5f;
            pos.x = origin.x + w;
            pos.y = origin.y + h;
            return pos;
        }

        public static bool OfType(System.Type type, System.Type compareType)
        {
            return compareType == type;
        }
    }
}