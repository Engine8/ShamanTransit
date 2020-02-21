using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : Movable
{    
    private SpriteRenderer _spriteRenderer;

    //Camera scales
    public float[] CameraLineScales;
    public Cinemachine.CinemachineVirtualCamera Camera;

    // Start is called before the first frame update
    new void Start()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();

        base.Start();
    }

    private void Update()
    {
        //change line
        if (Input.GetButtonDown("Up") && !_isLineSwapBlocked)
        {
            gameObject.layer -= 1;
            _targetLine -= 1;
            if (gameObject.layer < 8)
                gameObject.layer = 8;
            if (_targetLine < 0)
                _targetLine = 0;
        }
        else if (Input.GetButtonDown("Down") && !_isLineSwapBlocked)
        {
            gameObject.layer += 1;
            _targetLine += 1;
            if (gameObject.layer > 10)
                gameObject.layer = 10;
            if (_targetLine > 2)
                _targetLine = 2;
        }

        //define sorting layer
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
            if (_curLine == 0)
            {
                _spriteRenderer.sortingLayerName = "Line1";
            }
            else if (_curLine == 1)
            {
                _spriteRenderer.sortingLayerName = "Line2";
            }
            else
            {
                _spriteRenderer.sortingLayerName = "Line3";
            }
        }
    }

    new private void FixedUpdate()
    {
        bool pastIsLineSwapBlocked = _isLineSwapBlocked;
        base.FixedUpdate();
        if (_isLineSwapBlocked || pastIsLineSwapBlocked)
        {
            float newCameraScale = Mathf.Lerp(CameraLineScales[_curLine], CameraLineScales[_targetLine], _curveModif);
            Camera.m_Lens.OrthographicSize = newCameraScale;
        }
    }
}
