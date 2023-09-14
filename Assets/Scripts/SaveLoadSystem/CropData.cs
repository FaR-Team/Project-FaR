public class CropData : IPersistent
{
    public CropData(string name, string description, string version)
    {
        Name = name;
        Description = description;
        Version = version;
    }

    public string Name { get; set; }
    public string Description { get; set; }
    public string Version { get; set; }

}
