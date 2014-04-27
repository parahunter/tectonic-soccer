﻿using UnityEngine;
using System.Collections;

public class ScoreCounter : MonoBehaviour {

	public int team = 0;

	void onGoal()
	{
		if(GameController.instance.lastScorer == team)
		{
			renderer.material.mainTexture = null;
		}
	}

	// Use this for initialization
	void Start () {
		GameController.instance.onScored += onGoal;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
