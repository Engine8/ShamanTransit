using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoaderCallback : MonoBehaviour
{
    public Text LoadingPercents;

    private bool isFirstUpdate = true;
    // Update is called once per frame
    void Update()
    {
        if (isFirstUpdate)
        {
            isFirstUpdate = false;
            Loader.LoaderCallback();
        }

        LoadingPercents.text = Loader.GetLoadingProgress() * 100 + "%";

    }
}
