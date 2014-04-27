﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TectonicBody : MonoBehaviour 
{
	
	PitchTectonics pitch;
	
	public float offsetY = 0.5f;
	
	public float bounceForce = 99f;
	
	void Start()
	{
		pitch = PitchTectonics.instance;
	}
	
	void FixedUpdate()
	{
		Vector3 position = transform.position + Vector3.down * offsetY;	
		
		Vector2 pos2D = new Vector2( position.x, position.z );
		
		float pitchHeight = pitch.GetHeight(pos2D);
		if(pitchHeight > position.y && !Mathf.Approximately(pitchHeight, 0))
		{
			Vector3 normal = pitch.GetNormal(pos2D);
			
			rigidbody.AddForce(normal * bounceForce);
		}	
	}
	
}