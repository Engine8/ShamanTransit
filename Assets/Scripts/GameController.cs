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
    public GameObject IngameUI;

    public GameObject WinScreen;
    public Text WinText;
    public Text WinMoneyText;
    
    public GameObject LoseScreen;
    public Text LoseText;
    public GameObject AttackUI;

    public AudioClip WinSound;
    public AudioClip DefeatSound;
    
    public PlayerController PlayerCharacter;

    //Note: is needed to find better solution
    public Camera MainCamera;
    public CinemachineVirtualCamera VirtCamera;
    public Volume MainCameraVolume;
    public VolumeProfile volumeProfile;
    public ParallaxBackground Moon;
    public float RefreshCameraTime = 0.8f;
    private float _currentRefreshCameraTime = 0f;
    public float LookaheadTimeNormal = 0.55f;
    public float LookaheadTimeAttack = 0.2f;
    private bool _isNeedToRefreshCamera = false;

    public GameObject StartPoint;
    public GameObject EndPoint;

    private float _lastPlayerCharacterXPosition;
    private float _unitsPassed;

    public bool IsAttackMode;
    public bool IsGameEnded;

    public UnityEvent OnGameModeChanged;

    public LoadingComponent loadingComponent;

    public GameObject SnowPrefab;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        IsGameEnded = false;
        WinScreen.SetActive(false);
        LoseScreen.SetActive(false);
        StartCoroutine(CameraShaking(0));
#if UNITY_EDITOR
        SoundManager.Initialize();
        GameData.Initialize();
        if (PlayerDataController.Instance == null)
            PlayerDataController.Initialize();
#endif
    }

    // Start is called before the first frame update
    void Start()
    {
        VirtCamera.GetCinemachineComponent<CinemachineFramingTransposer>().m_ScreenX = 0.58f;

        bool isSnowy = false;
        WindStatus windStatus;
        ChunksPlacer.Instance.GetSnowInfo(out isSnowy, out windStatus);
        if (isSnowy)
        {
            GameObject newSnowObject = Instantiate(SnowPrefab, VirtCamera.transform);
            SnowController snowController = newSnowObject.GetComponent<SnowController>();
            snowController.IsSnowy = true;
            snowController.WindStatusVar = windStatus;
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

        volumeProfile = MainCameraVolume.profile;

        PlayerCharacter.OnDie.AddListener(GameDefeated);
        PlayerCharacter.OnHit.AddListener(OnPlayerHit);
        PlayerCharacter.OnAttackHit.AddListener(OnAttackPlayerHit);
        PlayerCharacter.OnLevelEnd.AddListener(LevelEnded);
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
                CinemachineFramingTransposer framTransposer = VirtCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
                if (IsAttackMode)
                {
                    framTransposer.m_ScreenX = Mathf.Lerp(0.5f, 1, _currentRefreshCameraTime / RefreshCameraTime);
                    framTransposer.m_LookaheadTime = Mathf.Lerp(LookaheadTimeNormal, LookaheadTimeAttack, _currentRefreshCameraTime / RefreshCameraTime);
                }
                else
                {
                    framTransposer.m_ScreenX = Mathf.Lerp(1, 0.5f, _currentRefreshCameraTime / RefreshCameraTime);
                    framTransposer.m_LookaheadTime = Mathf.Lerp(LookaheadTimeAttack, LookaheadTimeNormal, _currentRefreshCameraTime / RefreshCameraTime);
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

    private void ShowEndgameUI(bool isGameWin, string addText)
    {
        IngameUI.SetActive(false);
        AttackUI.SetActive(false);
        //SoundManager.Instance.StopAllSounds();
        if (isGameWin)
        {
            WinScreen.SetActive(true);
            WinText.text = addText;
            SoundManager.Instance.PlaySoundClip(WinSound, false);
            StartCoroutine(MoneyAnimationStart());
            LevelPassed();
        }
        else
        {
            LoseText.text = addText;
            LoseScreen.SetActive(true);
            SoundManager.Instance.PlaySoundClip(DefeatSound, false);
        }
    }

    private IEnumerator MoneyAnimationStart()
    {
        int moneySum = ChunksPlacer.Instance.GetMoneyMultiplier() * PlayerCharacter.SoulCount;
        int curMoney = 0;
        if (moneySum == 0)
        {
            WinMoneyText.text = curMoney.ToString();
        }
        while (curMoney < moneySum)
        {
            curMoney += 100;
            if (curMoney > moneySum)
                curMoney = moneySum;
            WinMoneyText.text = curMoney.ToString();
            yield return new WaitForSeconds(0.1f);
        }
    }

    private void GameDefeated()
    {
        IsGameEnded = true;
        ShowEndgameUI(false, "You was eaten");
    }

    private void LevelEnded()
    {
        IsGameEnded = true;
        if (PlayerCharacter.SoulCount > 0)
            ShowEndgameUI(true, "You done it!");
        else
            ShowEndgameUI(false, "No souls was collected");
    }

    private void LevelPassed()
    {
        PlayerDataController.Instance.AddMoney(ChunksPlacer.Instance.GetMoneyMultiplier() * PlayerCharacter.SoulCount);
        //consider that stage include only 5 levels
        if (PlayerDataController.Instance.Data.CurrentLevel == 4)
        {
            //++PlayerDataController.Instance.Data.CurrentStage;
            int x = 1 + 1;
        }
        else
        {
            ++PlayerDataController.Instance.Data.CurrentLevel;
        }
        PlayerDataController.Instance.WriteData();
    }

    public void OnContinueButtonClick()
    {
        loadingComponent.StartLoadLevel("Map");
    }

    public void OnReloadButtonClick()
    {
        loadingComponent.StartLoadLevel("LevelScene");
    }

    public void SetGameMode(int gameMode)
    {
        if (gameMode == 1)
        {
            IsAttackMode = true;
            AttackUI.SetActive(true);
            AttackUI.GetComponentInChildren<SightScale>().SpeedRotate = 2f;
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
    public void NextScene()
    {
        GameData.Instance.SetCurrentLevel(0, GameData.Instance.CurrentLevel <= 5 ? GameData.Instance.CurrentLevel + 1:0) ;
        loadingComponent.StartLoadLevel("LevelScene");
    }
    public void ShakeCamera(float time)
    {        
        if (!IsGameEnded)
        {
            StartCoroutine(CameraShaking(time));
        }
    }

    private IEnumerator CameraShaking(float time)
    {
        CinemachineBasicMultiChannelPerlin shakeSettings = VirtCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();

        shakeSettings.m_FrequencyGain = 2f;
        shakeSettings.m_AmplitudeGain = 1f;
        yield return new WaitForSeconds(time);
        shakeSettings.m_FrequencyGain = 0;
        shakeSettings.m_AmplitudeGain = 0;
    }
}
