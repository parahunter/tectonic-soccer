using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class BeatDetectionInitialiser : MonoBehaviour 
{
	BeatDetector detector;
	
	// Use this for initialization
	void Start ()
	{
		detector = GameObject.Find("BeatDetector").GetComponent<BeatDetector>();
		detector.Initialize(audio.clip);
		
		detector.SynchAndPlay(audio);
	}
	
}
