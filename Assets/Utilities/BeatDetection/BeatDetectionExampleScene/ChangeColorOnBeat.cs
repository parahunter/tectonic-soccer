using UnityEngine;
using System.Collections;

public class ChangeColorOnBeat : MonoBehaviour 
{
	//what band (low = 0, mid = 1, high = 2) 
	public int band = 0;

	BeatDetector detector;

	void Start () 
	{
		//here you tell the beat detector which function it should call
		detector = GameObject.Find("BeatDetector").GetComponent<BeatDetector>();
		detector.AddBeatListener(ChangeColor, band);
	}
	
	void ChangeColor()
	{
		Vector3 col = new Vector3(Random.Range(0,1f),Random.Range(0,1f), Random.Range(0,1f));		
		renderer.material.color = new Color(col.x, col.y, col.z);
	}
	
	void OnDestroy()
	{
		//if you destroy a game object that is connected to the beat detector then remember to remove it from the beat detector
		detector.RemoveBeatListener(ChangeColor, band);	
	}
}
