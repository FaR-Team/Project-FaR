using System.Collections.Generic;
using System.Linq;
using UnityEngine.Serialization;

[System.Serializable]
public class AllChestSystems : IAllData<AllChestSystems>
{
    public List<ChestData> currentDataList;
    public Queue<ChestData> data;
    public List<SceneChestData> scenesDataList;
    public int dataCounter;
    
    public AllChestSystems()
    {
        dataCounter = 0;
        currentDataList = new List<ChestData>();
        scenesDataList = new();
        data = new Queue<ChestData>();
    }
   
    public void SaveQueue(string sceneName)
    {
        currentDataList = data.ToList();
        
        SceneChestData newData = new SceneChestData()
        {
            sceneName = sceneName,
            datas = data.ToList()
        };
        
        int sceneIndex = scenesDataList.FindIndex(sceneData => sceneData.sceneName.Equals(sceneName));
        
        if (sceneIndex != -1) // If found data with Scene Name
        {
            scenesDataList[sceneIndex] = newData;
        }
        else
        {
            scenesDataList.Add(newData);
        }
    }

    public void LoadQueue()
    {
        foreach (var item in currentDataList)
        {
            data.Enqueue(item);
        }
    }
    
    public void LoadQueue(List<ChestData> list)
    {
        foreach (var item in list)
        {
            data.Enqueue(item);
        }
    }

    public void CopyData(AllChestSystems allChestsData)
    {
        currentDataList = allChestsData.currentDataList;
        dataCounter = allChestsData.dataCounter;
        scenesDataList = allChestsData.scenesDataList;
        LoadQueue(GetSceneDataFromName(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name).datas);
    }
    
    public void ClearAfterSave()
    {
        dataCounter = 0;
        data.Clear();
    }
    
    public void SetScenesDataOnLoad(List<SceneChestData> datas)
    {
        scenesDataList = datas;
    }

    public SceneChestData GetSceneDataFromName(string sceneName)
    {
        return scenesDataList.FirstOrDefault(sceneData => sceneData.sceneName.Equals(sceneName));
    }
}

[System.Serializable]
public struct SceneChestData
{
    public string sceneName;
    public List<ChestData> datas;
}