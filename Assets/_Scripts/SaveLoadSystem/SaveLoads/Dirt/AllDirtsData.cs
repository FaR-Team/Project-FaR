using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class AllDirtsData : IAllData<AllDirtsData>
{
    public List<DirtData> dataList;
    
    public Queue<DirtData> data;
    public int counter;

    public AllDirtsData() 
    {
        dataList = new List<DirtData>();
        data = new Queue<DirtData>();
        counter = 0;
    }

    public void SaveQueue()
    {
        dataList = data.ToList();
    }

    public void LoadQueue()
    {
        foreach (var item in dataList)
        {
            data.Enqueue(item);
        }
    }

    public void CopyData(AllDirtsData allData)
    {
        dataList = allData.dataList;
        counter = allData.counter;
        LoadQueue();
    }

    public void ClearAfterSave()
    {
        counter = 0;
        data.Clear();
    }
}
