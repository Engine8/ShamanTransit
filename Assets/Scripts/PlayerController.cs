using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerController : Movable
{   
    private SpriteRenderer _spriteRenderer;
    private SoulKeeper soulKeeper;
   


    public UnityEvent OnLevelEnd;

    public int SoulCount
    {
        get
        {
            return soulKeeper.GetSoulCount();
        }
    }

    //Camera scales
    public float[] CameraLineScales;
    public Cinemachine.CinemachineVirtualCamera Camera;

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        soulKeeper = transform.Find("SoulKeeper").GetComponent<SoulKeeper>();

    }

    // Start is called before the first frame update
    new void Start()
    {

        OnChangeLineEnd.AddListener(ChangeSortingLayer);
        OnAttackHit.AddListener(() => { Handheld.Vibrate(); });
        //GameController.Instance.OnGameModeChanged.AddListener(OnGameModeChanged);
        base.Start();
    }

    private void Update()
    {
        //change line
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

        if (Input.touchCount > 0 && !_isLineSwapBlocked && !GameController.Instance.IsAttackMode)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Moved)
            {
                // if swipe is vertical
                if (Mathf.Abs(touch.deltaPosition.y) > Mathf.Abs(touch.deltaPosition.x))
                {
                    if (touch.deltaPosition.y > 0)
                    {
                        _targetLine -= 1;
                        if (_targetLine < 0)
                            _targetLine = 0;
                    }
                    else
                    {
                        _targetLine += 1;
                        if (_targetLine > 2)
                            _targetLine = 2;
                    }
                }
            }
        }
    }

    new protected void OnTriggerEnter2D(Collider2D other)
    {
        base.OnTriggerEnter2D(other);
        if (other.gameObject.CompareTag("Obstacle"))
        {
            DeleteSoul();
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

    new private void ChangeSortingLayer()
    {
        if (_targetLine > _curLine)
        {
            if (_targetLine == 1)
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
        else
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
