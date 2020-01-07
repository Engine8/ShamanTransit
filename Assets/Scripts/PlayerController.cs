using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : Movable
{
    public GameObject[] Lines;
    //private int _curLineLayer;

    // Start is called before the first frame update
    new void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    new void Update()
    {
        if (Input.GetButtonDown("Up") )
        {
            gameObject.layer -= 1;
            if (gameObject.layer < 8)
                gameObject.layer = 8;
        }
        else if (Input.GetButtonDown("Down"))
        {
            gameObject.layer += 1;
            if (gameObject.layer > 10)
                gameObject.layer = 10;
        }
        //very bad things: need find method to get layer name
        int lineIndex;
        if (gameObject.layer == 8)
            lineIndex = 0;
        else if (gameObject.layer == 9)
            lineIndex = 1;
        else
            lineIndex = 2;
        //Debug.Log($"Current lineIndex = {lineIndex}");
        if (Lines[lineIndex] != null)
        {
            Debug.Log($"Current lineIndex = {lineIndex}, y position = {Lines[lineIndex].transform.position.y}");
            _rb2d.position = new Vector2(_rb2d.position.x, Lines[lineIndex].transform.position.y);
        }
        base.Update();
    }
}
