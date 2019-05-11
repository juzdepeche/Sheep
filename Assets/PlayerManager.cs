using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            Debug.Log(1);
        }
        if (Input.GetButtonDown("Fire2"))
        {
            Debug.Log(21);
        }
        if (Input.GetButtonDown("Fire3"))
        {
            Debug.Log(3);
        }
        if (Input.GetButtonDown("Jump"))
        {
            Debug.Log(4);
        }
    }
}
