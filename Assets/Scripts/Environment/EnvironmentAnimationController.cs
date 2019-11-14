using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvironmentAnimationController : MonoBehaviour
{
    private Animator anim;
    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(anim) anim.SetTrigger("Contact");
    }
}
