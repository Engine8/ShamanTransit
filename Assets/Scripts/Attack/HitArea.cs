using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HitArea : MonoBehaviour 
{
    public Transform Arrow;

    public EnemyController EnemyRef;
    public EnemyController BossRef;
    private Image _missAr;
    private SightScale _sightScale;
    private float _pauseAttacksMiss = 0;
    private float _pauseAttacksHit = 0;

    public AudioClip GoodShot;
    public AudioClip BadShot;

    void Awake()
    {
        _sightScale = transform.parent.GetComponent<SightScale>();
        _missAr = transform.parent.transform.GetChild(1).GetComponentInChildren<Image>();
    }

    void Start()
    {
        transform.localEulerAngles = new Vector3(0, 0, -40);
        PlayerController.Instance.OnDieStart.AddListener(OnPlayerCharacterDie);
    }

    public void SetEnemy(EnemyController value)
    {
        EnemyRef = value;
        EnemyRef.OnBattleEnd.AddListener(EndBattle);
        if (EnemyRef.GetEnemyType() == EnemyType.Boss)
        {
            BossRef = EnemyRef;
            ((Boss)BossRef).OnPawnSpawn.AddListener(AddBossPawn);
        }
    }

    public void EndBattle()
    {
        if (EnemyRef.GetEnemyType() != EnemyType.Boss && BossRef != null) //player kill pawn
        {
            Destroy(EnemyRef.gameObject, 9f);
            EnemyRef = BossRef;
        }
        else
        {
            _sightScale.Stop();
            if (EnemyRef.GetEnemyType() == EnemyType.Boss)
            {
                GameController.Instance.SetGameStatus(GameController.GameStatus.BossRun, true);
            }
            else if (EnemyRef.GetCount() == 0)
            {
                GameController.Instance.SetGameStatus(GameController.GameStatus.Run, true);
                Destroy(EnemyRef.gameObject, 9f);
            }
        }
    }

    public void BossBattleSectionStart()
    {
        EnemyRef = BossRef;
        GameController.Instance.SetGameStatus(GameController.GameStatus.Attack, true);
    }

    public void Tach()
    {
        //Debug.Log(sightScale);
        if (!_sightScale.GetVictory())
        {
            PlayerController.Instance.PlayAnimAttack();

            if (Arrow.localEulerAngles.z <= (transform.localEulerAngles.z + 3) && Arrow.localEulerAngles.z >= (_sightScale.SpeedRotate > 0? transform.localEulerAngles.z - 32 : transform.localEulerAngles.z - 34))
            {
                if (Time.time > _pauseAttacksHit)
                {
                    transform.localEulerAngles = new Vector3(0, 0, -Random.Range(0f, 60f));
                    if (EnemyRef.GetActiv())
                    {
                        if (EnemyRef.GetCount() > 0)
                        {
                            EnemyRef.TakeDamage();
                            SoundManager.Instance.PlaySoundClip(GoodShot, true);
                        }
                    }
                    _sightScale.CalculateSpeed();
                    _pauseAttacksHit = Time.time + 0.5f;
                }
            }
            else
            {
                SoundManager.Instance.PlaySoundClip(BadShot, true);
                StartCoroutine("Miss");
                if (Time.time > _pauseAttacksMiss)
                {
                    if (EnemyRef.GetActiv() && BossRef == null)
                        EnemyRef.AttackOnMiss();
                    _pauseAttacksMiss = Time.time + 0.5f;
                }
            }
        }
    }

    IEnumerator Miss()
    {
        _missAr.color = Color.red;

        yield return new WaitForSeconds(0.1f);
        _missAr.color = Color.white;
    }

    //Delete enemies objects when player character dies
    public void OnPlayerCharacterDie()
    {
        _sightScale.Stop();
        if (BossRef == null)
            GameController.Instance.SetGameStatus(GameController.GameStatus.Run, false);
        else
            EnemyRef = BossRef; //if player dies then pawn on scene, boss should take care of that
        EnemyRef.StartPlayerDieAnimation();
    }

    public void AddBossPawn()
    {
        EnemyRef = ((Boss)BossRef).GetPawn();
        EnemyRef.OnBattleEnd.AddListener(EndBattle);
    }
}