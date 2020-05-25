using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BossEndgameAnimator : EndgameAnimator
{

    public AudioClip Music;

    private void Awake()
    {
        OnAnimationEnd = new UnityEvent();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    /*
    public override void FadeOut()
    {
        GameController.Instance.SetTargetCameraSettings(GameController.CameraStatusE.Center);
        GameController.Instance.PlayerCharacter.transform.position = PlayerPosition.transform.position;
        GameController.Instance.PlayerCharacter.transform.SetParent(PlayerPosition.transform, true);
        GameController.Instance.PlayerCharacter.SetAnimationBool("IsIdle", false);
        AnimatorRef.SetTrigger("FadeOut");
    }
    */
    public override void StartAnimation()
    {
        AnimatorRef.SetBool("IsStart", true);
        SoundManager.Instance.PlayMusicClip(Music, true);
        Cinemachine.CinemachineBrain brain = Camera.main.gameObject.GetComponent<Cinemachine.CinemachineBrain>();
        brain.m_UpdateMethod = Cinemachine.CinemachineBrain.UpdateMethod.LateUpdate;
        brain.m_BlendUpdateMethod = Cinemachine.CinemachineBrain.BrainUpdateMethod.LateUpdate;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void EndAnimation()
    {
        OnAnimationEnd.Invoke();
    }
}
