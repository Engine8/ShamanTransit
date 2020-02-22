using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering;

public class GameController : MonoBehaviour
{
    public static GameController Instance;


    //UI
    //public TextMeshProUGUI TextMeshCargo;
    //public TextMeshProUGUI TextMeshRoute;

    public GameObject IngameUI;
    public GameObject EndgameUI;
    public Text EndgameMainText;
    //public Text EndgameCargoText;
    public Text EndgameMoneyText;

    private AudioSource _uiAudioSource;

    public AudioClip WinSound;
    public AudioClip DefeatSound;
    public AudioClip CoinSound;
    
    public PlayerController PlayerCharacter;

    //Note: is needed to find better solution
    public Camera MainCamera;
    public Volume MainCameraVolume;
    public VolumeProfile volumeProfile;
    public ParallaxBackground Moon;

    //Amount of cargo that lost on hit
    //Maybe defined by level in scene manager
    //public int CargoPerHit = 1;
    public int MoneyPerLevel = 500;

    public GameObject StartPoint;
    public GameObject EndPoint;

    private float _lastPlayerCharacterXPosition;
    private float _unitsPassed;

    public bool IsGameEnded;

    public LoadingComponent loadingComponent;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }

        _uiAudioSource = transform.Find("UIAudioSource").gameObject.GetComponent<AudioSource>();
        IsGameEnded = false;
        EndgameUI.SetActive(false);
    }

    // Start is called before the first frame update
    void Start()
    {
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

        volumeProfile = MainCameraVolume.profile;

        PlayerCharacter.OnDie.AddListener(GameDefeated);
        PlayerCharacter.OnHit.AddListener(OnPlayerHit);
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (PlayerCharacter.transform.position.x < EndPoint.transform.position.x)
        {
            //calculate moon or sun position
            _unitsPassed += PlayerCharacter.transform.position.x - _lastPlayerCharacterXPosition;
            _lastPlayerCharacterXPosition = PlayerCharacter.transform.position.x;
            //AddScore(Mathf.FloorToInt(_unitsPassed / 4));
            _unitsPassed %= 4;

            float routeLength = EndPoint.transform.position.x - StartPoint.transform.position.x;
            float routeDoneInPercents = (PlayerCharacter.transform.position.x - StartPoint.transform.position.x) / routeLength * 100;
            //TextMeshRoute.SetText($"{routeDoneInPercents} %");

            //Note: needed to find better solution
            float cameraWidth = MainCamera.ViewportToWorldPoint(new Vector3(1f, 1f, MainCamera.transform.position.z)).x 
                              - MainCamera.ViewportToWorldPoint(new Vector3(0f, 1f, MainCamera.transform.position.z)).x;
            //Debug.Log($"CameraWidth = {cameraWidth}");
            Moon.Offset = new Vector2( cameraWidth * routeDoneInPercents / 100, 0);
        }
        else
        {
            // if game has not already ended
            if (!IsGameEnded) 
            {
                PlayerCharacter.Acceleration = 0;
                PlayerCharacter.Speed = 0;
                ShowEndgameUI(true);
                IsGameEnded = true;
            }
        }
    }

    private void OnPlayerHit()
    {
        if (!IsGameEnded)
        {
            UnityEngine.Rendering.Universal.Vignette vignette;
            volumeProfile.TryGet(out vignette);

            vignette.intensity.Override(0.25f + 0.30f / PlayerCharacter.MaxHP * (PlayerCharacter.MaxHP - PlayerCharacter.CurrentHP));
        }
    }

    private void ShowEndgameUI(bool isGameWin)
    {
        IngameUI.SetActive(false);
        //EndgameCargoText.text = $"{PlayerCharacter.CurrentCargoCount} / {PlayerCharacter.MaxCargoCount}";
        if (isGameWin)
        {
            EndgameMainText.text = "You win!";
        }
        else
        {
            EndgameMainText.text = "You fail";
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
        int moneySum = MoneyPerLevel;
        int curMoney = 0;
        int i = 1;
        while (curMoney < moneySum)
        {
            Debug.Log($"Step {i}");
            curMoney += 100;
            if (curMoney > moneySum)
                curMoney = moneySum;
            EndgameMoneyText.text = curMoney.ToString();
            yield return new WaitForSeconds(0.1f);
        }
    }

    private void GameDefeated()
    {
        IsGameEnded = true;
        ShowEndgameUI(false);
    }

    public void OnContinueButtonClick()
    {
        PlayerDataController.Instance.AddMoney(MoneyPerLevel);
        //consider that stage include only 5 levels
        PlayerDataController.Instance.Data.CurrentLevel = ++PlayerDataController.Instance.Data.CurrentLevel % 5;
        PlayerDataController.Instance.Data.CurrentStage = PlayerDataController.Instance.Data.CurrentLevel / 5 + 1;
        PlayerDataController.Instance.WriteData();
        loadingComponent.StartLoadLevel("Map");
    }
}
