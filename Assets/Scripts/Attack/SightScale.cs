using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SightScale : MonoBehaviour
{
    public Transform Arrow;
    public Transform HitArea;

    private bool _Victory;
    public float SpeedRotate;
    public void BafSpeed()
    {
        if (GameController.Instance.IsAttackMode)
        {
           
            SpeedRotate = (Mathf.Abs(SpeedRotate) + 1) * SpeedRotate / Mathf.Abs(SpeedRotate);

            if (Mathf.Abs(SpeedRotate) > 4)
                SpeedRotate = 2 * SpeedRotate / Mathf.Abs(SpeedRotate);
            if (Random.Range(0, 100) == 46)
                SpeedRotate = 1 * SpeedRotate / Mathf.Abs(SpeedRotate);
        }
    }
    public void Stop()
    {
         _Victory = true;
        GameController.Instance.SetGameMode(0);
        StartCoroutine("Weate");
    }

    IEnumerator Weate()
    {
        while (true) {
            yield return new WaitForSeconds(0.25f);
            if (Mathf.Abs(SpeedRotate) <= 0)
                break;
            SpeedRotate = (Mathf.Abs(SpeedRotate) - 0.5f) * SpeedRotate / Mathf.Abs(SpeedRotate);
        }
        _Victory = false;
        gameObject.SetActive(false);
    }
   
    void FixedUpdate()
    {
        if (Arrow.localEulerAngles.z >= 90f && SpeedRotate > 0)
        {
            SpeedRotate *= -1;
        }
        else if (Arrow.localEulerAngles.z <= 1f || Arrow.localEulerAngles.z >= 350f && SpeedRotate < 0)
        {
            SpeedRotate *= -1;
        }
        float rotationZ = Arrow.localEulerAngles.z + SpeedRotate;
        Arrow.localEulerAngles = new Vector3(0, 0, rotationZ);
    }
    public bool GetVictory()
    {
        return _Victory;
    }
}
