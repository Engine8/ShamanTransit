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

    public int MaxHP;
    public int CurrentHP;
    protected bool _isDead;

    public int MaxHPBattle;
    public int CurrentHPBattle;
    //debug
    public float curSpeed;

    protected float _speed;
    protected Rigidbody2D _rb2d;

    public UnityEvent OnHit;
    public UnityEvent OnDie;
    public UnityEvent OnAttackHit;
    public UnityEvent OnChangeLineEnd;

    public AnimationCurve AccelerationCurve;

    public float Speed
    {
        get { return _speed; }
        set { _speed = value; }
    }

    //variables for swapping lines
    public Transform[] Lines;
    public float LineSwapTime = 1f;
    protected int _curLine;
    protected int _targetLine;
    //time lerp coefficient
    protected float _lerpModif = 0f;
    //curve lerp coefficient
    protected float _curveModif;
    protected bool _isLineSwapBlocked = false;
    public float[] LineScales;
    public AnimationCurve SwapLineCurve;

    [Range(0, 1)]
    public float PhysicsLayerChangeTime1 = 0.3f; //moment of change from current line layer to middle layer
    [Range(0, 1)]
    public float PhysicsLayerChangeTime2 = 0.7f; //moment of change from middle layer to target line layer

    private int _changeLineStatus = 0;

    // Start is called before the first frame update
    protected void Start()
    {
        _rb2d = GetComponent<Rigidbody2D>();
        _speed = StartSpeed;

        if (gameObject.layer == 8)
        {
            _curLine = 0;
        }
        else if (gameObject.layer == 10)
        {
            _curLine = 1;
        }
        else if (gameObject.layer == 12)
        {
            _curLine = 2;
        }
        _targetLine = _curLine;
    }

    // Update is called once per frame
    protected void FixedUpdate()
    {
        if (!GameController.Instance.IsGameEnded)
        {
            //delta x calculations
            if (_speed < MaxSpeed)
            {
                float accelerationValue = AccelerationCurve.Evaluate(_speed / MaxSpeed) * Time.fixedDeltaTime;
                Acceleration = accelerationValue; //debug
                _speed += accelerationValue;
                curSpeed = _speed; //debug
                if (_speed > MaxSpeed)
                    _speed = MaxSpeed;
            }
            float dX = Vector2.right.x * _speed * Time.fixedDeltaTime;

            //delta y calculations
            float dY = Lines[_curLine].position.y;
            bool isLineChangeEnded = false;
            if (_curLine != _targetLine)
            {
                _isLineSwapBlocked = true; //block all input
                _lerpModif += Time.fixedDeltaTime;
                if (_lerpModif > LineSwapTime)
                {
                    _lerpModif = LineSwapTime;
                    isLineChangeEnded = true;
                }
                //transofrm time-based value _lerpModif in abstract swap status value
                //value varies from 0 to 1, where 0 - object on current line, 1 - object on target line
                _curveModif = SwapLineCurve.Evaluate(_lerpModif / LineSwapTime);
                dY = Mathf.Lerp(Lines[_curLine].position.y, Lines[_targetLine].position.y, _curveModif);
                float newXYScale = Mathf.Lerp(LineScales[_curLine], LineScales[_targetLine], _curveModif);
                transform.localScale = new Vector3(newXYScale, newXYScale, 1);

                //based on swap status value define physics layer
                if (_curveModif > PhysicsLayerChangeTime1 && _curveModif < PhysicsLayerChangeTime2 && _changeLineStatus == 0)
                {
                    _changeLineStatus = 1;

                    if (_targetLine < _curLine)
                    {
                        gameObject.layer -= 1;
                    }

                    if (_targetLine > _curLine)
                    {
                        ChangeSortingLayer();
                        gameObject.layer += 1;
                    }
                    Debug.Log($"New layer: {gameObject.layer}");
                }
                else if (_curveModif > PhysicsLayerChangeTime2 && _changeLineStatus == 1)
                {
                    _changeLineStatus = 2;
                    if (_targetLine < _curLine)
                    {
                        ChangeSortingLayer();
                        gameObject.layer -= 1;
                    }
                    if (_targetLine > _curLine)
                    {
                        gameObject.layer += 1;
                    }
                    Debug.Log($"New layer: {gameObject.layer}");
                    _changeLineStatus = 0;
                }
                /*
                if (gameObject.layer < 8)
                    gameObject.layer = 8;
                else if (gameObject.layer > 12)
                    gameObject.layer = 12;
                    */

            }
            _rb2d.MovePosition(new Vector2(_rb2d.position.x + dX, dY));

            if (isLineChangeEnded)
            {
                _curLine = _targetLine;
                _isLineSwapBlocked = false;
                OnChangeLineEnd.Invoke();
            }
            if (!_isLineSwapBlocked)
            {
                _lerpModif = 0f;
                _curveModif = 0f;
            }
        }
    }

    protected void OnTriggerEnter2D(Collider2D other)
    {
        Obstacle obstacle = other.gameObject.GetComponent<Obstacle>();
        Debug.Log(other.name + " = " + (obstacle != null));
        if (obstacle != null)
        {
            Debug.Log("Collision with obstacle");

            CurrentHP -= obstacle.Damage;
            if (CurrentHP < 0)
            {
                CurrentHP = 0;
            }

            if (OnHit != null)
            {
                OnHit.Invoke();
            }

            if (CurrentHP == 0 || obstacle.Type == Obstacle.ObstacleType.Deadly)
            {
                if (OnDie != null)
                {
                    OnDie.Invoke();
                }
            }
            else if (obstacle.Type == Obstacle.ObstacleType.Slower)
            {
                _speed -= obstacle.SpeedReduce;
                if (_speed < 0)
                {
                    _speed = 0;
                }
            }
        }
    }

    public void TakeDamage(int damage)
    {
        Debug.Log("TakeDamage");
        CurrentHPBattle -= damage;
        OnAttackHit.Invoke();
        if (CurrentHPBattle <= 0 && !_isDead)
        {
            _isDead = true;
             OnDie.Invoke();
        }
    }

    public bool GetDead()
    {
        return _isDead;
    }

    public void OnHitLog()
    {
        Debug.Log("Main character hit");
    }

    public void ChangeSortingLayer()
    {

    }
}
