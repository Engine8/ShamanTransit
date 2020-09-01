using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EndgameAnimator : MonoBehaviour
{
    public GameObject PlayerPosition;
    public GameObject SoulPoint;
    public Animator TotemAnimator;
    public Animator AnimatorRef;
    public UnityEvent OnAnimationEnd;

    public float TimeBetweenSoulStart = 0.5f;
    public float _nextSoulTime;
    public float Speed;
    public float SoulFadeTime;

    public AudioClip TotemFullSound;

    private void Awake()
    {
        OnAnimationEnd = new UnityEvent();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public virtual void FadeOut()
    {
        GameController.Instance.SetTargetCameraSettings(GameController.CameraStatusE.Center);
        GameController.Instance.PlayerCharacter.transform.position = PlayerPosition.transform.position;
        GameController.Instance.PlayerCharacter.transform.SetParent(PlayerPosition.transform, true);
        GameController.Instance.PlayerCharacter.SetAnimationBool("IsIdle", false);
        AnimatorRef.SetTrigger("FadeOut");
    }

    public virtual void StartAnimation()
    {
        AnimatorRef.SetBool("IsStart", true);
        Cinemachine.CinemachineBrain brain = Camera.main.gameObject.GetComponent<Cinemachine.CinemachineBrain>();
        brain.m_UpdateMethod = Cinemachine.CinemachineBrain.UpdateMethod.LateUpdate;
        brain.m_BlendUpdateMethod = Cinemachine.CinemachineBrain.BrainUpdateMethod.LateUpdate;
    }

    public void StartAnimateSouls()
    {
        GameController.Instance.PlayerCharacter.SetAnimationBool("IsIdle", true);
        StartCoroutine(AnimateSouls());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public virtual void EndAnimation()
    {
        OnAnimationEnd.Invoke();
    }

    private class PositionData
    {
        public Soul soul;
        public Vector3 startPos;
        public Vector3 direction;
        public bool inDestination;
        public float t;
    }

    public IEnumerator AnimateSouls()
    {
        List<Soul> souls = GameController.Instance.PlayerCharacter.GetSouls();
        List<PositionData> data = new List<PositionData>();
        int soulsDone = 0;
        int index = 0;
        _nextSoulTime = Time.time + TimeBetweenSoulStart;

        int half = souls.Count / 2;

        while(soulsDone != souls.Count)
        {
            //start next soul
            if (_nextSoulTime < Time.time && index < souls.Count)
            {
                souls[index].Status = Soul.SoulStatus.Animation;
                PositionData soulData = new PositionData();
                soulData.soul = souls[index];
                soulData.direction = (SoulPoint.transform.position - soulData.soul.transform.position).normalized;
                soulData.startPos = souls[index].transform.position;
                soulData.inDestination = false;
                soulData.t = 0;
                data.Add(soulData);
                _nextSoulTime = Time.time + TimeBetweenSoulStart;
                ++index;
            }

            //process souls
            for (int i = 0; i < data.Count; ++i)
            {
                PositionData soulData = data[i];
                //move souls
                if (!soulData.inDestination)
                {
                    Vector3 newPos = soulData.soul.transform.position + soulData.direction * Speed * Time.deltaTime;
                    float currentDistance = (soulData.soul.transform.position - SoulPoint.transform.position).magnitude;
                    float newDistance = (newPos - SoulPoint.transform.position).magnitude;
                    //check over stepping
                    if (newDistance > currentDistance)
                    {
                        newPos = SoulPoint.transform.position;
                        newDistance = 0;
                    }
                    soulData.soul.transform.position = newPos;
                    //check end
                    if (newDistance < 0.2f)
                    {
                        soulData.inDestination = true;
                    }
                }

                if (soulData.inDestination)
                {
                    soulData.t += Time.deltaTime;
                    Color color = soulData.soul.spriteRenderer.color;
                    float t = Mathf.Min(soulData.t / SoulFadeTime, 1);
                    color.a = Mathf.Lerp(1, 0, t);
                    soulData.soul.spriteRenderer.color = color;

                    if (t == 1)
                    {
                        data.Remove(soulData);
                        --i;
                        ++soulsDone;

                        if (half != 0 && soulsDone == half)
                        {
                            TotemAnimator.SetBool("Half", true);
                        }
                    }
                }
            }
            yield return null;

        }
        TotemAnimator.SetBool("Full", true);
        SoundManager.Instance.PlaySoundClip(TotemFullSound, true);
        yield return new WaitForSeconds(0.5f);
        EndAnimation();
    }
}
