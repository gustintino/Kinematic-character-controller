using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    int move = 0;
    bool which = true;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(which)
        {
            transform.Translate(0.2f, 0, 0);
            move++;
            if(move == 100)
            {
                which = false;
            }
        }
        else
        {
            transform.Translate(-0.2f, 0, 0);
            move--;
            if (move == 0)
            {
                which = true;
            }
        }

    }
}
