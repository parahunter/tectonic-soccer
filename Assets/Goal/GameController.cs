using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class GameController : MonoBehaviour 
{
	public int[] playerGoals = new int[2];
	public int lastScorer = -1;
	public int winningPlayer = -1;
	public event System.Action onScored;
		
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
		++playerGoals[playerGoal];
		if (playerGoals [0] > playerGoals [1])
			winningPlayer = 0;
		else if (playerGoals [0] < playerGoals [1])
			winningPlayer = 1;
		else
			winningPlayer = -1;

		onScored();
	}
}
