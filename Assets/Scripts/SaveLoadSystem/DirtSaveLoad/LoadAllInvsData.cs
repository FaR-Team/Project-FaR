using System;
using UnityEngine;

public static class LoadAllInvsData
{
    public static AllInventorySystems GetData(bool isTemporary)
    {
        var data = new AllInventorySystems(0);
        string path = PathFinder.GetPath(data.GetType().FullName, isTemporary);

        Loader<AllInventorySystems> loader = new Loader<AllInventorySystems>();

        try
        {
            AllInventorySystems preresult = loader.Load(path);
            AllInventorySystems result = new AllInventorySystems(preresult.dataListValues, preresult.keys, preresult.dataCounter);
            return result;
        }
        catch (Exception e)
        {
            Debug.LogWarning(e);
            return null;
        }

    }
}