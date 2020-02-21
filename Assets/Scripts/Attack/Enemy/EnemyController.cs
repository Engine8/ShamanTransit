using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public virtual int GetCount() { return 404; }
    public virtual void TakeDamage() { }
    public virtual void Attack() { }
    public virtual bool GetActiv() { return false; }
}
