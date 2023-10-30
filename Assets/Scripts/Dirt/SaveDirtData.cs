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
        DirtSaver.instance.AddDirt(this);
    }
    private void OnDisable()
    {
        DirtSaver.instance.RemoveDirt(this);
    }
    public async Task SaveData()
    {
        Debug.Log("saving Dirt");

        DirtSaveData dirtSaveData = new DirtSaveData(dirt._isWet, dirt.IsEmpty, dirt.currentCrop, dirt.currentCropData, dirt.cropSaveData, transform.position);
        
        await DirtSaver.instance.WriteSave(dirtSaveData);
    }
}