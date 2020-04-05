using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;
using UnityEngine.Events;
//
//Сводка:
//      Movable is the class which represent all movable objects in the game 
public class Movable : MonoBehaviour
{
    public int MaxHP;
    public int CurrentHP;
    protected bool _isDead;

    public int MaxHPBattle;
    public int CurrentHPBattle;

    public AudioClip OnHitSound;
    public UnityEvent OnHit;
    public UnityEvent OnDie;
    public UnityEvent OnAttackHit;
    public UnityEvent OnChangeLineEnd;

    protected Rigidbody2D _rb2d;
    protected Animator _animator;
    public AnimationCurve AccelerationCurve;
    protected float _speed;



    public float Speed
    {
        get { return _speed; }
        set { _speed = value; }
    }

    public float BasicMaxSpeed { get; private set;}
    public float MaxSpeed = 10f;
    public float BasicAccelerationModif { get; private set; } = 1;
    public float AccelerationModif = 2f;
    //debug variable;
    public float CurrentAcceleration;

    public float StartSpeed = 0f;

    //debug
    public float curSpeed; //shows current speed in editor
    //
    protected float _timeCounter = 0f;
    protected float _curveModif;

    //jump variables
    /*
     * Jump status values: 
     * 0 - not jump
     * 1 - jump
    */
    protected int _jumpStatus;
    public AnimationCurve JumpCurve;
    public float[] JumpOffsets;
    public float JumpTime = 1f;

    //variables for swapping lines
    public AnimationCurve SwapLineCurve;
    public Transform[] Lines;
    public float[] LineScales;
    public float LineSwapTime = 1f;
    [Range(0, 1)]
    public float PhysicsLayerChangeTime1 = 0.3f; //moment of change from current line layer to middle layer
    [Range(0, 1)]
    public float PhysicsLayerChangeTime2 = 0.7f; //moment of change from middle layer to target line layer
    protected int _curLine;
    protected int _targetLine;
    protected bool _isLineSwapBlocked = false;
    /*
     * Change line status values: 
     * 0 - on start line
     * 1 - between start and target line
     * 2 - on target line
    */
    private int _changeLineStatus = 0;

    public AudioClip StepSound;
    public ParticleSystem StepSnow;

    // Start is called before the first frame update
    protected void Start()
    {
        _rb2d = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
        _speed = StartSpeed;
        _jumpStatus = 0;

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

        OnHit.AddListener(OnHitSoundPlay);

        BasicMaxSpeed = MaxSpeed;

  
    }

    // Update is called once per frame
    protected void FixedUpdate()
    {
        if (!_isDead && !GameController.Instance.IsGameEnded)
        {
            //calculate acceleration on earth (x coordinate)
            if (_jumpStatus == 0)
            {
                if (_speed <= MaxSpeed)
                {
                    float accelerationValue = AccelerationCurve.Evaluate(_speed / MaxSpeed) * AccelerationModif *  Time.fixedDeltaTime;
                    CurrentAcceleration = accelerationValue; //debug
                    _speed += accelerationValue;
                    curSpeed = _speed; //debug
                    if (_speed > MaxSpeed)
                        _speed = MaxSpeed;
                }
            }
            float dX = Vector2.right.x * _speed * Time.fixedDeltaTime;

            //delta y calculations on earth
            float dY = Lines[_curLine].position.y;
            bool isLineChangeEnded = false;
            bool isJumpEnded = false;
            _timeCounter += Time.fixedDeltaTime;
            if (_curLine != _targetLine && _jumpStatus == 0)
            {
                float newXYScale;
                isLineChangeEnded = CalculateSwitchLine(out dY, out newXYScale);
                transform.localScale = new Vector3(newXYScale, newXYScale, 1);
                DefineSwapLinePhysicsLayer();
            }
            else if (_jumpStatus == 1)
            {
                isJumpEnded = CalculateJump(out dY);
            }

            _rb2d.MovePosition(new Vector2(_rb2d.position.x + dX, dY));

            if (isLineChangeEnded)
            {
                _curLine = _targetLine;
                _isLineSwapBlocked = false;
                OnChangeLineEnd.Invoke();
            }
            else if (isJumpEnded)
            {
                _isLineSwapBlocked = false;
                _jumpStatus = 0;
            }
            if (!_isLineSwapBlocked)
            {
                _timeCounter = 0f;
                _curveModif = 0f;
            }
        }
    }
    
    private bool CalculateSwitchLine(out float dY, out float newXYScale)
    {
        bool isLineChangeEnded = false;
        _isLineSwapBlocked = true; //block all input
        if (_timeCounter > LineSwapTime)
        {
            _timeCounter = LineSwapTime;
            isLineChangeEnded = true;
        }
        //transofrm time-based value _timeCounter in abstract swap status value _curveModif
        //_curveModif varies from 0 to 1, where 0 - object on current line, 1 - object on target line
        _curveModif = SwapLineCurve.Evaluate(_timeCounter / LineSwapTime);
        dY = Mathf.Lerp(Lines[_curLine].position.y, Lines[_targetLine].position.y, _curveModif);
        newXYScale = Mathf.Lerp(LineScales[_curLine], LineScales[_targetLine], _curveModif);
        return isLineChangeEnded;
    }

    private void DefineSwapLinePhysicsLayer()
    {
        //based on swap status value define physics layer
        //enter in "middle" layer (1-2 or 2-3)
        if (_curveModif > PhysicsLayerChangeTime1 && _curveModif < PhysicsLayerChangeTime2 && _changeLineStatus == 0)
        {
            _changeLineStatus = 1;

            //when object moves "up", it should hide behind obstacles fast
            if (_targetLine < _curLine)
            {
                ChangeSortingLayer();
                gameObject.layer -= 1;
            }
            else if (_targetLine > _curLine)
            {
                gameObject.layer += 1;
            }
            //Debug.Log($"New layer: {gameObject.layer}");
        }
        //enter in target line
        else if (_curveModif > PhysicsLayerChangeTime2 && _changeLineStatus == 1)
        {
            _changeLineStatus = 2;
            if (_targetLine < _curLine)
            {
                gameObject.layer -= 1;
            }
            //when object moves "down", it should get out from behind obstacles on target line in last part of swap
            else if (_targetLine > _curLine)
            {
                ChangeSortingLayer();
                gameObject.layer += 1;
            }
            //Debug.Log($"New layer: {gameObject.layer}");
            _changeLineStatus = 0;
        }
    }

    private bool CalculateJump(out float dY)
    {
        bool isJumpEnded = false;
        _isLineSwapBlocked = true;
        if (_timeCounter > JumpTime)
        {
            _timeCounter = JumpTime;
            isJumpEnded = true;
        }
        //transofrm time-based value _timeCounter in abstract value _curveModif
        //_curveModif define the Y position between current line level and JumpOffset (should be set in editor)
        _curveModif = JumpCurve.Evaluate(_timeCounter / JumpTime);
        dY = Mathf.Lerp(Lines[_curLine].position.y, JumpOffsets[_curLine], _curveModif);
        return isJumpEnded;
    }

    protected void OnTriggerEnter2D(Collider2D other)
    {
        Obstacle obstacle = other.gameObject.GetComponent<Obstacle>();
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
                    DieStart();
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
            DieStart();
        }
    }

    public bool GetDead()
    {
        return _isDead;
    }

    public void OnHitSoundPlay()
    {
        if (OnHitSound != null)
            SoundManager.Instance.PlaySoundClip(OnHitSound, true);
    }

    virtual public void ChangeSortingLayer()
    {

    }

    public void SetMaxSpeed(float value)
    {
        if (_speed > value)
        {
            _speed = value;
        }
        MaxSpeed = value;
    }

    public void SetAccelerationModif(float value)
    {
        if (value > 0)
        {
            AccelerationModif = value;
        }
    }

    public void OnStep(int soundPlayStatus)
    {
        if (soundPlayStatus == 1)
            SoundManager.Instance.PlaySoundClip(StepSound, true);
        StepSnow.Play();
    }

    public void DieStart()
    {
        _isDead = true;

        _animator.SetBool("IsDead", true);

        OnDie.Invoke();
    }
}
