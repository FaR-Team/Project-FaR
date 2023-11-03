using UnityEngine;
using System.Threading.Tasks;

[RequireComponent(typeof(Dirt))]
public class SaveDirtData : MonoBehaviour
{
    Dirt dirt;

    private void Awake()
    {
        dirt = GetComponent<Dirt>();
    }

    private void OnEnable()
    {
        DirtSaver.instance.AddSavedObject(this);
    }
    private void OnDisable()
    {
        DirtSaver.instance.RemoveSavedObject(this);
    }
    public async Task SaveData()
    {
        DirtData dirtSaveData = 
            new DirtData(dirt._isWet, dirt.IsEmpty, dirt.currentSeedData, dirt.GetCropSaveData(), transform.position);

        await DirtSaver.instance.WriteSave(dirtSaveData);
    }
}