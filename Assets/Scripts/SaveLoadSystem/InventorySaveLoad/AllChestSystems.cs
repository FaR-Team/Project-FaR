using System.Collections.Generic;

public class AllChestSystems
{
    public int dataCounter;
    public List<ChestData> data;

    public AllChestSystems(int dataCounter)
    {
        this.dataCounter = dataCounter;
        data = new List<ChestData>();
    }
    public AllChestSystems()
    {
        dataCounter = 0;
        data = new List<ChestData>();
    }

    public AllChestSystems(List<ChestData> data, int dataCounter)
    {
        this.dataCounter = dataCounter;
        this.data = data;
    }
}