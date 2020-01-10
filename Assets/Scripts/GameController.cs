using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public static GameController GameControllerInstance;

    public TextMeshProUGUI TextMeshScore;
    public TextMeshProUGUI TextMeshRoute;
    public PlayerController PlayerCharacter;

    //Note: is needed to find better solution
    public Camera MainCamera;
    public ParallaxBackground Moon;

    [SerializeField]
    private GameObject StartPoint;
    [SerializeField]
    private GameObject EndPoint;

    private float _lastPlayerCharacterXPosition;
    private float _unitsPassed;

    private void Awake()
    {
        if (GameControllerInstance == null)
        {
            GameControllerInstance = new GameController();
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        if (TextMeshScore != null)
        {
            TextMeshScore.SetText("Score: 0");
        }
        else
        {
            Debug.LogError("TextMeshScore reference in GameController not stated");
        }
        if (TextMeshRoute != null)
        {
            TextMeshRoute.SetText("0 %");
        }
        else
        {
            Debug.LogError("TextMeshRoute reference in GameController not stated");
        }
        if (PlayerCharacter != null)
        {
            PlayerCharacter.OnHit.AddListener(OnPlayerHit);
            _lastPlayerCharacterXPosition = PlayerCharacter.transform.position.x;
        }
        else
        { 
            Debug.LogError("PlayerCharacter reference in GameController not stated");
        }

        if (StartPoint == null || EndPoint == null)
        {
            Debug.LogError("Start or End point reference in GameController not stated");
        }
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (PlayerCharacter.transform.position.x < EndPoint.transform.position.x)
        {
            _unitsPassed += PlayerCharacter.transform.position.x - _lastPlayerCharacterXPosition;
            _lastPlayerCharacterXPosition = PlayerCharacter.transform.position.x;
            AddScore(Mathf.FloorToInt(_unitsPassed / 4));
            _unitsPassed %= 4;

            float routeLength = EndPoint.transform.position.x - StartPoint.transform.position.x;
            float routeDoneInPercents = (PlayerCharacter.transform.position.x - StartPoint.transform.position.x) / routeLength * 100;
            TextMeshRoute.SetText($"{routeDoneInPercents} %");

            //Note: needed to find better solution
            float cameraWidth = MainCamera.ViewportToWorldPoint(new Vector3(1f, 1f, MainCamera.transform.position.z)).x 
                              - MainCamera.ViewportToWorldPoint(new Vector3(0f, 1f, MainCamera.transform.position.z)).x;
            Debug.Log($"CameraWidth = {cameraWidth}");
            Moon.Offset = new Vector2( cameraWidth * routeDoneInPercents / 100, 0);
        }
        else
        {
            PlayerCharacter.Acceleration = 0;
            PlayerCharacter.Speed = 0;
        }
    }

    private void OnPlayerHit()
    {
        AddScore(-50);
    }

    private void AddScore(int value)
    {
        int score = int.Parse(TextMeshScore.text.Substring(6));
        score += value;
        if (score < 0)
            score = 0;
        TextMeshScore.SetText($"Score: {score}");
    }

}
