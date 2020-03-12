using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerController : Movable
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
    private SoulKeeper soulKeeper;
    public int SoulCount
    {
        get
        {
            return soulKeeper.GetSoulCount();
        }
    }

    public UnityEvent OnLevelEnd;

    public Cinemachine.CinemachineVirtualCamera Camera;


    private float _startYPosition = -1f;
    //private float _endYposition = 0f;
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
        soulKeeper = transform.Find("SoulKeeper").GetComponent<SoulKeeper>();
    }

    // Start is called before the first frame update
    new void Start()
    {
        //OnChangeLineEnd.AddListener(ChangeSortingLayer);
//#if UNITY_ANDROID
        OnAttackHit.AddListener(() => 
        {
            if (GameData.Instance.VibrationStatus)
                Handheld.Vibrate();
        });
//#endif
        //GameController.Instance.OnGameModeChanged.AddListener(OnGameModeChanged);
        base.Start();
    }

    private void Update()
    {
        //PC controls
        if (Input.GetButtonDown("Up") && !_isLineSwapBlocked && !GameController.Instance.IsAttackMode)
        {
            _targetLine -= 1;
            if (_targetLine < 0)
                _targetLine = 0;
        }
        else if (Input.GetButtonDown("Down") && !_isLineSwapBlocked && !GameController.Instance.IsAttackMode)
        {
            _targetLine += 1;
            if (_targetLine > 2)
                _targetLine = 2;
        }
        else if (Input.GetButtonDown("Jump") && !_isLineSwapBlocked && !GameController.Instance.IsAttackMode && _jumpStatus == 0)
        {
            _jumpStatus = 1;
        }

        //mobile controls
        if (Input.touchCount > 0 && !_isLineSwapBlocked && !GameController.Instance.IsAttackMode)
        {
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
            if (GameData.Instance.VibrationStatus)
                Handheld.Vibrate();
        }
        else if (other.gameObject.CompareTag("TriggerEnd"))
        {
            Acceleration = 0;
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
            _isLineSwapBlocked = true;
        else
            _isLineSwapBlocked = false;
    }

    override public void ChangeSortingLayer()
    {
        if (_targetLine == 0)
        {
            _spriteRenderer.sortingLayerName = "Line1";
            soulKeeper.SetSoulsSortingLayer("Line1");
        }
        else if (_targetLine == 1)
        {
            _spriteRenderer.sortingLayerName = "Line2";
            soulKeeper.SetSoulsSortingLayer("Line2");
        }
        else
        {
            _spriteRenderer.sortingLayerName = "Line3";
            soulKeeper.SetSoulsSortingLayer("Line3");
        }
    }

    public void AddSoul()
    {
        soulKeeper.AddSoul();
    }

    public void DeleteSoul()
    {
        soulKeeper.DeleteSoul();
    }
}
