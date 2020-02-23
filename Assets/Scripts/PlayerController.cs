using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : Movable
{   
    private SpriteRenderer _spriteRenderer;

   
    //Properties for changing lines
    [Range(0, 1)]
    public float PhysicsLayerChangeTime1 = 0.3f; //moment of change from current line layer to middle layer
    [Range(0, 1)]
    public float PhysicsLayerChangeTime2 = 0.7f; //moment of change from middle layer to target line layer

    private int ChangeLineStatus = 0;

    //Camera scales
    public float[] CameraLineScales;
    public Cinemachine.CinemachineVirtualCamera Camera;

    // Start is called before the first frame update
    new void Start()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        OnChangeLineEnd.AddListener(ChangeSortingLayer);
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
        if (_curveModif > PhysicsLayerChangeTime1 && _curveModif < PhysicsLayerChangeTime2 && ChangeLineStatus == 0)
        {
            ChangeLineStatus = 1;

            if (_targetLine < _curLine)
            {
                gameObject.layer -= 1;
            }

            if (_targetLine > _curLine)
            {
                ChangeSortingLayer();
                gameObject.layer += 1;
            }
        }
        else if (_curveModif > PhysicsLayerChangeTime2 && ChangeLineStatus == 1)
        {
            ChangeLineStatus = 2;
            if (_targetLine < _curLine)
            {
                ChangeSortingLayer();
                gameObject.layer -= 1;
            }
            if (_targetLine > _curLine)
            {
                gameObject.layer += 1;
            }
        }
        if (_curLine == _targetLine)
            ChangeLineStatus = 0;


    }

    new private void FixedUpdate()
    {
        bool pastIsLineSwapBlocked = _isLineSwapBlocked;
        base.FixedUpdate();
        /*
        if (_isLineSwapBlocked || pastIsLineSwapBlocked)
        {
            float newCameraScale = Mathf.Lerp(CameraLineScales[_curLine], CameraLineScales[_targetLine], _curveModif);
            Camera.m_Lens.OrthographicSize = newCameraScale;
        }
        */
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
