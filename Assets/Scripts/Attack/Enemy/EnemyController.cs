using UnityEngine;
using System.Collections;

public class EnemyController : MonoBehaviour
{
    public AudioClip EnterSound;
    public AudioClip AttackSound;
    public AudioClip DieSound;
    public bool IsCameraShaking;

    protected bool _isInAnimation = false;
    public float RunTime = 1f;
    public AnimationCurve RunCurve;
    protected Vector3 _startAnimPosition;
    protected Vector3 _targetAnimPosition;
    protected float _animDistance;


    public virtual int GetCount() { return 404; }
    public virtual void TakeDamage() { }
    public virtual void Attack() { }
    public virtual bool GetActiv() { return false; }
    public virtual void StartPlayerDieAnimation() { }

    public IEnumerator AnimatePlayerDeath()
    {
        float curTime = 0;
        bool end = false;
        while (!end)
        {
            curTime += Time.deltaTime;
            if (curTime > RunTime)
            {
                curTime = RunTime;
                end = true;
            }

            float tVal = curTime / RunTime;
            //define x position
            float x = Mathf.Lerp(_startAnimPosition.x, _targetAnimPosition.x, RunCurve.Evaluate(tVal));
            gameObject.transform.position = new Vector3(x, _startAnimPosition.y, _startAnimPosition.z);
            yield return null;
        }
        SetEnemyStatic();
    }

    public virtual void SetEnemyStatic() { }
}
