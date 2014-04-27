using UnityEngine;
using System.Collections;

public class SoundController : MonoBehaviour {

	public static SoundController instance 
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
		GameController.instance.onScored += PlaySound;
	}

	void PlaySound()
	{
		GameController controller = GameController.instance;

		int lastScorer = controller.lastScorer;
		int winningPlayer = controller.winningPlayer;
		if (lastScorer == 0)
		{
			// Handle sound for player 0
		}
		else
		{
			// Handles sound for player 1
		}

		if(controller.winningPlayer == lastScorer)
		{
			// Play crazy celebrations
		}
		else if(winningPlayer == -1)
		{
			// Play draw level sound
		}
		else
		{
			// Losing but got a goal :p
		}
	}
}
