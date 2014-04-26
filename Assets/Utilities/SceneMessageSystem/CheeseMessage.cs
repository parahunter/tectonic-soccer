using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CheeseMessage : MonoBehaviour 
{
	public LovelyCheese content
	{
		get;
		private set;
	}
	
	static CheeseMessage instance;
	
	public static CheeseMessage GetMessage()
	{
		return instance;
	}
	
	public static void CreateMessage(LovelyCheese content)
	{
		if(instance != null)
			Destroy( instance.gameObject );
		
		GameObject obj = new GameObject("CheeseMessage");
		DontDestroyOnLoad( obj );
			
		instance = obj.AddComponent<CheeseMessage>();
		instance.content = content;
	}
}