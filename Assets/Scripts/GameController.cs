using Cinemachine;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Experimental.Rendering.Universal;
using UnityEngine.Rendering;
using UnityEngine.UI;

public enum GameMode
{
    Default = 0,
    BossFight = 1,
}

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
    public HitArea HitAreaRef;

    public AudioClip WinSound;
    public AudioClip DefeatSound;

    //-------------Player--------------------
    [Space(20, order = 0)]
    [Header("Player", order = 1)]
    public PlayerController PlayerCharacter;

    public float DieTime = 2f;
    public float EndgameTime = 2f;
    private bool _secondChanceRequested = false;
    private bool _secondChanceUsed = false;
    private Coroutine _secondChanceCoroutine = null;
    public Color PlayerDeadColor;

    //------------end player-----------------


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

    [Space(20, order = 0)]
    [Header("Camera", order = 1)]
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


    //--------------Game status----------------
    public enum GameStatus
    {
        Run = 0,
        BossRun = 1,
        Attack = 2,
    }

    public GameStatus CurrentGameStatus;
    public GameMode CurrentGameMode;

    //-----------End game status---------------

    public GameObject StartPoint;
    public GameObject EndPoint;

    private float _lastPlayerCharacterXPosition;
    private float _unitsPassed;

    public bool IsGameEnded;

    public UnityEvent OnGameModeChanged;

    public LoadingComponent loadingComponent;

    public GameObject SnowPrefab;

    /*
     * Player die variables
     */
    public Light2D GlobalLight;
    private Color _defaultColor;


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
        //StartCoroutine(CameraShaking(0));
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
        CurrentGameMode = ChunksPlacer.Instance.GetMapGameMode();
        if (isSnowy)
        {
            GameObject newSnowObject = Instantiate(SnowPrefab, VirtCamera.transform);
            SnowController snowController = newSnowObject.GetComponent<SnowController>();
            snowController.IsSnowy = true;
            snowController.WindStatusVar = windStatus;
        }
            
        volumeProfile = MainCameraVolume.profile;

        _lastPlayerCharacterXPosition = PlayerCharacter.transform.position.x;
        PlayerCharacter.OnDieEnd.AddListener(OnPlayerDie);
        PlayerCharacter.OnHit.AddListener(OnPlayerHit);
        PlayerCharacter.OnAttackHit.AddListener(OnAttackPlayerHit);
        PlayerCharacter.OnLevelEnd.AddListener(LevelEnded);
        PlayerCharacter.OnSecondChanceClick.AddListener(RevivePlayerCharacter);

        _defaultColor = GlobalLight.color;

        //set default camera settings on level start
        SetTargetCameraSettings(CameraStatusE.Run);
        CinemachineFramingTransposer framTransposer = VirtCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
        _currentLookaheadTime = framTransposer.m_LookaheadTime;
        _currentScreenXPos = framTransposer.m_ScreenX;

        _currentRefreshTime = 0f;
        _isNeedToRefreshCamera = true;
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

                if (_secondChanceRequested)
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

    private void EndBattle()
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
            UpdatePlayerDataOnLevelPass();
        }
        else
        {
            LoseText.text = addText;
            LoseScreen.SetActive(true);
            SoundManager.Instance.PlaySoundClip(DefeatSound, false);
        }
        PlayerDataController.Instance.WriteData();
    }

    private IEnumerator MoneyAnimationStart()
    {
        int moneySum = ChunksPlacer.Instance.GetBasicReward() + ChunksPlacer.Instance.GetMoneyMultiplier() * PlayerCharacter.SoulCount;
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
        if (PlayerDataController.Instance.HasItem(1) == 0 || _secondChanceUsed)
        {
            StartCoroutine(ChangeGlobalLight(EndgameTime, _defaultColor, PlayerDeadColor, LevelEnded));
        }
        else
            //second chance will be activated on LateUpdate
            _secondChanceRequested = true;

    }

    public void ActivateSecondChance()
    {
        //check if the second life item purchased
        if (PlayerDataController.Instance.HasItem(1) != 0)
        {
            PlayerCharacter.EnableSecondChance();
            SecondChanceStartAnimate();
        }
        _secondChanceRequested = false;
    }

    //Update global light each frame for DieTime seconds or until player's click
    private void SecondChanceStartAnimate()
    {
        _secondChanceCoroutine = StartCoroutine(ChangeGlobalLight(DieTime, _defaultColor, PlayerDeadColor, SecondChanceFailed));
    }

    private void SecondChanceFailed()
    {
        PlayerCharacter.DisableSecondChance();
        LevelEnded();
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
        if (CurrentGameStatus == GameStatus.Attack)
        {
            SetTargetCameraSettings(CameraStatusE.Attack);
            ((Boss)HitAreaRef.BossRef).ContinueBattle();
            AttackUI.SetActive(true);
        }
        else if (CurrentGameStatus == GameStatus.Run)
            SetTargetCameraSettings(CameraStatusE.Run);

        //Stop SecondChanceAnimation coroutine
        if (_secondChanceCoroutine != null)
        {
            StopCoroutine(_secondChanceCoroutine);
            _secondChanceCoroutine = null;
            //Debug.Log("Second chance activated");
        }
        _secondChanceUsed = true;

        PlayerDataController.Instance.UseItem(1);
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

    private void LevelEnded()
    {
        IsGameEnded = true;
        if (PlayerCharacter.GetDead())
            ShowEndgameUI(false, "Тебя съели");
        else if (PlayerCharacter.SoulCount > 0 || CurrentGameMode == GameMode.BossFight)
            ShowEndgameUI(true, "Ты сделал это!");
        else
            ShowEndgameUI(false, "Не было собрано ни одной души");
    }
    
    //updates player data (not write it)
    private void UpdatePlayerDataOnLevelPass()
    {
        PlayerDataController.Instance.AddMoney(ChunksPlacer.Instance.GetMoneyMultiplier() * PlayerCharacter.SoulCount);
        
        //check what passed last available level
        if (GameData.Instance.CurrentLevel == PlayerDataController.Instance.Data.CurrentLevel)
        {
            //we have only one stage and consider that stage include only 5 levels
            if (PlayerDataController.Instance.Data.CurrentLevel == 4)
            {
                //++PlayerDataController.Instance.Data.CurrentStage;
                int x = 1 + 1;
            }
            else
            {
                ++PlayerDataController.Instance.Data.CurrentLevel;
            }
        }
    }

    public void OnContinueButtonClick()
    {
        loadingComponent.StartLoadLevel("Map");
    }

    public void OnReloadButtonClick()
    {
        loadingComponent.StartLoadLevel("LevelScene");
    }

    public void SetGameStatus(GameStatus gameStatus, bool isNeedToChangeCamera)
    { 
        if (gameStatus == GameStatus.Attack)
        {
            CurrentGameStatus = GameStatus.Attack;
            AttackUI.SetActive(true);
            if (isNeedToChangeCamera)
            {
                SetTargetCameraSettings(CameraStatusE.Attack);
            }
        }
        else if (gameStatus == GameStatus.BossRun)
        {
            CurrentGameStatus = GameStatus.BossRun;
            //EndBattle();
            if (isNeedToChangeCamera)
            {
                SetTargetCameraSettings(CameraStatusE.Death);
            }
        }
        else if (gameStatus == GameStatus.Run)
        {
            CurrentGameStatus = GameStatus.Run;
            EndBattle();
            if (isNeedToChangeCamera)
            {
                SetTargetCameraSettings(CameraStatusE.Run);
            }
        }
        OnGameModeChanged.Invoke();
    }

    public void NextScene()
    {
        GameData.Instance.SetCurrentLevel(0, GameData.Instance.CurrentLevel < 4 ? GameData.Instance.CurrentLevel + 1:0) ;
        loadingComponent.StartLoadLevel("LevelScene");
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
            Debug.Log(_targetLookaheadTime);
            _targetLookaheadTime = CameraSettings.LookaheadTimeAttack;
            Debug.Log(_targetLookaheadTime);
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

    public int DefinePhysicsLayerByString(string layerName)
    {
        if (layerName == "Line1")
            return 8;
        else if (layerName == "Line12")
            return 9;
        else if (layerName == "Line2")
            return 10;
        else if (layerName == "Line23")
            return 11;
        else if (layerName == "Line3")
            return 12;
        return -1;
    }
}
