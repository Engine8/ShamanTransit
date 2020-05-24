using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnowController : MonoBehaviour
{
    private ParticleSystem _particleSystem;
    private WindZone _windZone;

    //current particle system status
    public bool IsSnowy = true;
    public WindStatus WindStatusVar;

    [Tooltip("Define the wind time behaviour (strenght, visualization")]
    public AnimationCurve WindCurve;
    //
    public float SpeedReduce;
    public float AccelerationReduce;
    //Wind main force acting on particle system
    public float WindForce;
    //Standart wind burst time
    public float WindTime;

    //variables for control

    //Current wind burst time (defined when calling methods)
    private float _currentAllWindTime;
    //time from wind burst start
    private float _currentWindTime = 0;
    //current wind status
    private bool _isWindy = true;
    //time for next wind burst (if it is random)
    private float _timeToWind;

    public AudioClip WindSound;
    public float WindSoundFadingTime;
    private bool _soundMarkedToStop = false;


    private void Awake()
    {
        _windZone = transform.GetChild(0).GetComponent<WindZone>();
        _particleSystem = GetComponent<ParticleSystem>();
        _currentAllWindTime = WindTime;
    }

    // Start is called before the first frame update
    void Start()
    {
        if (IsSnowy)
            _particleSystem.Play();
        else
            _particleSystem.Stop();
        _windZone.windMain = 0;
        _isWindy = false;
        if (WindStatusVar == WindStatus.Random)
            _timeToWind = CalculateRandomTime();
    }

    // Update is called once per frame
    void Update()
    {
        if (!IsSnowy)
            return;
        if (_isWindy)
        {
            bool ended = false;
            _currentWindTime += Time.deltaTime;
            if (_currentWindTime > _currentAllWindTime)
            {
                _currentWindTime = _currentAllWindTime;
                ended = true;
            }
            else if (_currentWindTime > (_currentAllWindTime - WindSoundFadingTime) && !_soundMarkedToStop)
            {
                _soundMarkedToStop = true;
                SoundManager.Instance.StopLongSound("wind");
            }

            float _curveValue = WindCurve.Evaluate(_currentWindTime / _currentAllWindTime);
            float windForceValue = Mathf.Lerp(0, WindForce, _curveValue);
            float characterSpeed = Mathf.Lerp(PlayerController.Instance.BasicMaxSpeed, PlayerController.Instance.BasicMaxSpeed - SpeedReduce, _curveValue);
            float characterAcceleration = Mathf.Lerp(PlayerController.Instance.BasicAccelerationModif, PlayerController.Instance.BasicAccelerationModif - AccelerationReduce, _curveValue);
            _windZone.windMain = windForceValue;
            PlayerController.Instance.SetMaxSpeed(characterSpeed);
            PlayerController.Instance.SetAccelerationModif(characterAcceleration);
            if (ended)
            {
                _isWindy = false;
                _currentAllWindTime = WindTime;
                _currentWindTime = 0;
            }
        }
        //calculate time to next random burst only when wind doesn't blow
        else if (WindStatusVar == WindStatus.Random) 
        {
            _timeToWind -= Time.deltaTime;
            if (_timeToWind < 0 && GameController.Instance.CurrentGameStatus != GameController.GameStatus.Attack)
            {
                _timeToWind = CalculateRandomTime();
                CreateWindBurst();
            }
        }

    }

    /* Requests request burst of wind
     * returns true if succesful, false if wind has already activated
     * arguments: float time - time of wind burst, -1 <= time <=0 - endless, >0 - time, not defined - standart time WindTime
     */
    public bool CreateWindBurst(float time = -2f)
    {
        if (_isWindy || !IsSnowy)
            return false;
        if (time <= -1.5f)
            _currentAllWindTime = WindTime;
        else if (time <= 0)
            _currentAllWindTime = -1;
        else
            _currentAllWindTime = time;
        

        _isWindy = true;
        SoundManager.Instance.PlayLongSoundClip("wind", WindSound, true, WindSoundFadingTime);
        _soundMarkedToStop = false;
        return true;
    }

    private float CalculateRandomTime()
    {
        float time = Random.Range(10f, 20f);
        return time;
    }

    private void OnDestroy()
    {
        SoundManager.Instance.StopLongSound("wind");
    }
}
