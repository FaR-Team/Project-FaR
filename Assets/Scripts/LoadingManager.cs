using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadingManager : MonoBehaviour 
{
    private static LoadingManager instance;
    private static bool loading;

    public static int LOADING_SCENE_INDEX = 3;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public static bool LoadNewScene(int sceneID)
    {
        if (loading) return false;
        instance.StartCoroutine(LoadScene(sceneID));
        return true;
    }
    
    public static IEnumerator LoadScene(int sceneID)
    {
        loading = true;
        SaveLoadHandlerSystem.Invoke(true);


        AsyncOperation loadingSceneOperation = SceneManager.LoadSceneAsync(LOADING_SCENE_INDEX);
        while (!loadingSceneOperation.isDone)
        {
            yield return null;
        }
        
        AsyncOperation targetSceneOperation = SceneManager.LoadSceneAsync(sceneID);
        targetSceneOperation.allowSceneActivation = false;

        while (!targetSceneOperation.isDone)
        {
            float progress = Mathf.Clamp01(targetSceneOperation.progress / 0.9f);

            if (progress >= 1f)
            {
                targetSceneOperation.allowSceneActivation = true;
            }
            yield return null;
        }
        
        loading = false;
    }
}
