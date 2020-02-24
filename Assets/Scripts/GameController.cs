﻿using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering;
using UnityEngine.Events;
using Cinemachine;

public class GameController : MonoBehaviour
{
    public static GameController Instance;


    //UI
    //public TextMeshProUGUI TextMeshCargo;
    //public TextMeshProUGUI TextMeshRoute;

    public GameObject IngameUI;
    public GameObject EndgameUI;
    public GameObject AttackUI;
    public Text EndgameMainText;
    public Text EndgameMoneyText;

    private AudioSource _uiAudioSource;

    public AudioClip WinSound;
    public AudioClip DefeatSound;
    public AudioClip CoinSound;
    
    public PlayerController PlayerCharacter;

    //Note: is needed to find better solution
    public Camera MainCamera;
    public CinemachineVirtualCamera VirtCamera;
    public Volume MainCameraVolume;
    public VolumeProfile volumeProfile;
    public ParallaxBackground Moon;
    public float RefreshCameraTime = 0.8f;
    private float _currentRefreshCameraTime = 0f;
    private bool _isNeedToRefreshCamera = false;
    //Amount of cargo that lost on hit
    //Maybe defined by level in scene manager
    //public int CargoPerHit = 1;
    public int MoneyPerLevel = 500;

    public GameObject StartPoint;
    public GameObject EndPoint;

    private float _lastPlayerCharacterXPosition;
    private float _unitsPassed;

    public bool IsAttackMode;
    public bool IsGameEnded;

    public UnityEvent OnGameModeChanged;

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
        VirtCamera.GetCinemachineComponent<CinemachineFramingTransposer>().m_ScreenX = 0.5f;
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
        PlayerCharacter.OnAttackHit.AddListener(OnAttackPlayerHit);
        PlayerCharacter.OnLevelEnd.AddListener(GameWin);
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (!IsGameEnded)
        {
            //calculate moon or sun position
            _unitsPassed += PlayerCharacter.transform.position.x - _lastPlayerCharacterXPosition;
            _lastPlayerCharacterXPosition = PlayerCharacter.transform.position.x;
            //AddScore(Mathf.FloorToInt(_unitsPassed / 4));
            _unitsPassed %= 4;

            float routeLength = EndPoint.transform.position.x - StartPoint.transform.position.x;
            float routeDoneInPercents = (PlayerCharacter.transform.position.x - StartPoint.transform.position.x) / routeLength * 100;
            //TextMeshRoute.SetText($"{routeDoneInPercents} %");

            //Note: find better solution
            float cameraWidth = MainCamera.ViewportToWorldPoint(new Vector3(1f, 1f, MainCamera.transform.position.z)).x 
                              - MainCamera.ViewportToWorldPoint(new Vector3(0f, 1f, MainCamera.transform.position.z)).x;
            //Debug.Log($"CameraWidth = {cameraWidth}");
            Moon.Offset = new Vector2( cameraWidth * routeDoneInPercents / 100, 0);

            if (_isNeedToRefreshCamera)
            {
                _currentRefreshCameraTime += Time.deltaTime;
                bool isEnded = false;
                if (_currentRefreshCameraTime >= RefreshCameraTime)
                {
                    _currentRefreshCameraTime = RefreshCameraTime;
                    isEnded = true;
                }
                if (IsAttackMode)
                {
                    VirtCamera.GetCinemachineComponent<CinemachineFramingTransposer>().m_ScreenX = Mathf.Lerp(VirtCamera.GetCinemachineComponent<CinemachineFramingTransposer>().m_ScreenX, 
                                                                                                              1, _currentRefreshCameraTime / RefreshCameraTime);
                }
                else
                {
                    VirtCamera.GetCinemachineComponent<CinemachineFramingTransposer>().m_ScreenX = Mathf.Lerp(VirtCamera.GetCinemachineComponent<CinemachineFramingTransposer>().m_ScreenX, 
                                                                                                              0.5f, _currentRefreshCameraTime / RefreshCameraTime);
                }
                if (isEnded)
                {
                    _currentRefreshCameraTime = 0f;
                    _isNeedToRefreshCamera = false;
                }
            }
        }
    } 
    private void OnPlayerHit()
    {
        if (!IsGameEnded)
        {
            StartCoroutine("Damage");
        }
    }
    IEnumerator Damage()
    {
        UnityEngine.Rendering.Universal.Vignette vignette;
        volumeProfile.TryGet(out vignette);

        vignette.intensity.Override(0.35f);
        yield return new WaitForSeconds(0.5f);
        vignette.intensity.Override(0f);
    }
    private void BattleGameWin()
    {
        UnityEngine.Rendering.Universal.Vignette vignette;
        volumeProfile.TryGet(out vignette);
        vignette.intensity.Override(0f);
        PlayerCharacter.CurrentHPBattle = PlayerCharacter.MaxHPBattle;
    }
    private void OnAttackPlayerHit()
    {
        UnityEngine.Rendering.Universal.Vignette vignette;
        volumeProfile.TryGet(out vignette);

        vignette.intensity.Override(0.25f + 0.30f / PlayerCharacter.MaxHPBattle * (PlayerCharacter.MaxHPBattle - PlayerCharacter.CurrentHPBattle));
    }
    private void ShowEndgameUI(bool isGameWin)
    {
        IngameUI.SetActive(false);
        AttackUI.SetActive(false);
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
        int moneySum = ChunksPlacer.Instance.GetMoneyMultiplier() * PlayerCharacter.SoulCount;
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

    private void GameWin()
    {
        IsGameEnded = true;
        ShowEndgameUI(true);
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

    public void SetGameMode(int gameMode)
    {
        if (gameMode == 1)
        {
            IsAttackMode = true;
            AttackUI.SetActive(true);
            AttackUI.GetComponent<SightScale>().SpeedRotate = 2f;
            _isNeedToRefreshCamera = true;
        }
        else if (gameMode == 0)
        {
            BattleGameWin();
            IsAttackMode = false;
            _isNeedToRefreshCamera = true;
        }

        OnGameModeChanged.Invoke();
    }
}
