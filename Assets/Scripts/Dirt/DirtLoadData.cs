using System;
using System.Threading.Tasks;
using UnityEngine;

public class DirtLoadData : MonoBehaviour
{
    public Task<DirtSaveData> Load(bool isTemporary)
    {

        try
        {
            DirtSaveData dirtSaveData = LoadAllDirtData.GetData(isTemporary).data.Dequeue();
            return Task.FromResult(dirtSaveData);
        }
        catch (Exception e)
        {

            Debug.LogWarning("Couldn't load DirtSaveData:" + e);

            return Task.FromResult(new DirtSaveData());
        }

    }
}
