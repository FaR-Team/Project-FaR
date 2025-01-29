using FaRUtils.Systems.DateTime;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utils;

public class CambiarEscena : MonoBehaviour, IInteractable
{
    [SerializeField] private GameObject _prompt;
    public GameObject InteractionPrompt => _prompt;
    
    public int targetSceneIndex;

    public void Interact(Interactor interactor, out bool interactSuccessful)
    {
        if (LoadingManager.LoadNewScene(targetSceneIndex))
        {
            interactSuccessful = true;
        }
        else
        {
            interactSuccessful = false;
        }
    }
    public void InteractOut()
    {

    }

    public void EndInteraction()
    {

    }
}