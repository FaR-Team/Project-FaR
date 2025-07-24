using FaRUtils.Systems.DateTime;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utils;

public class CambiarEscena : MonoBehaviour, IInteractable
{
    [SerializeField] private InteractionPromptUI _prompt;
    public InteractionPromptUI InteractionPrompt => _prompt;
    public Transform InteractionTarget => transform;
    
    public int targetSceneIndex;

    public void Interact(InteractorBase interactor, out bool interactSuccessful)
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
        if (_prompt != null)
        {
            _prompt.Close();
        }
    }
}