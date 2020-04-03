using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationAttackFalseAct : MonoBehaviour
{
    private RectTransform imagClick;
    private RectTransform TextClick;
    private float minimum = 0.9f;
    private float maximum = 1.1f;
    static float t = 0.2f;
    void Start()
    {
        imagClick = transform.GetChild(1).GetComponent<RectTransform>();
        TextClick = transform.GetChild(2).GetComponent<RectTransform>();
        StartCoroutine("ScaleIncrease");
    }
    IEnumerator ScaleIncrease()
    {
        t = 0.2f;
        while (imagClick.localScale.x != minimum)
        {
           imagClick.localScale = new Vector3(Mathf.Lerp(maximum, minimum, t), Mathf.Lerp(maximum, minimum, t), 0);
           TextClick.localScale = new Vector3(Mathf.Lerp(maximum, minimum, t), Mathf.Lerp(maximum, minimum, t), 0);
         
           yield return new WaitForSecondsRealtime(0.01f);
           t += 0.1f;
        }
        StartCoroutine("ScaleDecrease");
    }
    IEnumerator ScaleDecrease() 
    {
        t = 0.2f;
        while (imagClick.localScale.x != maximum)
        {
            imagClick.localScale = new Vector3(Mathf.Lerp(minimum, maximum, t), Mathf.Lerp(minimum, maximum, t), 0);
            TextClick.localScale = new Vector3(Mathf.Lerp(minimum, maximum, t), Mathf.Lerp(minimum, maximum, t), 0);

            yield return new WaitForSecondsRealtime(0.01f);
            t += 0.05f;
        }
        StartCoroutine("ScaleIncrease");
    }
}
