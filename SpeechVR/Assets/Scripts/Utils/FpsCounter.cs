using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FpsCounter : MonoBehaviour 
{

     public float deltaTime;
     private TextMeshProUGUI text;
     private void Start()
     {
         text = GetComponent<TextMeshProUGUI>();
     }

     void Update () {
         deltaTime += (Time.deltaTime - deltaTime) * 0.1f;
         float fps = 1.0f / deltaTime;
         text.text = Mathf.Ceil (fps).ToString ();
     }
     
}
