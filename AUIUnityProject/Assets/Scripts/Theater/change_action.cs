using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class change_action : MonoBehaviour {

    Animator animator;

    // Use this for initialization
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update () {

        if (Input.GetKeyDown(KeyCode.S))
        {
            animator.SetTrigger("change_action");
        }
    }
}
