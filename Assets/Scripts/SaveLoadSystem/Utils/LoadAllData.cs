using System;
using UnityEngine;

public static class LoadAllData
{
    public static T GetData<T>(bool isTemporary) where T : IAllData<T>, new()
    {
        var data = new T();
        string path = PathFinder.GetPath(data.GetType().FullName, isTemporary);

        Loader<T> loader = new Loader<T>();

        try
        {
            T result = new T();
            var preresult = loader.Load(path);
            result.CopyData(preresult);

            return result;
        }
        catch (Exception e)
        {
            throw e; // Return default value for error case
        }
    }
}

public interface IAllData<T>
{
    public void CopyData(T data);
}