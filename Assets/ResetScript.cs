using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ResetScript : MonoBehaviour {

	Vector3 position;

	Vector3 velocity;

	Quaternion rotation;

	void Reset()
	{
		rigidbody.velocity = velocity;
		transform.rotation = rotation;
		transform.position = position;
	}

	// Use this for initialization
	void Start () {
		position = transform.position;
		velocity = rigidbody.velocity; 
		rotation = transform.rotation;
		
		GameController.instance.onReset += Reset;
	}
}
