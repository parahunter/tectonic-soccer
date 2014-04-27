using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlaySoundOnGoal : MonoBehaviour 
{

	public void Start()
	{
		GameController.instance.onScored += Play;
	}

	void Play()
	{
		audio.Play();
	}
	
}
