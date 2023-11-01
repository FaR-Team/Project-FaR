using System;
using System.Collections.Generic;
using System.Linq;

[Serializable]
public class AllDirtsData
{
    public List<DirtData> dataList;
    public int DirtCounter;

    public Queue<DirtData> data;

    public AllDirtsData(int dirtCounter)
    {
        dataList = new List<DirtData>();
        data = new Queue<DirtData>();
        DirtCounter = dirtCounter;
    }

    public AllDirtsData(List<DirtData> dataList, int dirtCounter)
    {
        this.dataList = dataList;
        DirtCounter = dirtCounter;
        data = new Queue<DirtData>();
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
