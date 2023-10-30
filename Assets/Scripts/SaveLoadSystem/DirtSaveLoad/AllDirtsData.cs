using System;
using System.Collections.Generic;
using System.Linq;

[Serializable]
public class AllDirtsData
{
    public List<DirtSaveData> dataList;
    public int DirtCounter;

    public Queue<DirtSaveData> data;

    public AllDirtsData(int dirtCounter)
    {
        dataList = new List<DirtSaveData>();
        data = new Queue<DirtSaveData>();
        DirtCounter = dirtCounter;
    }

    public AllDirtsData(List<DirtSaveData> dataList, int dirtCounter)
    {
        this.dataList = dataList;
        DirtCounter = dirtCounter;
        data = new Queue<DirtSaveData>();
        LoadQueue();
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
}
