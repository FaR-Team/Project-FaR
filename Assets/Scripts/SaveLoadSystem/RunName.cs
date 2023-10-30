using UnityEngine;

public class RunName : MonoBehaviour
{
    public static RunName instance;
    public string currentRunName { get; private set; }

    private void Awake()
    {
        instance= this;
        currentRunName = "Run Test";
    }
}
