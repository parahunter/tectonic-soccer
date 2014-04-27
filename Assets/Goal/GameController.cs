using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameController : MonoBehaviour 
{
	public int player0Goals = 0;
	public int player1Goals = 0;
		
	public static GameController instance
	{
		get;
		private set;
	}
	
	void Awake()
	{
		instance = this;	
	}
	
	public void OnGoal(int playerGoal)
	{
		
		
	}
	
	
}
