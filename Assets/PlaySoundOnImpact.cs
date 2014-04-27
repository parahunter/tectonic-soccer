using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlaySoundOnImpact : MonoBehaviour 
{
	public float minPitch = 0.7f;
	public float maxPitch = 1.3f;
	public float minVolume = 0.7f;
	public float maxVolume = 1f;	

	public void OnCollisionEnter()
	{
		if(audio.isPlaying)
			return;
	
		audio.volume = Random.Range(minVolume, maxVolume);
		audio.pitch = Random.Range(minPitch, maxPitch);
		
		audio.Play();	
	}

}
