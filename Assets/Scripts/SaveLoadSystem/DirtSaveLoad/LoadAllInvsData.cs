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
            return loader.Load(path);
        }
        catch
        {
            throw;
        }
    }
}

public class LoadAllContainer : MonoBehaviour
{
    public static LoadAllContainer instance;
    private void Awake()
    {
        instance = this;
    }
    private void Start()
    {
        LoadChest();
    }

    private void LoadChest()
    {
        try
        {
            var data = LoadAllInvsData.GetData(false);

            for (int i = 0; i < data.dataCounter; i++)
            {
                /*
                 crear cada cofre en el lugar que corresponde y a su vez con los items
                 */
            }

            /*
         por cada 

         */
        }
        catch
        {

        }
        throw new NotImplementedException();
    }
}