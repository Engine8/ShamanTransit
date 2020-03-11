using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnowController : MonoBehaviour
{
    [SerializeField]
    private ParticleSystem _particleSystem;

    public bool IsSnowy = false;

    // Start is called before the first frame update
    void Start()
    {
        if (IsSnowy)
            _particleSystem.Play();
        else
            _particleSystem.Stop();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
