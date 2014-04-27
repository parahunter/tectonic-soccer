using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TectonicBody : MonoBehaviour 
{
	
	PitchTectonics pitch;
	
	public float offsetY = 0.5f;
	
	public float bounceForce = 99f;
	
	public float minPitch = 0.8f;
	public float maxPitch = 1.2f;
	public float minVolume = 0.8f;
	public float maxVolume = 1f;
		
	void Start()
	{
		pitch = PitchTectonics.instance;
	}
	
	bool triggeredSound = false;
	
	void FixedUpdate()
	{
		Vector3 position = transform.position + Vector3.down * offsetY;	
		
		Vector2 pos2D = new Vector2( position.x, position.z );
		
		float pitchHeight = pitch.GetHeight(pos2D);
		
		if(pitchHeight > position.y && !triggeredSound && audio != null)
			audio.Play();
		
		else if(pitchHeight < position.y)
			triggeredSound = false;
		
		if(pitchHeight > position.y && !Mathf.Approximately(pitchHeight, 0))
		{
			Vector3 normal = pitch.GetNormal(pos2D);
			
			rigidbody.AddForce(normal * bounceForce);
		}	
	}
	
}
