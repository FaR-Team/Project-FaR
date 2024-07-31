using FaRUtils.Systems.DateTime;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CambiarEscena : MonoBehaviour, IInteractable
{
    [SerializeField] private GameObject _prompt;

    public GameObject InteractionPrompt => _prompt;

    public GameObject LoadingScreenCanvas;
    public int buildIndex;
    public GameObject reloj;

    public void Interact(Interactor interactor, out bool interactSuccessful)
    {
        Debug.Log("Cambiando a escena a" + buildIndex);
        LoadScene(buildIndex);
        interactSuccessful = true;
    }

    public void InteractOut()
    {
        Debug.Log(null);
    }

    public void LoadScene(int sceneID)
    {
        TimeManager.Instance.AdvanceTime(3);
        StartCoroutine(LoadSceneAsync(sceneID));
        SaveLoadHandlerSystem.IsTemporary();
    }


    IEnumerator LoadSceneAsync(int sceneID)
    {
        //reloj.gameObject.SetActive(false);
        //LoadingScreenCanvas.SetActive(true);

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
        Debug.Log("Cambió la escena a" + buildIndex);
    }
}