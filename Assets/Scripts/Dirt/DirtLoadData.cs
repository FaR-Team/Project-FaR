using UnityEngine;

public class DirtLoadData : MonoBehaviour
{
    DirtSaveData dirtSaveData;

    public  DirtSaveData Load()
    {
        dirtSaveData = LoadDirt.GetData().data.Dequeue();
        return dirtSaveData;
    }

    public void SaveMyself()
    {
        DirtSaver.instance.WriteSave(dirtSaveData);
    }
}
