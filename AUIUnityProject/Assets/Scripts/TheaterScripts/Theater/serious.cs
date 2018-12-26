using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class serious : MonoBehaviour {

    Animator animator;

    // Use this for initialization
    void Start () {
        animator = GetComponent<Animator>();
    }
	
	// Update is called once per frame
	void Update () {

        float r;

        if (Input.GetKeyDown(KeyCode.S))
        {
            r = Random.value;
            animator.SetFloat("rand", r);
        }

    }
}
