using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerController : Character
{   
    private static PlayerController _instance;

    public static PlayerController Instance
    {
        get
        {
            return _instance;
        }
    }

    private SpriteRenderer _spriteRenderer;
    /*
    private SoulKeeper _soulKeeper;
    public int SoulCount
    {
        get
        {
            return _soulKeeper.GetSoulCount();
        }
    }
    */
    public UnityEvent OnLevelEnd;

    public Cinemachine.CinemachineVirtualCamera Camera;

    private float _startYPosition = -1f;
    private float _sumDeltaPositionOnY = 0f;
    private bool _isSwipe = false;


    private void Awake()
    {
        if (_instance != null)
        {
            Destroy(this);
            return;
        }

        _instance = this;
        _spriteRenderer = GetComponent<SpriteRenderer>();
        //_soulKeeper = transform.Find("SoulKeeper").GetComponent<SoulKeeper>();
    }

    // Start is called before the first frame update
    new void Start()
    {/*
        #if UNITY_ANDROID
        OnAttackHit.AddListener(() => 
        {
            if (GameData.Instance.VibrationStatus)
                Handheld.Vibrate();
        });
        #endif
        */
        base.Start();
        //_animator.SetBool("IsIdle", false);
    }

    private void Update()
    {
        //PC controls
        if (Input.GetButtonDown("Up") && !_isLineSwapBlocked /*&& !GameController.Instance.IsAttackMode*/)
        {
            _targetLine -= 1;
            if (_targetLine < 0)
                _targetLine = 0;
        }
        else if (Input.GetButtonDown("Down") && !_isLineSwapBlocked /*&& !GameController.Instance.IsAttackMode*/)
        {
            _targetLine += 1;
            if (_targetLine > 2)
                _targetLine = 2;
        }
        /*
        else if (Input.GetButtonDown("Jump") && !_isLineSwapBlocked && !GameController.Instance.IsAttackMode && _jumpStatus == 0)
        {
            _jumpStatus = 1;
        }
        */
        //mobile controls
        if (Input.touchCount > 0 && !_isLineSwapBlocked)
        {
            Touch touch = Input.GetTouch(0);
            //deadzone with continious swipes
            if (touch.phase == TouchPhase.Moved)
            { 
                // if swipe is vertical
                if (Mathf.Abs(touch.deltaPosition.y) > Mathf.Abs(touch.deltaPosition.x))
                {
                    _sumDeltaPositionOnY += touch.deltaPosition.y;
                    if (_sumDeltaPositionOnY > Screen.height * 0.05)
                    {
                        _targetLine -= 1;
                        if (_targetLine < 0)
                            _targetLine = 0;
                        _sumDeltaPositionOnY = 0;
                        _isSwipe = true;
                    }
                    else if (_sumDeltaPositionOnY < Screen.height * -0.05)
                    {
                        _targetLine += 1;
                        if (_targetLine > 2)
                            _targetLine = 2;
                        _sumDeltaPositionOnY = 0;
                        _isSwipe = true;
                    }
                }
            }
            else if (touch.phase == TouchPhase.Ended)
            {
                _sumDeltaPositionOnY = 0;
                _isSwipe = false;
            }
        }
    }

    /*
    protected void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Obstacle"))
        {
            DeleteSoul();
            #if UNITY_ANDROID
                        if (GameData.Instance.VibrationStatus)
                            Handheld.Vibrate();
            #endif
        }
        else if (other.gameObject.CompareTag("TriggerEnd"))
        {
            Speed = 0;
            _animator.SetBool("IsIdle", true);
            OnLevelEnd.Invoke();
        }
        else if (other.gameObject.CompareTag("Soul"))
        {
            //AddSoul();
            other.gameObject.SetActive(false);
        }
    }
    */
    override public void ChangeSortingLayer()
    {
        if (_targetLine == 0)
        {
            _spriteRenderer.sortingLayerName = "Line1";
            //_soulKeeper.SetSoulsSortingLayer("Line1");
        }
        else if (_targetLine == 1)
        {
            _spriteRenderer.sortingLayerName = "Line2";
            //_soulKeeper.SetSoulsSortingLayer("Line2");
        }
        else
        {
            _spriteRenderer.sortingLayerName = "Line3";
            //_soulKeeper.SetSoulsSortingLayer("Line3");
        }
    }

    /*
    public void AddSoul()
    {
        _soulKeeper.AddSoul();
    }

    public void DeleteSoul()
    {
        _soulKeeper.DeleteSoul();
    }
    */
    public override void DieStart()
    {
        _isDead = true;

        //_animator.SetBool("IsDead", true);
        //_soulKeeper.ReleaseSouls();
        OnDieStart.Invoke();
    }

    public override void OnDieAnimationEnd()
    {
        OnDieEnd.Invoke();
    }
}
