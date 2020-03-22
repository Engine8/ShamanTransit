using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public AudioClip EnterSound;
    public AudioClip AttackSound;
    public AudioClip DieSound;
    public bool IsCameraShaking;

    public virtual int GetCount() { return 404; }
    public virtual void TakeDamage() { }
    public virtual void Attack() { }
    public virtual bool GetActiv() { return false; }
}
