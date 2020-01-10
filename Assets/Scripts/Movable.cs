using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
//
//Сводка:
//      Movable is the class which represent all movable objects in the game 
public class Movable : MonoBehaviour
{
    public float MaxSpeed = 10f;
    public float Acceleration = 2f;
    public float StartSpeed = 0f;

    protected float _speed;
    protected Rigidbody2D _rb2d;

    public UnityEvent OnHit;
    public UnityEvent OnDie;

    public float Speed
    {
        get { return _speed; }
        set { _speed = value; }
    }

    // Start is called before the first frame update
    protected void Start()
    {
        _rb2d = GetComponent<Rigidbody2D>();
        _speed = StartSpeed;
    }

    // Update is called once per frame
    protected void FixedUpdate()
    {
        if (_speed < MaxSpeed)
        {
            _speed += Acceleration * Time.deltaTime;
            if (_speed > MaxSpeed)
                _speed = MaxSpeed;
        }
        _rb2d.MovePosition(_rb2d.position + Vector2.right * _speed * Time.deltaTime);
    }

    protected void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Collision entered");
        if (other.gameObject.tag == "Obstacle")
        {
            Debug.Log("Collision with obstacle");
            Obstacle obstacle = other.gameObject.GetComponent<Obstacle>();
            if (OnHit != null)
            {
                OnHit.Invoke();
            }
            if (obstacle.Type == Obstacle.ObstacleType.Slower)
            {
                _speed -= obstacle.SpeedReduce;
                if (_speed < 0)
                {
                    _speed = 0;
                }
            }
            else if (obstacle.Type == Obstacle.ObstacleType.Deadly)
            {
                if (OnDie != null)
                {
                    OnDie.Invoke();
                }
            }
        }
    }
   
    public void OnHitLog()
    {
        Debug.Log("Main character hit");
    }

}
