using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CameraFollow : MonoBehaviour 
{
	public Ball ball;
	
	void Update()
	{
		transform.LookAt( ball.transform.position );
	}
}
