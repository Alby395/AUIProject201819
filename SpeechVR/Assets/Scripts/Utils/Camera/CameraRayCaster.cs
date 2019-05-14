using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class CameraRayCaster : MonoBehaviour
{
	[SerializeField] private VRPointer pointer;

	[SerializeField] private float maxDistance;
	[SerializeField] private LayerMask layerMask;

	private Transform _cameraTransform;
	// Use this for initialization
	void Start ()
	{
		_cameraTransform = transform;
	}
	
	// Update is called once per frame
	void Update ()
	{
		
		EyeRaycast();
	}

	/// <summary>
	/// Sends a raycast and check whether the object hit is interactable
	/// </summary>
	private void EyeRaycast()
	{
		
		Ray ray = new Ray(_cameraTransform.position, _cameraTransform.forward);

		RaycastHit hit;
		
		if (Physics.Raycast(ray, out hit, maxDistance, layerMask))
		{
			pointer.StartCount(hit);
		}
		else if (pointer.IsCounting())
		{
			pointer.StopCount();
		}
	}
}
