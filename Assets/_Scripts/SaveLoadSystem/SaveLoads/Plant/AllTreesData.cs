using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;
using Utils;

[Serializable]
public class AllTreesData : IAllData<AllTreesData>
{
    public List<TreeBushData> currentDataList;
    public List<ScenePlantData> scenesDataList;

    public Queue<TreeBushData> data;
    public int counter;

    public AllTreesData() 
    {
        currentDataList = new List<TreeBushData>();
        data = new Queue<TreeBushData>();
        scenesDataList = new();
        counter = 0;
    }

    public void SaveQueue(string sceneName)
    {
        currentDataList = data.ToList();

        ScenePlantData newData = new ScenePlantData()
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

    public void LoadQueue(List<TreeBushData> list)
    {
        foreach (var item in list)
        {
            data.Enqueue(item);
        }
    }
    

    public void SetScenesDataOnLoad(List<ScenePlantData> datas)
    {
        scenesDataList = datas;
    }

    public void CopyData(AllTreesData allData)
    {
        currentDataList = allData.currentDataList;
        counter = allData.counter;
        scenesDataList = allData.scenesDataList;
        LoadQueue(GetSceneDataFromName(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name).datas);
    }

    public void ClearAfterSave()
    {
        counter = 0;
        data.Clear();
    }
    
    public ScenePlantData GetSceneDataFromName(string sceneName)
    {
        return scenesDataList.FirstOrDefault(sceneData => sceneData.sceneName.Equals(sceneName));
    }
}

[Serializable]
public struct ScenePlantData
{
    public string sceneName;
    public List<TreeBushData> datas;
}
