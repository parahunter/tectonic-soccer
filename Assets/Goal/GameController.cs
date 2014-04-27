using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class GameController : MonoBehaviour 
{
	enum GameState
	{
		inPlay = 0,
		goalScord = 1,
	}
	
	private GameState gameState = GameState.inPlay;

	public int[] playerGoals = new int[2];
	public int lastScorer = -1;
	public int winningPlayer = -1;

	public event System.Action onScored;
	private float goalTimer = 0.0f;

	public event System.Action onReset;
	private bool reseting = false;
	public float resetPause = 3.0f;
	public float kickOffDelay = 2.0f;

	private float goalPause = 0.0f;

	public event System.Action onKickOff;
		
	public static GameController instance
	{
		get;
		private set;
	}
	
	void Awake()
	{
		instance = this;	
	}

	void Start()
	{
		if (kickOffDelay < 0.25f) { kickOffDelay = 0.25f; }
		goalPause = resetPause + kickOffDelay;
	}
	
	public void OnGoal(int playerGoal)
	{
		if (gameState == GameState.inPlay) {
			++playerGoals [playerGoal];
			if (playerGoals [0] > playerGoals [1])
				winningPlayer = 0;
			else if (playerGoals [0] < playerGoals [1])
				winningPlayer = 1;
			else
				winningPlayer = -1;

			gameState = GameState.goalScord;
			goalTimer = 0.0f;
			onScored ();
		}
	}

	void Update()
	{
		if (gameState == GameState.goalScord)
		{
			goalTimer += Time.deltaTime;

			if(goalTimer > resetPause && !reseting)
			{
				reseting = true;
				onReset();
			}

			if(goalTimer > goalPause)
			{
				goalTimer = 0.0f;
				reseting = false;
				onKickOff();
			}
		}
	}
}
