using UnityEngine;

public class InteractionPromptUI : MonoBehaviour
{
    [SerializeField] private GameObject _promptImage;
    
    public bool IsDisplayed { get; private set; }

    private void Start()
    {
        if (_promptImage != null)
            _promptImage.SetActive(false);
    }

    public void SetUp(Transform target)
    {
        if (_promptImage != null)
        {
            _promptImage.SetActive(true);
            IsDisplayed = true;
        }
    }

    public void Close()
    {
        if (_promptImage != null)
            _promptImage.SetActive(false);
        IsDisplayed = false;
    }
}
