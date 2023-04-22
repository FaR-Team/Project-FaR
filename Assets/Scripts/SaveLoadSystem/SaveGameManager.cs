using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveGameManager : MonoBehaviour
{
    public static SaveData data = new SaveData();

    private void Start() {
        SaveLoad.OnLoadGame += LoadData;
    }
    public void DeleteData()
    {
        SaveLoad.DeleteSaveData();
    }

    public static void SaveData()
    {
        var SaveData = data;
        //SaveData.Items.Add();

        SaveLoad.Save(SaveData);
    }

    public static void LoadData(SaveData _data)
    {
        data = _data;
    }

    public static void TryLoadData()
    {
        SaveLoad.load();
    }
}
