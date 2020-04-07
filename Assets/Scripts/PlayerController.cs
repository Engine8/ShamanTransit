﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerController : Movable
{   
    private static PlayerController _instance;

    public delegate void FinalHelpMessage();
    public event FinalHelpMessage FinishHelp;

    public static PlayerController Instance
    {
        get
        {
            return _instance;
        }
    }


    private SpriteRenderer _spriteRenderer;
    private SoulKeeper _soulKeeper;
    public int SoulCount
    {
        get
        {
            return _soulKeeper.GetSoulCount();
        }
    }

    public UnityEvent OnLevelEnd;

    public Cinemachine.CinemachineVirtualCamera Camera;


    private float _startYPosition = -1f;
    private float _sumDeltaPositionOnY = 0f;
    private bool _isSwipe = false;

    private ParticleSystemRenderer _stepParticleRenderer;
    private TouchObject _secondChanceClickArea;
    public UnityEvent OnSecondChanceClick;

    private void Awake()
    {
        if (_instance != null)
        {
            Destroy(this);
            return;
        }

        _instance = this;
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _soulKeeper = transform.Find("SoulKeeper").GetComponent<SoulKeeper>();
        _secondChanceClickArea = GetComponent<TouchObject>();
        _secondChanceClickArea.IsActive = false;
    }

    // Start is called before the first frame update
    new void Start()
    {
        _stepParticleRenderer = StepSnow.gameObject.GetComponent<ParticleSystemRenderer>();
        //OnChangeLineEnd.AddListener(ChangeSortingLayer);
        #if UNITY_ANDROID
        OnAttackHit.AddListener(() => 
        {
            if (GameData.Instance.VibrationStatus)
                Handheld.Vibrate();
        });
        #endif
        GameController.Instance.OnGameModeChanged.AddListener(OnGameModeChanged);
        base.Start();
        if (HelpControl.helpControl != null)
            FinishHelp += HelpControl.helpControl.Move;
    }

    private void Update()
    {
       
        //PC controls
        if (Input.GetButtonDown("Up") && !_isLineSwapBlocked /*&& !GameController.Instance.IsAttackMode*/)
        {
            if (FinishHelp != null)
                FinishHelp.Invoke();

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
        else if (Input.GetButtonDown("Jump") && !_isLineSwapBlocked /*&& !GameController.Instance.IsAttackMode*/ && _jumpStatus == 0)
        {
            _jumpStatus = 1;
        }

        //mobile controls
        if (Input.touchCount > 0 && !_isLineSwapBlocked && !GameController.Instance.IsAttackMode)
        {

            Debug.Log("Смена слоя перед помощу");
            if (FinishHelp != null)
                FinishHelp.Invoke();

            Touch touch = Input.GetTouch(0);

            //Version 1 - deadzone with discrete swipes
            /*
            if (touch.phase == TouchPhase.Began)
            {
                _startYPosition = touch.position.y;
            }
            else if (touch.phase == TouchPhase.Ended && _startYPosition > -0.1f)
            {
                float deltaPositionY = touch.position.y - _startYPosition;
                if (deltaPositionY > Screen.height * 0.05)
                {
                    _targetLine -= 1;
                    if (_targetLine < 0)
                        _targetLine = 0;
                }
                else if (deltaPositionY < Screen.height * -0.05)
                {
                    _targetLine += 1;
                    if (_targetLine > 2)
                        _targetLine = 2;
                }
                else
                {
                    _jumpStatus = 1;
                }
                //reset status
                //-1 to ignore End touch phase after exit attack mode
                _startYPosition = -1f;
            }
            */

            //Version 2 - deadzone with continious swipes
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
                if (!_isSwipe)
                {
                    _jumpStatus = 1;
                }
                _isSwipe = false;
            }
        }
    }

    new protected void OnTriggerEnter2D(Collider2D other)
    {
        base.OnTriggerEnter2D(other);
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
            AccelerationModif = 0;
            Speed = 0;
            OnLevelEnd.Invoke();
        }
        else if (other.gameObject.CompareTag("Soul"))
        {
            AddSoul();
            other.gameObject.SetActive(false);
        }
}

    void OnGameModeChanged()
    {
        if (GameController.Instance.IsAttackMode)
        {
            _animator.SetBool("Battle", true);
            _isLineSwapBlocked = true;
        }
        else
        {
            _animator.SetBool("Battle", false);
            _isLineSwapBlocked = false;
        }
    }

    override public void ChangeSortingLayer()
    {
        if (_targetLine == 0)
        {
            _spriteRenderer.sortingLayerName = "Line1";
            _soulKeeper.SetSoulsSortingLayer("Line1");
            _stepParticleRenderer.sortingLayerName = "Line1";
        }
        else if (_targetLine == 1)
        {
            _spriteRenderer.sortingLayerName = "Line2";
            _soulKeeper.SetSoulsSortingLayer("Line2");
            _stepParticleRenderer.sortingLayerName = "Line1";
        }
        else
        {
            _spriteRenderer.sortingLayerName = "Line3";
            _soulKeeper.SetSoulsSortingLayer("Line3");
            _stepParticleRenderer.sortingLayerName = "Line1";
        }
    }

    public void AddSoul()
    {
        _soulKeeper.AddSoul();
    }

    public void DeleteSoul()
    {
        _soulKeeper.DeleteSoul();
    }

    public void PlayAnimAttack()
    {
        //StartCoroutine("AttackAnimation");
        _animator.Play("Player_Attack");
    }

    public override void OnDieAnimationEnd()
    {
        _secondChanceClickArea.IsActive = true;
        OnDie.Invoke();
    }

    private void OnSecondChanceAreaClick()
    {
        if (OnSecondChanceClick != null)
            OnSecondChanceClick.Invoke();
    }

}
