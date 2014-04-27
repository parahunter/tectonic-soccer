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
		position = new Vector3 (transform.position.x, transform.position.y, transform.position.z);
		velocity = new Vector3 (rigidbody.velocity.x, rigidbody.velocity.y, rigidbody.velocity.z);
		rotation = new Quaternion (transform.rotation.x, transform.rotation.y, transform.rotation.z, transform.rotation.w);
		
		GameController.instance.onReset += Reset;
	}
}
