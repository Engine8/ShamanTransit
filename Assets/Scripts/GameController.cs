using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    public static GameController GameControllerInstance;


    //UI
    public TextMeshProUGUI TextMeshCargo;
    public TextMeshProUGUI TextMeshRoute;

    public GameObject IngameUI;
    public GameObject EndgameUI;
    private Text _endgameMainText;
    private Text _endgameCargoText;
    private Text _endgameMoneyText;

    private AudioSource _uiAudioSource;

    public AudioClip WinSound;
    public AudioClip DefeatSound;
    public AudioClip CoinSound;
    
    public PlayerController PlayerCharacter;

    //Note: is needed to find better solution
    public Camera MainCamera;
    public ParallaxBackground Moon;

    //Amount of cargo that lost on hit
    //Maybe defined by level in scene manager
    public int CargoPerHit = 1;
    public int MoneyPerCargo = 500;

    [SerializeField]
    private GameObject StartPoint;
    [SerializeField]
    private GameObject EndPoint;

    private float _lastPlayerCharacterXPosition;
    private float _unitsPassed;

    private bool _isGameEnded;

    private void Awake()
    {
        if (GameControllerInstance == null)
        {
            GameControllerInstance = new GameController();
        }

        //Find UI elements
        _endgameMainText = EndgameUI.transform.Find("MainText").gameObject.GetComponent<Text>();
        _endgameCargoText = EndgameUI.transform.Find("CargoText").gameObject.GetComponent<Text>();
        _endgameMoneyText = EndgameUI.transform.Find("MoneyText").gameObject.GetComponent<Text>();

        _uiAudioSource = transform.Find("UIAudioSource").gameObject.GetComponent<AudioSource>();
        _isGameEnded = false;
        EndgameUI.SetActive(false);
    }

    // Start is called before the first frame update
    void Start()
    {
        if (TextMeshCargo != null)
        {
            UpdateCargoUI();
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

        PlayerCharacter.OnDie.AddListener(GameDefeated);
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (PlayerCharacter.transform.position.x < EndPoint.transform.position.x)
        {
            _unitsPassed += PlayerCharacter.transform.position.x - _lastPlayerCharacterXPosition;
            _lastPlayerCharacterXPosition = PlayerCharacter.transform.position.x;
            //AddScore(Mathf.FloorToInt(_unitsPassed / 4));
            _unitsPassed %= 4;

            float routeLength = EndPoint.transform.position.x - StartPoint.transform.position.x;
            float routeDoneInPercents = (PlayerCharacter.transform.position.x - StartPoint.transform.position.x) / routeLength * 100;
            TextMeshRoute.SetText($"{routeDoneInPercents} %");

            //Note: needed to find better solution
            float cameraWidth = MainCamera.ViewportToWorldPoint(new Vector3(1f, 1f, MainCamera.transform.position.z)).x 
                              - MainCamera.ViewportToWorldPoint(new Vector3(0f, 1f, MainCamera.transform.position.z)).x;
            //Debug.Log($"CameraWidth = {cameraWidth}");
            Moon.Offset = new Vector2( cameraWidth * routeDoneInPercents / 100, 0);
        }
        else
        {
            if (!_isGameEnded)
            {
                PlayerCharacter.Acceleration = 0;
                PlayerCharacter.Speed = 0;
                ShowEndgameUI(true);
                _isGameEnded = true;
            }
        }
    }

    private void OnPlayerHit()
    {
        PlayerCharacter.CurrentCargoCount -= CargoPerHit;
        if (PlayerCharacter.CurrentCargoCount <= 0)
        {
            PlayerCharacter.CurrentCargoCount = 0;
            ShowEndgameUI(false);
        }
        UpdateCargoUI();
    }

    private void UpdateCargoUI()
    {
        TextMeshCargo.SetText($"{PlayerCharacter.CurrentCargoCount}/{PlayerCharacter.MaxCargoCount}");
    }

    private void ShowEndgameUI(bool isGameWin)
    {
        IngameUI.SetActive(false);
        _endgameCargoText.text = $"{PlayerCharacter.CurrentCargoCount} / {PlayerCharacter.MaxCargoCount}";
        if (isGameWin)
        {
            _endgameMainText.text = "You win!";
        }
        else
        {
            _endgameMainText.text = "You fail";
        }
        EndgameUI.SetActive(true);
        if (isGameWin)
        {
            _uiAudioSource.clip = WinSound;
            _uiAudioSource.PlayOneShot(WinSound);
            StartCoroutine(MoneyAnimationStart());
        }
        else
        {
            _uiAudioSource.PlayOneShot(DefeatSound);
        }   
    }

    private IEnumerator MoneyAnimationStart()
    {
        int moneySum = PlayerCharacter.CurrentCargoCount * MoneyPerCargo;
        int curMoney = 0;
        int i = 1;
        while (curMoney < moneySum)
        {
            Debug.Log($"Step {i}");
            curMoney += 100;
            if (curMoney > moneySum)
                curMoney = moneySum;
            _endgameMoneyText.text = curMoney.ToString();
            yield return new WaitForSeconds(0.1f);
        }
    }

    private void GameDefeated()
    {
        _isGameEnded = true;
        ShowEndgameUI(false);
    }
}
