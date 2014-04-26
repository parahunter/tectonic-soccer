using UnityEngine;
using System.Collections;

public class PlayAudioOnAwake : MonoBehaviour 
{
	public AudioClip clip;
	public float volume = 0.8f;
	
	void Start()
	{
		AudioPool.PlayAudio(clip, transform.position, volume);	
	}
	
}