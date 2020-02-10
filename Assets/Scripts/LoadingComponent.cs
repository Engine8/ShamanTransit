using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingComponent : MonoBehaviour
{
    public Animator LoadingAnimator;
    public float ExitAnimationTime = -1;

    public void StartLoadLevel(string levelName)
    {
        if (ExitAnimationTime >= 0)
            LoadingAnimator.SetTrigger("ExitAnimation");
        Loader.LoadScene(levelName, ExitAnimationTime);
    }
}
