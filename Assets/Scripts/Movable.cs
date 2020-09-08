using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;
using UnityEngine.Events;
//
//Сводка:
//      Character is the class which represent all movable objects in the game 
public class Character : MonoBehaviour
{
    //general variables
    protected Rigidbody2D _rb2d;
    protected Animator _animator;

    //health variables
    [SerializeField]
    protected int _maxHP;
    public int MaxHP{ get; }
    [SerializeField]
    [Tooltip("Debug, changes are restricted")]
    protected int _currentHP;
    public int CurrentHP { get; }
    protected bool _isDead;

    //movement variables
    [SerializeField]
    protected AnimationCurve _accelerationCurve;
    [SerializeField]
    protected AnimationCurve _invertedAccelerationCurve;

    [SerializeField]
    [Tooltip("Time it takes for an object to accelerate from its initial speed to maximum speed")]
    protected float _accelerationTime;
    protected float _accelerationTimer = 0f;

    [SerializeField]
    protected float _startSpeed = 0f;
    public float StartSpeed
    {
        get { return _startSpeed; }
    }

    [SerializeField]
    [Tooltip("Debug, changes are restricted")]
    protected float _speed;
    public float Speed
    {
        get { return _speed; }
        set { _speed = value; }
    }

    [SerializeField]
    protected float _basicMaxSpeed;
    public float BasicMaxSpeed
    {
        get { return _basicMaxSpeed;  }
    }

    [SerializeField]
    [Tooltip("Debug, changes are restricted")]
    protected float _currentMaxSpeed;
    public float CurrentMaxSpeed
    {
        get { return _currentMaxSpeed; }
    }

    public AudioClip OnHitSound;
    public UnityEvent OnHit;
    public UnityEvent OnDieStart;
    public UnityEvent OnDieEnd;
    public UnityEvent OnChangeLineEnd;

    //variables for swapping lines
    public AnimationCurve SwapLineCurve;
    public Transform[] Lines;
    public float[] LineScales;

    [SerializeField]
    protected float _lineSwapTime = 1f;
    protected float _lineSwapCounter = 0f;
    protected float _lineSwapCurveModif;

    [SerializeField]
    [Range(0, 1)]
    [Tooltip("Moment of change from current line layer to middle layer (in percents of LineSwapTime)")]
    protected float PhysicsLayerChangeTimeToMiddle = 0.3f; 
    [SerializeField]
    [Range(0, 1)]
    [Tooltip("Moment of change from middle layer to target line (in percents of LineSwapTime)")]
    protected float PhysicsLayerChangeTimeToTarget = 0.7f;

    protected int _curLine;
    protected int _targetLine;
    protected bool _isLineSwapBlocked = false;
    /*
     * Swap line status values: 
     * 0 - on start line
     * 1 - between start and target line
     * 2 - on target line
    */
    private int _swapLineStatus = 0;


    // Start is called before the first frame update
    protected void Start()
    {
        _rb2d = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
        _speed = _startSpeed;
        _currentMaxSpeed = _basicMaxSpeed;

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


        //invert acceleration curve
        _invertedAccelerationCurve = new AnimationCurve();

        float totalTime = _accelerationCurve.keys[_accelerationCurve.length - 1].time;
        float sampleX = 0; //The "sample-point"
        float deltaX = 0.01f; //The "sample-delta"
        float lastY = _accelerationCurve.Evaluate(sampleX);
        while (sampleX < totalTime)
        {
            float y = _accelerationCurve.Evaluate(sampleX); //The "value"
            float deltaY = y - lastY; //The "value-delta"
            float tangent = deltaX / deltaY;
            Keyframe invertedKey = new Keyframe(y, sampleX, tangent, tangent);
            _invertedAccelerationCurve.AddKey(invertedKey);

            sampleX += deltaX;
            lastY = y;
        }
        for (int i = 0; i < _invertedAccelerationCurve.length; i++)
        {
            _invertedAccelerationCurve.SmoothTangents(i, 0.1f);
        }
    }

    protected void FixedUpdate()
    {
        if (!_isDead && !GameController.Instance.IsGameEnded)
        {
            //update speed
            _accelerationTimer = Mathf.Clamp(_accelerationTimer + Time.fixedDeltaTime, 0, _accelerationTime);
            _speed = _accelerationCurve.Evaluate(_accelerationTimer / _accelerationTime) * _currentMaxSpeed;
            //calculate dX
            float dX = Vector2.right.x * _speed * Time.fixedDeltaTime;

            //calculate dY (switch line)
            float dY = Lines[_curLine].position.y;
            bool isLineChangeEnded = false;
            if (_curLine != _targetLine)
            {
                _lineSwapCounter += Time.fixedDeltaTime;
                float newXYScale;
                isLineChangeEnded = CalculateSwitchLine(out dY, out newXYScale);
                transform.localScale = new Vector3(newXYScale, newXYScale, 1);
                DefineSwapLinePhysicsLayer();
            }

            _rb2d.MovePosition(new Vector2(_rb2d.position.x + dX, dY));

            if (isLineChangeEnded)
            {
                _curLine = _targetLine;
                _lineSwapCounter = 0f;
                _lineSwapCurveModif = 0f;
                OnChangeLineEnd.Invoke();
                _isLineSwapBlocked = false;
            }
        }
    }

    virtual protected void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Obstacle"))
        {
            Obstacle obstacle = collision.gameObject.GetComponent<Obstacle>();
            _speed -= obstacle.SpeedReduce;
            if (_speed < 0)
                _speed = 0;
            _accelerationTimer = _invertedAccelerationCurve.Evaluate(_speed);
            OnHit?.Invoke();
        }
    }

    private bool CalculateSwitchLine(out float dY, out float newXYScale)
    {
        bool isLineChangeEnded = false;
        _isLineSwapBlocked = true; //block all input
        if (_lineSwapCounter > _lineSwapTime)
        {
            _lineSwapCounter = _lineSwapTime;
            isLineChangeEnded = true;
        }
        //transofrm time-based value _timeCounter in abstract swap status value _curveModif
        //_curveModif varies from 0 to 1, where 0 - object on current line, 1 - object on target line
        _lineSwapCurveModif = SwapLineCurve.Evaluate(_lineSwapCounter / _lineSwapTime);
        dY = Mathf.Lerp(Lines[_curLine].position.y, Lines[_targetLine].position.y, _lineSwapCurveModif);
        newXYScale = Mathf.Lerp(LineScales[_curLine], LineScales[_targetLine], _lineSwapCurveModif);
        return isLineChangeEnded;
    }

    private void DefineSwapLinePhysicsLayer()
    {
        //based on swap status value define physics layer
        //enter in "middle" layer (1-2 or 2-3)
        if (_lineSwapCurveModif > PhysicsLayerChangeTimeToMiddle && _lineSwapCurveModif < PhysicsLayerChangeTimeToTarget && _swapLineStatus == 0)
        {
            _swapLineStatus = 1;

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
        }
        //enter in target line
        else if (_lineSwapCurveModif > PhysicsLayerChangeTimeToTarget && _swapLineStatus == 1)
        {
            _swapLineStatus = 2;
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
            _swapLineStatus = 0;
        }
    }

    public bool GetDead()
    {
        return _isDead;
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
        _currentMaxSpeed = value;
    }

    public virtual void DieStart()
    {
        _isDead = true;
        OnDieStart.Invoke();
    }

    public virtual void OnDieAnimationEnd()
    {
        OnDieEnd.Invoke();
    }

    /*
    public IEnumerator MoveToMiddleLine()
    {
        //wait until jump ends
        while (_jumpStatus == 1)
            yield return null;

        //character moves on line (after jump character CAN change line in any case)
        if (!_isLineSwapBlocked)
        {
            _isLineSwapBlocked = true;
            if (_curLine == 1)
                yield break;

            _targetLine = 1;
        }
        //character moves between lines
        else if (_isLineSwapBlocked && _jumpStatus == 0)
        {
            //if character moves to middle line, nothing needs to be done
            if (_targetLine == 1)
                yield break;

            //else character moves from middle line to line with _targetLine index
            //inverse movement
            _lineSwapCounter = LineSwapTime - _lineSwapCounter;
            _curLine = _targetLine;
            _targetLine = 1;
        }
    }
    */
}
