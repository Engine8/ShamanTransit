using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class AnimationAttackTrueAct : MonoBehaviour
{
    private Image  _image;

    void Start()
    {
        _image = transform.GetChild(0).GetComponent<Image>();
        StartCoroutine(ConcealmentImage(_image));
    }

    IEnumerator ConcealmentImage(Image _alphaIm)
    {
        yield return new WaitForSecondsRealtime(2f);
        while (_alphaIm.color.a > 0)
        {
            yield return new WaitForSecondsRealtime(0.01f);
            _alphaIm.color = new Vector4(_alphaIm.color.r, _alphaIm.color.g, _alphaIm.color.b, _alphaIm.color.a - 0.05f);
        }
        gameObject.SetActive(false);
    }
}
