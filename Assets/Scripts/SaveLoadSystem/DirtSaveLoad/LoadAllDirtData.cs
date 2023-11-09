using System;
using UnityEngine;

public static class LoadAllDirtData
{
    public static AllDirtsData GetData(bool isTemporary)
    {
        var data = new AllDirtsData(0);
        string path = PathFinder.GetPath(data.GetType().FullName, isTemporary);

        Loader<AllDirtsData> loader = new Loader<AllDirtsData>();

        try
        {
            AllDirtsData preresult = loader.Load(path);
            AllDirtsData result = new(preresult.dataList, preresult.DirtCounter);
            return result;
        }
        catch (Exception e)
        {
            Debug.LogException(e);
            return null;
        }

    }
}
