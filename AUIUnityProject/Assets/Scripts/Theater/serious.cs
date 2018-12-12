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

        if (Input.GetKeyDown(KeyCode.S)) {
            float r = Random.value;
            animator.SetFloat("rand", r);
        }
    }
}
