using System;
using System.Threading.Tasks;
using UnityEngine;

public class DirtLoadData : MonoBehaviour
{
    DirtSaveData dirtSaveData;

    public Task<DirtSaveData> Load()
    {
        try
        {
            dirtSaveData = LoadAllDirtData.GetData().data.Dequeue();
        }
        catch
        {
            dirtSaveData = new DirtSaveData();
        }

        return  Task.FromResult(dirtSaveData);
    }
}
