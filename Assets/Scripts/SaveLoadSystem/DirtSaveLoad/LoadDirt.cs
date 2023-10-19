using UnityEngine;

public class LoadDirt : MonoBehaviour
{
    static AllDirtsData data = new AllDirtsData();
    public static AllDirtsData GetData()
    {
        AllDirtsData result = (AllDirtsData)Loader.Load(data.GetType().FullName);
        return result;
    }
}