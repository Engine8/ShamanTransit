using UnityEngine;
using System.Collections;
using UnityEngine.Events;

public class EnemyController : MonoBehaviour
{
    public AudioClip EnterSound;
    public AudioClip AttackSound;
    public AudioClip DieSound;
    public AudioClip DamageSound;
    public bool IsCameraShaking;
    public Cinemachine.CinemachineImpulseSource ImpulseSource;

    protected bool _isInAnimation = false;
    public float RunTime = 1f;
    public AnimationCurve RunCurve;
    protected Vector3 _startAnimPosition;
    protected Vector3 _targetAnimPosition;
    protected float _animDistance;

    protected PlayerController _targetCharacter;
    protected Enemy _controlledEnemy;

    //invokes then battle should be ended: when enemy dies or boss attack phase ends
    public UnityEvent OnBattleEnd;
    public virtual void ProcessEnemyDeath() { }

    public virtual int GetCount() { return 404; }
    public virtual void TakeDamage() { }
    public virtual void Attack() { }
    public virtual void AttackOnMiss() { }
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
    public virtual void RunAway() { }

    public virtual EnemyType GetEnemyType() { return EnemyType.Wolf; }

}
