using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowingCamera : MonoBehaviour
{
    public float CameraSpeed = 2f;
    public Vector3 TargetViewOffset = Vector3.zero;
    public Transform TargetObject;
    public Movable MovableComponentTarget;
    public float DistanceCoefficient = 10f;

    private Vector3 PrevTargetPositon = Vector3.zero;

    // Start is called before the first frame update
    void Start()
    {
        if (TargetObject != null)
        {
            transform.position = TargetObject.position + TargetViewOffset;
            PrevTargetPositon = TargetObject.position;
            //Debug.Log($"CameraPosition: ({transform.position.x}, {transform.position.y})");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (TargetObject != null)
        {
            //calculate new target position
            Vector3 targetFollowPosition = TargetObject.position + TargetViewOffset + new Vector3((MovableComponentTarget.Speed), 0, 0) * DistanceCoefficient;
            //Debug.Log($"Distance difference: {TargetObject.position - PrevTargetPositon}");
            PrevTargetPositon = TargetObject.position;
            Vector3 targetFollowDirection = (targetFollowPosition - transform.position).normalized;
            float distance = Vector3.Distance(targetFollowPosition, transform.position);

            //Check for overpositioning camera (prevent errors while freezes, low FPS)
            if (distance > 0)
            {
                Vector3 cameraNewPosition = transform.position + targetFollowDirection * distance * CameraSpeed * Time.deltaTime;
                float newDistance = Vector3.Distance(targetFollowPosition, cameraNewPosition);
                if (newDistance > distance)
                {
                    //over move the camera
                    cameraNewPosition = targetFollowPosition;
                    //Debug.Log("Over move the camera");
                }            
                transform.position = cameraNewPosition;
                //Debug.Log($"Camera position: {transform.position}");
                //Debug.Log($"Distance: {distance - newDistance}, Time: {Time.deltaTime}");
            }
        }
    }
}
