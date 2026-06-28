using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class AllDebrisData : IAllData<AllDebrisData>
{
    public List<DebrisData> currentDataList;
    public List<SceneDebrisData> scenesDataList;

    public Queue<DebrisData> data;
    public int counter;

    public AllDebrisData() 
    {
        currentDataList = new List<DebrisData>();
        data = new Queue<DebrisData>();
        scenesDataList = new();
        counter = 0;
    }

    public void SaveQueue(string sceneName)
    {
        currentDataList = data.ToList();

        SceneDebrisData newData = new SceneDebrisData()
        {
            sceneName = sceneName,
            datas = data.ToList()
        };
        
        int sceneIndex = scenesDataList.FindIndex(sceneData => sceneData.sceneName.Equals(sceneName));
        
        if (sceneIndex != -1)
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

    public void LoadQueue(List<DebrisData> list)
    {
        foreach (var item in list)
        {
            data.Enqueue(item);
        }
    }

    public void SetScenesDataOnLoad(List<SceneDebrisData> datas)
    {
        scenesDataList = datas;
    }

    public void CopyData(AllDebrisData allData)
    {
        currentDataList = allData.currentDataList;
        counter = allData.counter;
        scenesDataList = allData.scenesDataList;
        var sceneData = GetSceneDataFromName(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
        if (sceneData.datas != null)
        {
            LoadQueue(sceneData.datas);
        }
    }

    public void ClearAfterSave()
    {
        counter = 0;
        data.Clear();
    }
    
    public SceneDebrisData GetSceneDataFromName(string sceneName)
    {
        return scenesDataList.FirstOrDefault(sceneData => sceneData.sceneName.Equals(sceneName));
    }
}

[Serializable]
public struct SceneDebrisData
{
    public string sceneName;
    public List<DebrisData> datas;
}
