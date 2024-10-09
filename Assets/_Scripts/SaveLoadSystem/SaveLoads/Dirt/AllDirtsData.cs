using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Utils;

[Serializable]
public class AllDirtsData : IAllData<AllDirtsData>
{
    public Dictionary<string, List<DirtData>> sceneDataDictionary;
    public List<DirtData> dataList;
    
    public Queue<DirtData> data;
    public int counter;

    public AllDirtsData() 
    {
        dataList = new List<DirtData>();
        data = new Queue<DirtData>();
        sceneDataDictionary = new();
        counter = 0;
    }

    public void SaveQueue(string sceneName)
    {
        dataList = data.ToList();

        sceneDataDictionary[sceneName] = dataList;
    }

    public void LoadQueue()
    {
        foreach (var item in dataList)
        {
            data.Enqueue(item);
        }
    }

    public void SetDictionaryOnLoad(Dictionary<string, List<DirtData>> dict)
    {
        sceneDataDictionary = dict;
    }

    public void CopyData(AllDirtsData allData)
    {
        dataList = allData.dataList;
        counter = allData.counter;
        sceneDataDictionary = allData.sceneDataDictionary;
        LoadQueue();
    }

    public void ClearAfterSave()
    {
        counter = 0;
        data.Clear();
    }
}
