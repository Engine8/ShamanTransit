using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxBackground : MonoBehaviour
{   
    public Transform CameraTransform;
    public Vector2 ParallaxMultiplier = new Vector2(0.5f, 0.5f);
    [SerializeField]
    private Vector2 _offset = new Vector2(0, 0); 
    [SerializeField]
    private bool _parallaxOnX = false;
    [SerializeField]
    private bool _parallaxOnY = false;

    private Vector3 _lastCameraPosition;
    private float _textureUnitSizeX;
    private float _textureUnitSizeY;
    private Vector2 _movementOffset = new Vector2(0, 0);

    public bool debug;

    public Vector2 Offset
    {
        get
        {
            return _offset;
        }
        set
        {
            _movementOffset = value - _offset;
            _offset = value;
        }
    }


    private void Start()
    {
        if (CameraTransform != null)
        {
            _lastCameraPosition = CameraTransform.position;
            Sprite sprite = GetComponent<SpriteRenderer>().sprite;
            Texture2D texture = sprite.texture;
            _textureUnitSizeX = texture.width / sprite.pixelsPerUnit;
            if (debug)
            {
                Debug.Log($"Texture width = {texture.width}");
                Debug.Log($"Pixel per unit = {sprite.pixelsPerUnit}");
            }
            _textureUnitSizeY = texture.height / sprite.pixelsPerUnit;
        }
        else
        {
            Debug.LogError($"ParallaxBacground at GameObject {gameObject.name} hasn't CameraTransform reference");
        }
    }

    private void LateUpdate()
    {
        //move object after the camera
        Vector3 deltaMovement = CameraTransform.position - _lastCameraPosition;
        transform.position += new Vector3(deltaMovement.x * ParallaxMultiplier.x + _movementOffset.x, deltaMovement.y * ParallaxMultiplier.y + _movementOffset.y, deltaMovement.z);
        _lastCameraPosition = CameraTransform.position;

        if (_parallaxOnX)
        {
            //check if the camera is out of bounds on x axis
            if (Mathf.Abs(CameraTransform.position.x - transform.position.x) >= _textureUnitSizeX)
            {
                //calculate offset to create sense of static picture
                float offsetPositionX = (CameraTransform.position.x - transform.position.x) % _textureUnitSizeX;
                transform.position = new Vector3(CameraTransform.position.x + offsetPositionX /*+ _offset.x*/, transform.position.y);
            }
        }
        if (_parallaxOnY)
        {
            //check if the camera is out of bounds on y axis
            if (Mathf.Abs(CameraTransform.position.y - transform.position.y) >= _textureUnitSizeY)
            {
                //calculate offset to create sense of static picture
                float offsetPositionY = (CameraTransform.position.y - transform.position.y) % _textureUnitSizeY;
                transform.position = new Vector3(transform.position.x, CameraTransform.position.y + offsetPositionY + _offset.y);
            }
        }
    }
}
