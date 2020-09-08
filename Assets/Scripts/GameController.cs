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

    [Space(20, order = 0)]
    [Header("UI", order = 1)]
    public GameObject WinScreen;
    public Button ContinueButton;
    public Text WinText;
    public Text WinMoneyText;

    public GameObject LoseScreen;
    public Text LoseText;
    public GameObject AttackUI;

    public AudioClip WinSound;
    public AudioClip DefeatSound;

    //-------------Player--------------------
    [Space(20, order = 0)]
    [Header("Player", order = 1)]
    public PlayerController PlayerCharacter;

    public float DieTime = 2f;
    public float EndgameTime = 2f;
    public Color PlayerDeadColor;

    //------------end player-----------------


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


    private float _lastPlayerCharacterXPosition;
    private float _unitsPassed;

    public bool IsGameEnded;

    public UnityEvent OnGameModeChanged;
    public UnityEvent OnGameStatusChanged;

    public LoadingComponent loadingComponent;

    public GameObject SnowPrefab;
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
        /*
        #if UNITY_EDITOR
        SoundManager.Initialize();
        GameData.Initialize();
        if (PlayerDataController.Instance == null)
            PlayerDataController.Initialize();
        #endif
        */
        OnGameModeChanged = new UnityEvent();
        OnGameStatusChanged = new UnityEvent();
    }

    void Start()
    {
        bool isSnowy = false;
        WindStatus windStatus;
        ChunksPlacer.Instance.GetSnowInfo(out isSnowy, out windStatus);
        CurrentGameMode = ChunksPlacer.Instance.GetMapGameMode();

        _lastPlayerCharacterXPosition = PlayerCharacter.transform.position.x;
        PlayerCharacter.OnLevelEnd.AddListener(LevelEnded);

        _defaultColor = GlobalLight.color;
    }

    private void ShowEndgameUI(bool isGameWin, string addText)
    {
        //SoundManager.Instance.StopMusic();
        //SoundManager.Instance.StopAllSounds();
        if (isGameWin)
        {
            WinScreen.SetActive(true);
            WinText.text = addText;
            //SoundManager.Instance.PlaySoundClip(WinSound, false);
            //if (GameData.Instance.CurrentLevel == 4)
            //    ContinueButton.gameObject.SetActive(false);
            //StartCoroutine(MoneyAnimationStart());
            //UpdatePlayerDataOnLevelPass();
        }
        else
        {
            LoseText.text = addText;
            LoseScreen.SetActive(true);
            //SoundManager.Instance.PlaySoundClip(DefeatSound, false);
        }
        //PlayerDataController.Instance.WriteData();
    }

    private IEnumerator MoneyAnimationStart()
    {
        int moneySum = ChunksPlacer.Instance.GetBasicReward() + ChunksPlacer.Instance.GetMoneyMultiplier();
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
        else //if (PlayerCharacter.SoulCount > 0 || CurrentGameMode == GameMode.BossFight)
            ShowEndgameUI(true, "Ты сделал это!");
        //else
            //ShowEndgameUI(false, "Не было собрано ни одной души");
    }
    
    //updates player data (not write it)
    private void UpdatePlayerDataOnLevelPass()
    {
        //PlayerDataController.Instance.AddMoney(ChunksPlacer.Instance.GetMoneyMultiplier() * PlayerCharacter.SoulCount);
        
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

    public void SetGameStatus(GameStatus gameStatus, bool isNeedToChangeCamera)
    { 
        if (gameStatus == GameStatus.Attack)
        {
            CurrentGameStatus = GameStatus.Attack;
            AttackUI.SetActive(true);
            if (isNeedToChangeCamera)
            {
                OnGameStatusChanged.Invoke();
            }
        }
        else if (gameStatus == GameStatus.BossRun)
        {
            CurrentGameStatus = GameStatus.BossRun;
            if (isNeedToChangeCamera)
            {
                OnGameStatusChanged.Invoke();
            }
        }
        else if (gameStatus == GameStatus.Run)
        {
            CurrentGameStatus = GameStatus.Run;
            if (isNeedToChangeCamera)
            {
                OnGameStatusChanged.Invoke();
            }
        }
        OnGameModeChanged.Invoke();
    }

    public void NextScene()
    {
        GameData.Instance.SetCurrentLevel(0, GameData.Instance.CurrentLevel < 4 ? GameData.Instance.CurrentLevel + 1:0) ;
        loadingComponent.StartLoadLevel("LevelScene");
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
