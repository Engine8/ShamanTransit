using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour
{
    public enum EObstacleType
    {
        Slower,
        Deadly,
        Damage
    }
    public List<ObstacleType> Types;

    public bool IsDestructible = false;

    public int DestructionSteps = 0;
    protected int _currentDestructionStep = -1;

    private void OnMouseDown()
    {
        if (IsDestructible)
        {
            ++_currentDestructionStep;

            if (_currentDestructionStep == DestructionSteps - 1)
            {
                Destroy(gameObject);
            }
            else
            {
                SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
                spriteRenderer.color = Colors[_currentDestructionStep];
            }
        }
    }


    [Header("Debug")]
    public List<Color> Colors;

}
