using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;
using Utils;

[Serializable]
public class AllDirtsData : IAllData<AllDirtsData>
{
    public List<DirtData> currentDataList;
    public List<SceneDirtData> scenesDataList;

    public Queue<DirtData> data;
    public int counter;

    public AllDirtsData() 
    {
        currentDataList = new List<DirtData>();
        data = new Queue<DirtData>();
        scenesDataList = new();
        counter = 0;
    }

    public void SaveQueue(string sceneName)
    {
        currentDataList = data.ToList();

        SceneDirtData newData = new SceneDirtData()
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

    public void LoadQueue(List<DirtData> list)
    {
        foreach (var item in list)
        {
            data.Enqueue(item);
        }
    }
    

    public void SetScenesDataOnLoad(List<SceneDirtData> datas)
    {
        scenesDataList = datas;
    }

    public void CopyData(AllDirtsData allData)
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
    
    public SceneDirtData GetSceneDataFromName(string sceneName)
    {
        return scenesDataList.FirstOrDefault(sceneData => sceneData.sceneName.Equals(sceneName));
    }
}

[Serializable]
public struct SceneDirtData
{
    public string sceneName;
    public List<DirtData> datas;
}
