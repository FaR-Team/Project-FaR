using System;
using System.Threading.Tasks;
using UnityEngine;

public class DirtLoadData : MonoBehaviour
{
    public Task<DirtData> Load(bool isTemporary)
    {

        try
        {
            DirtData dirtSaveData = LoadAllDirtData.GetData(isTemporary).data.Dequeue();
            return Task.FromResult(dirtSaveData);
        }
        catch (Exception e)
        {

            Debug.LogWarning("Couldn't load DirtSaveData:" + e);

            return Task.FromResult(new DirtData());
        }

    }
}
