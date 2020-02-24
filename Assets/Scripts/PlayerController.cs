using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerController : Movable
{   
    private SpriteRenderer _spriteRenderer;

   
    //Properties for changing lines
    [Range(0, 1)]
    public float PhysicsLayerChangeTime1 = 0.3f; //moment of change from current line layer to middle layer
    [Range(0, 1)]
    public float PhysicsLayerChangeTime2 = 0.7f; //moment of change from middle layer to target line layer

    private int _changeLineStatus = 0;

    public UnityEvent OnLevelEnd;

    public int SoulCount = 0;

    //Camera scales
    public float[] CameraLineScales;
    public Cinemachine.CinemachineVirtualCamera Camera;

    // Start is called before the first frame update
    new void Start()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        OnChangeLineEnd.AddListener(ChangeSortingLayer);
        GameController.Instance.OnGameModeChanged.AddListener(OnGameModeChanged);
        base.Start();
    }

    private void Update()
    {
        //change line
        if (Input.GetButtonDown("Up") && !_isLineSwapBlocked)
        {
            //gameObject.layer -= 1;
            _targetLine -= 1;
            //if (gameObject.layer < 8)
            //    gameObject.layer = 8;
            if (_targetLine < 0)
                _targetLine = 0;
        }
        else if (Input.GetButtonDown("Down") && !_isLineSwapBlocked)
        {
            //gameObject.layer += 1;
            _targetLine += 1;
            //if (gameObject.layer > 10)
            //    gameObject.layer = 10;
            if (_targetLine > 2)
                _targetLine = 2;
        }

        //"enter" in middle layer
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
        }
        if (_curLine == _targetLine)
            _changeLineStatus = 0;


    }

    new protected void OnTriggerEnter2D(Collider2D other)
    {
        base.OnTriggerEnter2D(other);
        if (other.gameObject.CompareTag("Obstacle"))
        {
            SoulCount -= 1;
            if (SoulCount < 0)
                SoulCount = 0;

        }
        if (other.gameObject.CompareTag("TriggerEnd"))
        {
            Acceleration = 0;
            Speed = 0;
            OnLevelEnd.Invoke();
        }
    }

    void OnGameModeChanged()
    {
        if (GameController.Instance.IsAttackMode)
            _isLineSwapBlocked = true;
        else
            _isLineSwapBlocked = false;
    }

    private void ChangeSortingLayer()
    {
        if (_targetLine > _curLine)
        {
            if (_targetLine == 1)
            {
                _spriteRenderer.sortingLayerName = "Line2";
            }
            else
            {
                _spriteRenderer.sortingLayerName = "Line3";
            }
        }
        else
        {
            if (_targetLine == 0)
            {
                _spriteRenderer.sortingLayerName = "Line1";
            }
            else if (_targetLine == 1)
            {
                _spriteRenderer.sortingLayerName = "Line2";
            }
            else
            {
                _spriteRenderer.sortingLayerName = "Line3";
            }
        }
    }
}
