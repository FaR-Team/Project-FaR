using UnityEngine;

public class DirtSaver : MonoBehaviour
{
    public string Name { get; set; }
    public string Description { get; set; }
    public string Version { get; set; }

    string id;
    void SaveMyData()
    {
        var CropData = new CropData(Name, Description, Version);

        Saver.keyValuePairs.Add(id, CropData);
    }
}
