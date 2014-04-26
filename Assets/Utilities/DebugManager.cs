using UnityEngine;
using System.Collections;

public class DebugManager : MonoBehaviour 
{
	
	
	int frameRateCounter = 0;
	int lastSecondFrameRate = 0;
	float lastTime = 0;
	
	Rect rect;
	
	static DebugManager instance;
	
	void Awake()
	{
		if(instance == null)
		{
			instance = this;
			rect = new Rect(0, 360, 100, 100);
			DontDestroyOnLoad(gameObject);
		}
		else
			Destroy(gameObject);
	}
	
	void Update()
	{
		if(lastTime + 1f < Time.realtimeSinceStartup)
		{
			lastSecondFrameRate = frameRateCounter;
			frameRateCounter = 0;
			lastTime = Time.realtimeSinceStartup;
		}
		
		frameRateCounter++;
	}
	
	void OnGUI()
	{
		GUI.Label(rect, " FPS: " + lastSecondFrameRate);
	}
	
	
}
