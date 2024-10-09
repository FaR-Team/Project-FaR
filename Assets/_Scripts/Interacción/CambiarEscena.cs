using FaRUtils.Systems.DateTime;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utils;

public class CambiarEscena : MonoBehaviour, IInteractable
{
    [SerializeField] private GameObject _prompt;

    public GameObject InteractionPrompt => _prompt;

    public GameObject LoadingScreenCanvas;
    public int buildIndex;
    public GameObject reloj;

    public void Interact(Interactor interactor, out bool interactSuccessful)
    {
        this.Log("Cambiando a escena " + buildIndex);
        LoadScene(buildIndex);
        interactSuccessful = true;
    }

    public void InteractOut()
    {
        this.Log(null);
    }

    public void LoadScene(int sceneID)
    {
        if (sceneID == 2) TimeManager.Instance.AdvanceTime(3);
        StartCoroutine(LoadSceneAsync(sceneID));
    }


    IEnumerator LoadSceneAsync(int sceneID)
    {
        //LoadingScreenCanvas.SetActive(true);

        SaveLoadHandlerSystem.Invoke(true);
        yield return new WaitForSeconds(0.25f); // TODO: FOR TESTING, change to WaitUntil all saving is complete
        
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneID);
        operation.allowSceneActivation = false;
        while (!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress / 0.9f);

            yield return null;
            if (progress == 1)
            {
                operation.allowSceneActivation = true;
            }
            yield return null;
        }
    }

    public void EndInteraction()
    {
        this.Log("Cambió la escena a " + buildIndex);
    }
}