using UnityEngine;

public class DirtAnimation : MonoBehaviour
{
    private GameObject parent;

    void Awake()
    {
        parent = transform.parent.gameObject;        
    }
    
    public void PrepareToGetDown()
    {
        parent.GetComponent<Dirt>().GetDown();
    }

    public void PrepareToRaiseColliders()
    {
        parent.GetComponent<Dirt>().RaiseColliders();
    }
}