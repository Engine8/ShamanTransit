using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AnimationMovingAct : MonoBehaviour
{
    private Text text;
    private float minimum = 0F;
    private float maximum = 255f;
    private float t = 1;
    void Start()
    {
        text = transform.GetChild(0).GetComponent<Text>();
        StartCoroutine("DecreaseColorAlpha");
    }
    IEnumerator DecreaseColorAlpha()
    {
        
        while (text.color.a > 0)
        {
            yield return new WaitForSecondsRealtime(0.01f);
            text.color = new Vector4(0, 0, 0, ( t));
            t -= 0.05f;
        }
       StartCoroutine("IncreaseColorAlpha");
    }
    IEnumerator IncreaseColorAlpha()
    {

        while  (text.color.a < 1)
        {
            yield return new WaitForSecondsRealtime(0.01f);
            text.color = new Vector4(0, 0, 0, (t));
            t += 0.05f;
        }

        StartCoroutine("ScaleIncrease");
    }
}
