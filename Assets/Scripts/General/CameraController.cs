using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Rendering;
using Cinemachine;

public class CameraController : MonoBehaviour
{
    public static CameraController Instance;

    public Camera MainCamera;
    public CinemachineVirtualCamera VirtCamera;
    public Volume MainCameraVolume;
    public VolumeProfile VolumeProfile;

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
    private CameraStatusE _targetCameraStatus;
    //These variable stores settings that was set up in current status
    private float _currentRefreshCameraTime;
    private float _currentLookaheadTime;
    private float _currentScreenXPos;
    private CameraStatusE _currentCameraStatus;

    private bool _isNeedToRefreshCamera = false;
    private float _currentRefreshTime = 0f;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    private void Start()
    {
        VolumeProfile = MainCameraVolume.profile;
        //set default camera settings on level start
        SetTargetCameraSettings(CameraStatusE.Run);
        CinemachineFramingTransposer framTransposer = VirtCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
        _currentLookaheadTime = framTransposer.m_LookaheadTime;
        _currentScreenXPos = framTransposer.m_ScreenX;

        _currentRefreshTime = 0f;
        _isNeedToRefreshCamera = true;

        GameController.Instance.OnGameStatusChanged.AddListener(ProcessGameStatusChange);
        PlayerController.Instance.OnHit.AddListener(UpdateHPIndication);

        UpdateHPIndication();
    }

    // Update is called once per frame
    void LateUpdate()
    {
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
            }
        }
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

    private void UpdateHPIndication()
    {
        UnityEngine.Rendering.Universal.Vignette vignette;
        VolumeProfile.TryGet(out vignette);
        if (PlayerController.Instance.MaxHP == PlayerController.Instance.CurrentHP)
            vignette.intensity.Override(0f);
        else
            vignette.intensity.Override(0.25f + 0.30f / PlayerController.Instance.MaxHP * (PlayerController.Instance.MaxHP - PlayerController.Instance.CurrentHP));
    }

    private void ProcessGameStatusChange()
    {
        GameController.GameStatus gameStatus = GameController.Instance.CurrentGameStatus;
        if (gameStatus == GameController.GameStatus.Run)
            SetTargetCameraSettings(CameraStatusE.Run);           
    }
}
