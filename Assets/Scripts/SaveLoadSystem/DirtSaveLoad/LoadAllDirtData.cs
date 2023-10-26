using UnityEditor.Localization.Plugins.XLIFF.V20;
using UnityEngine;

public class LoadAllDirtData : MonoBehaviour
{
    public static AllDirtsData GetData()
    {
        AllDirtsData data = new AllDirtsData(0);
        string path;
        try
        {
            path = PathFinder.GetPath(data.GetType().FullName, false);

            path += ".json";
        }
        catch
        {
            return null;
        }

        AllDirtsData result = (AllDirtsData)Loader.Load(path);
        result.LoadQueue();
        return result;
    }
}