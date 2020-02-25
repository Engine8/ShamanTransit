using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;

public static class Loader
{

    private class LoadingMonobehaviour : MonoBehaviour { }
    private static Action onLoaderCallback;
    private static AsyncOperation loadingOperation;

    public static void LoadScene(int sceneIndex)
    {
        onLoaderCallback = () =>
        {
            GameObject loadingGameObject = new GameObject("LoadingGameObject");
            loadingGameObject.AddComponent<LoadingMonobehaviour>().StartCoroutine(LoadSceneAsync(sceneIndex));
        };

        SceneManager.LoadScene("LoadingScene");
    }

    private static IEnumerator LoadSceneAsync(int sceneIndex)
    {
        yield return new WaitForSeconds(1f);

        loadingOperation = SceneManager.LoadSceneAsync(sceneIndex);
        yield return new WaitForSeconds(1f);
        while (!loadingOperation.isDone)
        {
            yield return new WaitForSeconds(1f);
        }
    }

    public static void LoadScene(string sceneName, float exitAnimationTime)
    {
        //Loading with loading screen
        /*
        onLoaderCallback = () =>
        {
            GameObject loadingGameObject = new GameObject("LoadingGameObject");
            loadingGameObject.AddComponent<LoadingMonobehaviour>().StartCoroutine(LoadSceneAsync(sceneName));
        };
        
        SceneManager.LoadScene("LoadingScene");
        */

        //Loading without loading screen and with animation
        GameObject loadingGameObject = new GameObject("LoadingGameObject");
        loadingGameObject.AddComponent<LoadingMonobehaviour>().StartCoroutine(LoadSceneAsync(sceneName, exitAnimationTime));
    }

    private static IEnumerator LoadSceneAsync(string sceneName, float exitAnimationTime)
    {
        loadingOperation = SceneManager.LoadSceneAsync(sceneName);
        loadingOperation.allowSceneActivation = false;
        yield return new WaitForSeconds(exitAnimationTime);
        loadingOperation.allowSceneActivation = true;
        while (!loadingOperation.isDone)
        {
            yield return null;
        }
    }

    public static float GetLoadingProgress()
    {
        if (loadingOperation != null)
            //calculate only loading part of scene transition
            //return Mathf.Clamp01(loadingOperation.progress / 0.9f);
            //calculating loading and activation parts of scene transition
            return loadingOperation.progress;
        else
            return 1f;
    }

    public static void LoaderCallback()
    {
        if (onLoaderCallback != null)
        {
            onLoaderCallback.Invoke();
            onLoaderCallback = null;
        }
    }
}
