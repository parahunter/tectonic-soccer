using UnityEngine;
using System.Collections;

public class FanScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
		Color newColor = new Color (Random.Range (0.0f, 1.0f), Random.Range (0.0f, 1.0f), Random.Range (0.0f, 1.0f));
		this.transform.Find ("Body").renderer.material.color = newColor;
		this.transform.Find ("Arms").renderer.material.color = newColor;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
