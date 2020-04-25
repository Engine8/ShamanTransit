using System.Collections;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering;
using UnityEngine.Events;
using UnityEngine.Experimental.Rendering.Universal;
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

    //-----------Camera status------------
    [Serializable]
    public struct CameraStatusSettings
    {
        //Run status
        [Header("Run status")]
        public float RefreshTimeRun;
        public float LookaheadTimeRun;
        [Range(0, 1)]
        public float ScreenXPosRun;
        //Attack status
        [Header("Attack status")]
        public float RefreshTimeAttack;
        public float LookaheadTimeAttack;
        [Range(0, 1)]
        public float ScreenXPosAttack;
        //death status
        [Header("Death status")]
        public float RefreshTimeDeath;
        public float LookaheadTimeDeath;
        [Range(0, 1)]
        public float ScreenXPosDeath;
    }
    public enum CameraStatusE
    {
        Run = 0,
        Attack = 1,
        Death = 2,
    }

    //This variable stores settings
    public CameraStatusSettings CameraSettings;
    //These variables stores settings that should be set up
    private float _targetRefreshCameraTime;
    private float _targetLookaheadTime;
    private float _targetScreenXPos;
    //These variable stores settings that was set up in current status
    private float _currentRefreshCameraTime;
    private float _currentLookaheadTime;
    private float _currentScreenXPos;

    private CameraStatusE _currentCameraStatus;
    private CameraStatusE _targetCameraStatus;
    private bool _isNeedToRefreshCamera = false;
    private float _currentRefreshTime = 0f;
    //----------- End Camera status------------

    public GameObject StartPoint;
    public GameObject EndPoint;

    private float _lastPlayerCharacterXPosition;
    private float _unitsPassed;

    public bool IsAttackMode;
    public bool IsGameEnded;

    public UnityEvent OnGameModeChanged;

    public LoadingComponent loadingComponent;

    public GameObject SnowPrefab;

    /*
     * Player die variables
     */
    public Light2D GlobalLight;
    private Color _defaultColor;
    public Color PlayerDeadColor;
    //Time for player to enable second life
    public float DieTime = 2f;
    public float EndgameTime = 2f;
    private bool _secondChance = false;
    private Coroutine _secondChanceCoroutine = null;

    delegate void VoidFunc();

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

        PlayerCharacter.OnDieEnd.AddListener(OnPlayerDie);
        PlayerCharacter.OnHit.AddListener(OnPlayerHit);
        PlayerCharacter.OnAttackHit.AddListener(OnAttackPlayerHit);
        PlayerCharacter.OnLevelEnd.AddListener(LevelEnded);
        PlayerCharacter.OnSecondChanceClick.AddListener(RevivePlayerCharacter);

        _defaultColor = GlobalLight.color;

        //set initial camera settings
        _currentCameraStatus = CameraStatusE.Run;
        _currentLookaheadTime = CameraSettings.LookaheadTimeRun;
        _currentRefreshCameraTime = CameraSettings.RefreshTimeRun;
        _currentScreenXPos = CameraSettings.ScreenXPosRun;
    }

    // Update is called once per frame
    void LateUpdate()
    {
            //Moon and sun movement
            /*
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
            */
        if (_isNeedToRefreshCamera)
        {
            _currentRefreshTime += Time.deltaTime;
            bool isEnded = false;
            if (_currentRefreshTime >= _targetRefreshCameraTime)
            {
                _currentRefreshTime = _targetRefreshCameraTime;
                isEnded = true;
            }
            float refreshValue = _currentRefreshTime / _targetRefreshCameraTime;
            CinemachineFramingTransposer framTransposer = VirtCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
            framTransposer.m_ScreenX = Mathf.Lerp(_currentScreenXPos, _targetScreenXPos, refreshValue);
            framTransposer.m_LookaheadTime = Mathf.Lerp(_currentLookaheadTime, _targetLookaheadTime, refreshValue);

            if (isEnded)
            {
                _currentCameraStatus = _targetCameraStatus;

                _currentLookaheadTime = _targetLookaheadTime;
                _currentRefreshCameraTime = _targetRefreshCameraTime;
                _currentScreenXPos = _targetScreenXPos;

                _currentRefreshTime = 0f;
                _isNeedToRefreshCamera = false;

                if (PlayerCharacter.GetDead())
                    ActivateSecondChance();
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

    private void OnPlayerDie()
    {
        //set camera target settings
        SetTargetCameraSettings(CameraStatusE.Death);

        //check second chance availability
        if (PlayerDataController.Instance.HasItem("Second life") == 0)
        {
            StartCoroutine(ChangeGlobalLight(EndgameTime, _defaultColor, PlayerDeadColor, GameDefeated));
        }

    }

    public void ActivateSecondChance()
    {
        //check if the second life item purchased
        if (PlayerDataController.Instance.HasItem("Second life") != 0)
        {
            PlayerCharacter.EnableSecondChance();
            SecondChanceStartAnimate();
            PlayerDataController.Instance.UseItem("Second life");
        }
    }

    //Update global light each frame for DieTime seconds or until player's click
    private void SecondChanceStartAnimate()
    {
        _secondChance = true;
        _secondChanceCoroutine = StartCoroutine(ChangeGlobalLight(DieTime, _defaultColor, PlayerDeadColor, SecondChanceFailed));
        /*
         * backup
        float curTime = 0f;
        bool end = false;
        while (!end)
        {
            curTime += Time.deltaTime;
            if (curTime > DieTime)
            {
                curTime = DieTime;
                end = true;
            }

            float t = curTime / DieTime;
            Color curGlobalLightColor = Color.Lerp(_defaultColor, PlayerDeadColor, t);
            GlobalLight.color = curGlobalLightColor;
            yield return null;
        }
        //if coroutine has not been break this means player didn't use second life and game is defeated
        _secondChance = false;
        PlayerCharacter.DisableSecondChance();
        GameDefeated();
        */
    }

    private void SecondChanceFailed()
    {
        _secondChance = false;
        PlayerCharacter.DisableSecondChance();
        GameDefeated();
    }

    IEnumerator RestoreColors()
    {
        Color startColor = GlobalLight.color;
        float curTime = 0f;
        bool end = false;
        while (!end)
        {
            curTime += Time.deltaTime;
            if (curTime > 1f)
            {
                curTime = 1f;
                end = true;
            }

            float t = curTime;
            Color curGlobalLightColor = Color.Lerp(startColor, _defaultColor, t);
            GlobalLight.color = curGlobalLightColor;
            yield return null;
        }
    }

    private void RevivePlayerCharacter()
    {
        SetTargetCameraSettings(CameraStatusE.Run);

        //Stop SecondChanceAnimation coroutine
        if (_secondChanceCoroutine != null)
        {
            StopCoroutine(_secondChanceCoroutine);
            _secondChanceCoroutine = null;
            Debug.Log("Second chance activated");
        }
        StartCoroutine(RestoreColors());
    }

    private IEnumerator ChangeGlobalLight(float time, Color startColor, Color endColor, VoidFunc func)
    {
        float curTime = 0f;
        bool end = false;
        while (!end)
        {
            curTime += Time.deltaTime;
            if (curTime > time)
            {
                curTime = time;
                end = true;
            }

            float t = curTime / time;
            Color curGlobalLightColor = Color.Lerp(startColor, endColor, t);
            GlobalLight.color = curGlobalLightColor;
            yield return null;
        }

        if (func != null)
            func();
    }

    private void GameDefeated()
    {
        IsGameEnded = true;
        ShowEndgameUI(false, "Тебя съели");
    }

    private void LevelEnded()
    {
        IsGameEnded = true;
        if (PlayerCharacter.SoulCount > 0)
            ShowEndgameUI(true, "Ты сделал это!");
        else
            ShowEndgameUI(false, "Не было собрано ни одной души");
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

    public void SetGameMode(int gameMode, bool isNeedToChangeCamera)
    {
        if (gameMode == 1)
        {
            IsAttackMode = true;
            AttackUI.SetActive(true);
            //AttackUI.GetComponentInChildren<SightScale>().SpeedRotate = 2f;

            if (isNeedToChangeCamera)
            {
                SetTargetCameraSettings(CameraStatusE.Attack);
            }
        }
        else if (gameMode == 0)
        {
            BattleGameWin();
            IsAttackMode = false;

            if (isNeedToChangeCamera)
            {
                SetTargetCameraSettings(CameraStatusE.Run);
            }
        }

        OnGameModeChanged.Invoke();
    }

    public void NextScene()
    {
        GameData.Instance.SetCurrentLevel(0, GameData.Instance.CurrentLevel < 5 ? GameData.Instance.CurrentLevel + 1:0) ;
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

    public void SetTargetCameraSettings(CameraStatusE targetStatus)
    {
        //set camera run status settings
        _targetCameraStatus = targetStatus;
        if (_targetCameraStatus == CameraStatusE.Run)
        {
            _targetLookaheadTime = CameraSettings.LookaheadTimeRun;
            _targetRefreshCameraTime = CameraSettings.RefreshTimeRun;
            _targetScreenXPos = CameraSettings.ScreenXPosRun;
        }
        else if (_targetCameraStatus == CameraStatusE.Attack)
        {
            _targetLookaheadTime = CameraSettings.LookaheadTimeAttack;
            _targetRefreshCameraTime = CameraSettings.RefreshTimeAttack;
            _targetScreenXPos = CameraSettings.ScreenXPosAttack;
        }
        else if (_targetCameraStatus == CameraStatusE.Death)
        {
            _targetLookaheadTime = CameraSettings.LookaheadTimeDeath;
            _targetRefreshCameraTime = CameraSettings.RefreshTimeDeath;
            _targetScreenXPos = CameraSettings.ScreenXPosDeath;
        }
        _isNeedToRefreshCamera = true;
    }
}
