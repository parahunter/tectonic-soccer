using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class AudioPlayer : MonoBehaviour 
{
	
	private AudioSource _source;
	
	void Awake()
	{
		_source = audio;	
	}
	
	public void Play(AudioClip clip, float volume)
	{
		_source.clip = clip;
		_source.volume = volume;
		_source.Play();
		StartCoroutine(_WaitThenRelease());
	}
	
	private IEnumerator _WaitThenRelease()
	{
		yield return new WaitForSeconds(_source.clip.length);
		
		AudioPool.ReleaseAudio(this);
	}
}
