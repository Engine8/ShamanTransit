using UnityEngine;

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
}
