using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class Obstacle : MonoBehaviour
{
    public enum ObstacleType
    {
        Slower,
        Deadly
    }
    public ObstacleType Type = ObstacleType.Slower;

    [Tooltip("The value by which the speed of the colliding object will be reduced")]
    public float SpeedReduce = 2f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
