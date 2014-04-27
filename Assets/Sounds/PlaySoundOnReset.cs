using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlaySoundOnReset : MonoBehaviour 
{
	public void Start()
	{
		GameController.instance.onReset += Play;
	}
	
	void Play()
	{
		audio.Play();
	}
}
