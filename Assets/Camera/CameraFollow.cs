using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CameraFollow : MonoBehaviour 
{
	public Ball ball;
	
	public float prediction = 3f;
	
	public float followTime;
	
	Vector3 velocity;
	
	void Update()
	{
		Vector3 direction = (ball.transform.position + ball.rigidbody.velocity * prediction - transform.position).normalized;
		
		transform.forward = Vector3.SmoothDamp(transform.forward, direction, ref velocity, followTime);
	}
}
